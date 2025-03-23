using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ObstaclePlacementManager : MonoBehaviour
{
    [Header("Tileset Prefabs")]
    [Tooltip("List of possible tileset prefabs; one will be chosen at random.")]
    public GameObject[] tilesetPrefabs;

    [Header("UI Buttons / Panels")]
    [Tooltip("6 UI buttons (or placeholders) for Player 1.")]
    public GameObject[] p1ObstacleSlots; // e.g. top row
    [Tooltip("6 UI buttons (or placeholders) for Player 2.")]
    public GameObject[] p2ObstacleSlots; // e.g. bottom row

    [Header("Highlight & Text")]
    [Tooltip("Highlight panel to show over a hovered obstacle (P1).")]
    public GameObject p1HighlightPanel;
    [Tooltip("Highlight panel to show over a hovered obstacle (P2).")]
    public GameObject p2HighlightPanel;

    [Tooltip("Text label under each slot showing obstacle name, cost, etc. (optional)")]
    public Text[] p1SlotLabels;
    public Text[] p2SlotLabels;

    [Header("Obstacle Prefabs")]
    public GameObject projectilePrefab;
    public GameObject conveyorPrefab;
    public GameObject topSwingBallPrefab;
    public GameObject bottomSwingBallPrefab;
    public GameObject windBlowerPrefab;
    public GameObject shockPadPrefab;

    [Header("Placement Settings")]
    [Tooltip("Layer or tag used to detect tile collisions / valid placement.")]
    public LayerMask tileLayer;
    [Tooltip("Distance from camera or how we detect snapping to tile. (example)")]
    public float snapRange = 0.5f;

    [Header("Scene Transition")]
    public Image fadeImage; 
    public float fadeDuration = 1f;
    public string nextSceneName = "Match";

    // For demonstration, each player’s obstacle counts
    private Dictionary<string,int> p1ObstaclesRemaining = new Dictionary<string,int>();
    private Dictionary<string,int> p2ObstaclesRemaining = new Dictionary<string,int>();

    // Input references
    private PlayerInput p1Input;
    private PlayerInput p2Input;

    // Index of the currently hovered slot
    private int p1HoverIndex = 0;
    private int p2HoverIndex = 0;

    // Are players in “placement mode”?
    private bool p1IsPlacing = false;
    private bool p2IsPlacing = false;

    // The obstacle being placed
    private GameObject p1CurrentObstacle;
    private GameObject p2CurrentObstacle;

    // Track if each player is “done”
    private bool p1Ready = false;
    private bool p2Ready = false;

    // Reference to the spawned tileset
    private GameObject spawnedTileset;

    private void Start()
    {
        // 1) Spawn a random tileset
        SpawnRandomTileset();

        // 2) Setup each player’s obstacle counts
        // Player 1
        p1ObstaclesRemaining["Projectile"] = 2;
        p1ObstaclesRemaining["Conveyor"]   = 1;
        p1ObstaclesRemaining["TopBall"]    = 2;
        p1ObstaclesRemaining["BotBall"]    = 2;
        p1ObstaclesRemaining["Wind"]       = 1;
        p1ObstaclesRemaining["ShockPad"]   = 3;

        // Player 2
        p2ObstaclesRemaining["Projectile"] = 2;
        p2ObstaclesRemaining["Conveyor"]   = 1;
        p2ObstaclesRemaining["TopBall"]    = 2;
        p2ObstaclesRemaining["BotBall"]    = 2;
        p2ObstaclesRemaining["Wind"]       = 1;
        p2ObstaclesRemaining["ShockPad"]   = 3;

        // 3) Setup initial UI states
        UpdateP1UI();
        UpdateP2UI();
        HighlightP1Slot();
        HighlightP2Slot();

        // 4) Find PlayerInputs in the scene (assuming they’re already spawned)
        // or you might have direct references from your game flow
        PlayerInput[] allInputs = FindObjectsOfType<PlayerInput>();
        foreach (var pi in allInputs)
        {
            // e.g. if you tag them or identify them by index
            if (pi.playerIndex == 0) p1Input = pi;
            else if (pi.playerIndex == 1) p2Input = pi;
        }

        // 5) If you have “SceneFadeIn” for this scene, it will fade in automatically.
        // We only do fade out after all obstacles placed.
    }

    private void Update()
    {
        HandlePlayer1Input();
        HandlePlayer2Input();

        // Check if both are ready => fade out and load next scene
        if (p1Ready && p2Ready)
        {
            StartCoroutine(FadeOutAndLoadScene(nextSceneName));
        }
    }

    // -----------------------------------------------------
    // 1) TILES: Spawn random tileset
    // -----------------------------------------------------
    private void SpawnRandomTileset()
    {
        if (tilesetPrefabs == null || tilesetPrefabs.Length == 0)
        {
            Debug.LogWarning("No tileset prefabs assigned!");
            return;
        }

        int index = Random.Range(0, tilesetPrefabs.Length);
        spawnedTileset = Instantiate(tilesetPrefabs[index], Vector3.zero, Quaternion.identity);
    }

    // -----------------------------------------------------
    // 2) INPUT: Player 1 handling
    // -----------------------------------------------------
    private void HandlePlayer1Input()
    {
        if (p1Input == null) return;

        // Read horizontal movement from “Move” action
        float moveX = p1Input.currentActionMap["Move"].ReadValue<Vector2>().x;

        // If not placing => navigate obstacle slots
        if (!p1IsPlacing && !p1Ready)
        {
            if (Mathf.Abs(moveX) > 0.5f)
            {
                if (moveX > 0.5f) p1HoverIndex++;
                else p1HoverIndex--;
                p1HoverIndex = Mathf.Clamp(p1HoverIndex, 0, p1ObstacleSlots.Length - 1);
                HighlightP1Slot();
            }

            // SELECT => e.g. “Jump” or “Submit”
            if (p1Input.currentActionMap["Jump"].triggered)
            {
                OnP1Select();
            }

            // BACK => e.g. “CancelSelection” or Esc
            if (p1Input.currentActionMap["CancelSelection"].triggered)
            {
                // Possibly do nothing or exit the scene
            }
        }
        else if (p1IsPlacing)
        {
            // The player is in “placement mode”
            // For a quick example, let’s move the obstacle with arrow keys or WASD
            Vector2 move = p1Input.currentActionMap["Move"].ReadValue<Vector2>();
            if (p1CurrentObstacle != null)
            {
                Vector3 pos = p1CurrentObstacle.transform.position;
                pos += new Vector3(move.x, move.y, 0f) * Time.deltaTime * 5f;
                p1CurrentObstacle.transform.position = pos;
            }

            // Press Space to confirm placement
            if (p1Input.currentActionMap["Jump"].triggered)
            {
                // Attempt to place
                TryPlaceObstacle(p1CurrentObstacle, true);
            }

            // Press Back (Esc) to cancel placement
            if (p1Input.currentActionMap["CancelSelection"].triggered)
            {
                // Cancel placement
                CancelPlacement(true);
            }
        }
    }

    // -----------------------------------------------------
    // 3) INPUT: Player 2 handling
    // -----------------------------------------------------
    private void HandlePlayer2Input()
    {
        if (p2Input == null) return;

        float moveX = p2Input.currentActionMap["Move"].ReadValue<Vector2>().x;

        if (!p2IsPlacing && !p2Ready)
        {
            if (Mathf.Abs(moveX) > 0.5f)
            {
                if (moveX > 0.5f) p2HoverIndex++;
                else p2HoverIndex--;
                p2HoverIndex = Mathf.Clamp(p2HoverIndex, 0, p2ObstacleSlots.Length - 1);
                HighlightP2Slot();
            }

            if (p2Input.currentActionMap["Jump"].triggered)
            {
                OnP2Select();
            }

            if (p2Input.currentActionMap["CancelSelection"].triggered)
            {
                // Possibly do nothing or exit
            }
        }
        else if (p2IsPlacing)
        {
            // Move the obstacle around
            Vector2 move = p2Input.currentActionMap["Move"].ReadValue<Vector2>();
            if (p2CurrentObstacle != null)
            {
                Vector3 pos = p2CurrentObstacle.transform.position;
                pos += new Vector3(move.x, move.y, 0f) * Time.deltaTime * 5f;
                p2CurrentObstacle.transform.position = pos;
            }

            if (p2Input.currentActionMap["Jump"].triggered)
            {
                TryPlaceObstacle(p2CurrentObstacle, false);
            }

            if (p2Input.currentActionMap["CancelSelection"].triggered)
            {
                CancelPlacement(false);
            }
        }
    }

    // -----------------------------------------------------
    // 4) OBSTACLE SELECTION
    // -----------------------------------------------------
    private void OnP1Select()
    {
        // Identify which obstacle is in that slot
        // For example, we can store in each slot a name or type
        string obstacleKey = GetObstacleKeyFromSlot(true, p1HoverIndex);

        // Check if we have any left
        if (p1ObstaclesRemaining[obstacleKey] > 0)
        {
            // Enter placement mode
            p1IsPlacing = true;
            p1CurrentObstacle = InstantiateObstacle(obstacleKey);
            p1ObstaclesRemaining[obstacleKey]--;
            UpdateP1UI();
        }
        else
        {
            Debug.Log("Player1 has none of " + obstacleKey + " left!");
        }
    }

    private void OnP2Select()
    {
        string obstacleKey = GetObstacleKeyFromSlot(false, p2HoverIndex);
        if (p2ObstaclesRemaining[obstacleKey] > 0)
        {
            p2IsPlacing = true;
            p2CurrentObstacle = InstantiateObstacle(obstacleKey);
            p2ObstaclesRemaining[obstacleKey]--;
            UpdateP2UI();
        }
        else
        {
            Debug.Log("Player2 has none of " + obstacleKey + " left!");
        }
    }

    private GameObject InstantiateObstacle(string key)
    {
        switch (key)
        {
            case "Projectile": return Instantiate(projectilePrefab);
            case "Conveyor":   return Instantiate(conveyorPrefab);
            case "TopBall":    return Instantiate(topSwingBallPrefab);
            case "BotBall":    return Instantiate(bottomSwingBallPrefab);
            case "Wind":       return Instantiate(windBlowerPrefab);
            case "ShockPad":   return Instantiate(shockPadPrefab);
        }
        return null;
    }

    // Example method that returns an ID or key for the obstacle in that slot
    private string GetObstacleKeyFromSlot(bool isP1, int slotIndex)
    {
        // This can be done via arrays, or you can manually name your 6 slots
        // For demonstration, let’s assume:
        // Slot 0 -> Projectile
        // Slot 1 -> Projectile
        // Slot 2 -> Conveyor
        // Slot 3 -> TopBall
        // Slot 4 -> BotBall
        // Slot 5 -> ShockPad
        // (Adapt to your actual design: 2 projectile, 1 conveyor, 2 top ball, 2 bottom ball, 1 wind, 3 shock, etc.)
        switch (slotIndex)
        {
            case 0: return "Projectile";
            case 1: return "Projectile";
            case 2: return "Conveyor";
            case 3: return "TopBall";
            case 4: return "BotBall";
            case 5: return "ShockPad";
            // If you have more or different layout, adapt
        }
        return "Projectile"; // fallback
    }

    // -----------------------------------------------------
    // 5) OBSTACLE PLACEMENT
    // -----------------------------------------------------
    private void TryPlaceObstacle(GameObject obstacle, bool isP1)
    {
        if (obstacle == null) return;

        // 1) Check if the obstacle is over a valid tile
        if (!IsOverValidTile(obstacle, isP1))
        {
            Debug.Log("Not over a valid tile or not enough space!");
            return;
        }

        // 2) Snap the obstacle to the tile
        Vector3 snappedPos = GetSnappedPosition(obstacle, isP1);
        obstacle.transform.position = snappedPos;

        // 3) Confirm placement
        if (isP1)
        {
            p1IsPlacing = false;
            p1CurrentObstacle = null;
        }
        else
        {
            p2IsPlacing = false;
            p2CurrentObstacle = null;
        }

        // 4) Check if the player has more obstacles left or if they’re done
        if (AllObstaclesPlaced(isP1))
        {
            // Mark as ready
            if (isP1) p1Ready = true;
            else p2Ready = true;
            Debug.Log("Player " + (isP1 ? "1" : "2") + " is ready!");
        }
    }

    private bool IsOverValidTile(GameObject obstacle, bool isP1)
    {
        // Simple example: cast a small overlap circle or box at the obstacle’s position
        // Check if it intersects with a tile in tileLayer
        Vector2 pos = obstacle.transform.position;
        Collider2D hit = Physics2D.OverlapCircle(pos, snapRange, tileLayer);
        if (hit != null)
        {
            // If it’s a bottom swinging ball and we require it to be at the bottom, add logic here
            // e.g. check tile’s tag or y-position
            return true;
        }
        return false;
    }

    private Vector3 GetSnappedPosition(GameObject obstacle, bool isP1)
    {
        // For a simple example, just get the center of the tile we overlapped
        Vector2 pos = obstacle.transform.position;
        Collider2D hit = Physics2D.OverlapCircle(pos, snapRange, tileLayer);
        if (hit != null)
        {
            return hit.bounds.center; // or offset as needed
        }
        return obstacle.transform.position;
    }

    private bool AllObstaclesPlaced(bool isP1)
    {
        // If the dictionary is all zero, we assume done
        Dictionary<string,int> dict = isP1 ? p1ObstaclesRemaining : p2ObstaclesRemaining;
        foreach (var kvp in dict)
        {
            if (kvp.Value > 0) return false; // still have some left
        }
        return true;
    }

    private void CancelPlacement(bool isP1)
    {
        if (isP1)
        {
            if (p1CurrentObstacle != null)
            {
                // Return the piece to the dictionary
                string key = GetObstacleKeyFromSlot(true, p1HoverIndex);
                p1ObstaclesRemaining[key]++;
                Destroy(p1CurrentObstacle);
                p1CurrentObstacle = null;
                p1IsPlacing = false;
                UpdateP1UI();
            }
        }
        else
        {
            if (p2CurrentObstacle != null)
            {
                string key = GetObstacleKeyFromSlot(false, p2HoverIndex);
                p2ObstaclesRemaining[key]++;
                Destroy(p2CurrentObstacle);
                p2CurrentObstacle = null;
                p2IsPlacing = false;
                UpdateP2UI();
            }
        }
    }

    // -----------------------------------------------------
    // 6) UI / HIGHLIGHT
    // -----------------------------------------------------
    private void HighlightP1Slot()
    {
        // Example: move p1HighlightPanel to the slot’s position
        if (p1HoverIndex < 0 || p1HoverIndex >= p1ObstacleSlots.Length) return;
        if (p1HighlightPanel != null && p1ObstacleSlots[p1HoverIndex] != null)
        {
            p1HighlightPanel.transform.position = p1ObstacleSlots[p1HoverIndex].transform.position;
            p1HighlightPanel.SetActive(true);
        }
    }

    private void HighlightP2Slot()
    {
        if (p2HoverIndex < 0 || p2HoverIndex >= p2ObstacleSlots.Length) return;
        if (p2HighlightPanel != null && p2ObstacleSlots[p2HoverIndex] != null)
        {
            p2HighlightPanel.transform.position = p2ObstacleSlots[p2HoverIndex].transform.position;
            p2HighlightPanel.SetActive(true);
        }
    }

    private void UpdateP1UI()
    {
        // For each slot, show how many remain. e.g. “Projectile x 2”
        // If you have text labels under each slot:
        if (p1SlotLabels == null) return;
        for (int i = 0; i < p1SlotLabels.Length; i++)
        {
            string key = GetObstacleKeyFromSlot(true, i);
            if (p1ObstaclesRemaining.ContainsKey(key))
            {
                p1SlotLabels[i].text = key + " x " + p1ObstaclesRemaining[key];
            }
        }
    }

    private void UpdateP2UI()
    {
        if (p2SlotLabels == null) return;
        for (int i = 0; i < p2SlotLabels.Length; i++)
        {
            string key = GetObstacleKeyFromSlot(false, i);
            if (p2ObstaclesRemaining.ContainsKey(key))
            {
                p2SlotLabels[i].text = key + " x " + p2ObstaclesRemaining[key];
            }
        }
    }

    // -----------------------------------------------------
    // 7) FADE OUT & LOAD MATCH SCENE
    // -----------------------------------------------------
    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color startColor = fadeImage.color;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Clamp01(t / fadeDuration);
                fadeImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }
        SceneManager.LoadScene(sceneName);
    }
}
