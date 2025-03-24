using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ObstaclePlacementManager : MonoBehaviour 
{
    public GameObject[] tilesetPrefabs, p1SlotPanels, p2SlotPanels;
    public TMP_Text[] p1SlotNameLabels, p1SlotCountLabels, p2SlotNameLabels, p2SlotCountLabels;
    public GameObject projectilePrefab, conveyorPrefab, topSwingBallPrefab, bottomSwingBallPrefab, windBlowerPrefab, shockPadPrefab;
    public LayerMask obstacleLayer;
    public float obstacleMoveSpeed = 10f;
    public Image fadeImage;
    public float fadeDuration = 1f;
    public string nextSceneName = "Match";
    
    Dictionary<string, int> p1ObstaclesRemaining = new Dictionary<string, int>(), 
                         p2ObstaclesRemaining = new Dictionary<string, int>();
    PlayerInput p1Input, p2Input;
    int p1HoverIndex, p2HoverIndex;
    bool p1IsPlacing, p2IsPlacing, p1Ready, p2Ready;
    GameObject p1CurrentObstacle, p2CurrentObstacle;
    float navCooldownTime = 0.3f, lastNavTimeP1, lastNavTimeP2;

    void Start()
    {
        if (tilesetPrefabs != null && tilesetPrefabs.Length > 0)
            Instantiate(tilesetPrefabs[Random.Range(0, tilesetPrefabs.Length)], Vector3.zero, Quaternion.identity);
        foreach (var dict in new[] { p1ObstaclesRemaining, p2ObstaclesRemaining })
        {
            dict["Projectiles"] = 2;
            dict["Conveyor"] = 1;
            dict["Hang Ball"] = 2;
            dict["Swing Ball"] = 2;
            dict["Wind"] = 1;
            dict["Shock Pad"] = 3;
        }
        foreach (var pi in FindObjectsOfType<PlayerInput>())
        {
            if (pi.playerIndex == 0) p1Input = pi;
            else if (pi.playerIndex == 1) p2Input = pi;
        }
        UpdateP1SlotUI();
        UpdateP2SlotUI();
        p1HoverIndex = FindNextAvailableSlot(true, 0, +1, false);
        p2HoverIndex = FindNextAvailableSlot(false, 0, +1, false);
        HighlightP1Slot();
        HighlightP2Slot();
    }

    void Update()
    {
        HandlePlayerInput(p1Input, true);
        HandlePlayerInput(p2Input, false);
        if (p1Ready && p2Ready)
        {
            SaveObstaclePlacements();
            StartCoroutine(FadeOutAndLoadScene(nextSceneName));
        }
    }

    void HandlePlayerInput(PlayerInput input, bool isP1)
    {
        if (input == null) return;
        float moveX = input.currentActionMap["Move"].ReadValue<Vector2>().x;
        bool isPlacing = isP1 ? p1IsPlacing : p2IsPlacing;
        bool isReady = isP1 ? p1Ready : p2Ready;
        if (!isPlacing && !isReady)
        {
            if (moveX > 0.5f && Time.time - (isP1 ? lastNavTimeP1 : lastNavTimeP2) > navCooldownTime)
            {
                if (isP1) { NavigateP1Slots(+1); lastNavTimeP1 = Time.time; }
                else { NavigateP2Slots(+1); lastNavTimeP2 = Time.time; }
            }
            else if (moveX < -0.5f && Time.time - (isP1 ? lastNavTimeP1 : lastNavTimeP2) > navCooldownTime)
            {
                if (isP1) { NavigateP1Slots(-1); lastNavTimeP1 = Time.time; }
                else { NavigateP2Slots(-1); lastNavTimeP2 = Time.time; }
            }
            if (input.currentActionMap["Jump"].triggered)
                if (isP1) OnP1Select(); else OnP2Select();
        }
        else if (isPlacing)
        {
            Vector2 move = input.currentActionMap["Move"].ReadValue<Vector2>();
            GameObject cur = isP1 ? p1CurrentObstacle : p2CurrentObstacle;
            if (cur != null)
                cur.transform.position += new Vector3(move.x, move.y, 0) * obstacleMoveSpeed * Time.deltaTime;
            if (input.currentActionMap["Jump"].triggered)
                TryPlaceObstacle(cur, isP1);
            if (input.currentActionMap["CancelSelection"].triggered)
                CancelPlacement(isP1);
        }
    }

    void NavigateP1Slots(int dir)
    {
        int old = p1HoverIndex;
        p1HoverIndex = FindNextAvailableSlot(true, p1HoverIndex + dir, dir, true);
        if (p1HoverIndex < 0 || p1HoverIndex >= p1SlotPanels.Length) p1HoverIndex = old;
        HighlightP1Slot();
    }

    void NavigateP2Slots(int dir)
    {
        int old = p2HoverIndex;
        p2HoverIndex = FindNextAvailableSlot(false, p2HoverIndex + dir, dir, true);
        if (p2HoverIndex < 0 || p2HoverIndex >= p2SlotPanels.Length) p2HoverIndex = old;
        HighlightP2Slot();
    }

    int FindNextAvailableSlot(bool isP1, int startIndex, int dir, bool wrap)
    {
        Dictionary<string, int> dict = isP1 ? p1ObstaclesRemaining : p2ObstaclesRemaining;
        GameObject[] panels = isP1 ? p1SlotPanels : p2SlotPanels;
        int count = panels.Length, index = startIndex;
        for (int i = 0; i < count; i++)
        {
            if (index < 0 || index >= count)
            {
                if (wrap) index = (index < 0) ? count - 1 : 0;
                else return startIndex;
            }
            if (dict[GetObstacleKeyFromSlot(isP1, index)] > 0) return index;
            index += dir;
        }
        return startIndex;
    }

    void HighlightP1Slot()
    {
        for (int i = 0; i < p1SlotPanels.Length; i++)
            p1SlotPanels[i].SetActive(i == p1HoverIndex);
    }

    void HighlightP2Slot()
    {
        for (int i = 0; i < p2SlotPanels.Length; i++)
            p2SlotPanels[i].SetActive(i == p2HoverIndex);
    }

    void OnP1Select()
    {
        string key = GetObstacleKeyFromSlot(true, p1HoverIndex);
        if (p1ObstaclesRemaining[key] > 0)
        {
            p1IsPlacing = true;
            p1CurrentObstacle = InstantiateRealObstacle(key);
            p1CurrentObstacle.transform.position = Vector3.zero;
            p1ObstaclesRemaining[key]--;
            UpdateP1SlotUI();
        }
    }

    void OnP2Select()
    {
        string key = GetObstacleKeyFromSlot(false, p2HoverIndex);
        if (p2ObstaclesRemaining[key] > 0)
        {
            p2IsPlacing = true;
            p2CurrentObstacle = InstantiateRealObstacle(key);
            p2CurrentObstacle.transform.position = Vector3.zero;
            p2ObstaclesRemaining[key]--;
            UpdateP2SlotUI();
        }
    }

    string GetObstacleKeyFromSlot(bool isP1, int slotIndex)
    {
        switch (slotIndex)
        {
            case 0: return "Projectiles";
            case 1: return "Conveyor";
            case 2: return "Hang Ball";
            case 3: return "Swing Ball";
            case 4: return "Wind";
            case 5: return "Shock Pad";
            default: return "Projectiles";
        }
    }

    GameObject InstantiateRealObstacle(string key)
    {
        switch (key)
        {
            case "Projectiles": return Instantiate(projectilePrefab);
            case "Conveyor": return Instantiate(conveyorPrefab);
            case "Hang Ball": return Instantiate(topSwingBallPrefab);
            case "Swing Ball": return Instantiate(bottomSwingBallPrefab);
            case "Wind": return Instantiate(windBlowerPrefab);
            case "Shock Pad": return Instantiate(shockPadPrefab);
            default: return null;
        }
    }

    void TryPlaceObstacle(GameObject o, bool isP1)
    {
        if (o == null) return;
        if (IsPositionOccupied(o.transform.position)) return;
        ObstaclePlacementsData.placements.Add(new ObstaclePlacementInfo {
            key = GetKeyFromObstacle(o),
            position = o.transform.position
        });
        if (isP1)
        {
            p1IsPlacing = false;
            p1CurrentObstacle = null;
            if (AllObstaclesPlaced(true)) p1Ready = true;
        }
        else
        {
            p2IsPlacing = false;
            p2CurrentObstacle = null;
            if (AllObstaclesPlaced(false)) p2Ready = true;
        }
    }

    bool IsPositionOccupied(Vector3 pos)
    {
        return Physics2D.OverlapCircle(pos, 0.1f, obstacleLayer) != null;
    }

    bool AllObstaclesPlaced(bool isP1)
    {
        var dict = isP1 ? p1ObstaclesRemaining : p2ObstaclesRemaining;
        foreach (var kv in dict)
            if (kv.Value > 0) return false;
        return true;
    }

    void CancelPlacement(bool isP1)
    {
        if (isP1 && p1CurrentObstacle != null)
        {
            string key = GetObstacleKeyFromSlot(true, p1HoverIndex);
            p1ObstaclesRemaining[key]++;
            Destroy(p1CurrentObstacle);
            p1CurrentObstacle = null;
            p1IsPlacing = false;
            UpdateP1SlotUI();
        }
        else if (!isP1 && p2CurrentObstacle != null)
        {
            string key = GetObstacleKeyFromSlot(false, p2HoverIndex);
            p2ObstaclesRemaining[key]++;
            Destroy(p2CurrentObstacle);
            p2CurrentObstacle = null;
            p2IsPlacing = false;
            UpdateP2SlotUI();
        }
    }

    void UpdateP1SlotUI()
    {
        for (int i = 0; i < p1SlotPanels.Length; i++)
        {
            string key = GetObstacleKeyFromSlot(true, i);
            p1SlotCountLabels[i].text = p1ObstaclesRemaining[key].ToString();
            p1SlotNameLabels[i].text = key;
        }
    }

    void UpdateP2SlotUI()
    {
        for (int i = 0; i < p2SlotPanels.Length; i++)
        {
            string key = GetObstacleKeyFromSlot(false, i);
            p2SlotCountLabels[i].text = p2ObstaclesRemaining[key].ToString();
            p2SlotNameLabels[i].text = key;
        }
    }

    IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color s = fadeImage.color;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float a = Mathf.Clamp01(t / fadeDuration);
                fadeImage.color = new Color(s.r, s.g, s.b, a);
                yield return null;
            }
        }
        SceneManager.LoadScene(sceneName);
    }

    void SaveObstaclePlacements()
    {
        Debug.Log("Placements saved: " + ObstaclePlacementsData.placements.Count);
    }

    public static class ObstaclePlacementsData
    {
        public static List<ObstaclePlacementInfo> placements = new List<ObstaclePlacementInfo>();
    }

    public class ObstaclePlacementInfo
    {
        public string key;
        public Vector3 position;
    }

    string GetKeyFromObstacle(GameObject o)
    {
        string n = o.name;
        if (n.Contains("Projectile")) return "Projectiles";
        if (n.Contains("Conveyor")) return "Conveyor";
        if (n.Contains("topSwing")) return "Hang Ball";
        if (n.Contains("bottomSwing")) return "Swing Ball";
        if (n.Contains("wind")) return "Wind";
        if (n.Contains("shock")) return "Shock Pad";
        return "";
    }
}
