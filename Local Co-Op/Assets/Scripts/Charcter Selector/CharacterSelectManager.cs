using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class CharacterSelectManager : MonoBehaviour
{
    private enum PlayerSelectState
    {
        Selecting,
        Selected,
        Ready
    }

    [Header("Characters")]
    public CharacterData[] characters; 

    private bool[] isCharacterTaken;

    private int player1Index = 0;
    private int player2Index = 0;

    private PlayerSelectState p1State = PlayerSelectState.Selecting;
    private PlayerSelectState p2State = PlayerSelectState.Selecting;

    [Header("UI - Character Indicators")]
    public GameObject[] p1SelectIndicators;
    public GameObject[] p2SelectIndicators;

    [Header("UI - Bottom Panels (Player1)")]
    public Image player1Image;
    public TMP_Text player1NameText;
    public TMP_Text player1StatusText;
    public TMP_Text player1PromptText;

    [Header("UI - Bottom Panels (Player2)")]
    public Image player2Image;
    public TMP_Text player2NameText;
    public TMP_Text player2StatusText;
    public TMP_Text player2PromptText;

    [Header("Input")]
    public PlayerInput player1Input; 
    public PlayerInput player2Input; 

    [Header("Key Prompts")]
    public string selectKeyP1 = "Space";
    public string selectKeyP2 = "A";
    public string readyKeyP1  = "Space";
    public string readyKeyP2  = "A";
    public string backKeyP1   = "ESC";
    public string backKeyP2   = "B";

    [Header("Scene Transition")]
    public Image fadeImage;            
    public float fadeDuration = 1.0f;   

    private float p1LastInputTime, p2LastInputTime;
    private float inputCooldown = 0.2f; 

    private Coroutine p1SelectingRoutine;
    private Coroutine p2SelectingRoutine;

    private void Awake()
    {
        isCharacterTaken = new bool[characters.Length];

    }

    private void Start()
    {
        UpdatePlayer1UI();
        UpdatePlayer2UI();
        UpdateIndicatorUI();

        p1SelectingRoutine = StartCoroutine(AnimateSelectingText(player1StatusText, 1));
        p2SelectingRoutine = StartCoroutine(AnimateSelectingText(player2StatusText, 2));


    Debug.Log("Player1Input: " + (player1Input != null ? "Assigned" : "NULL"));
    Debug.Log("Player2Input: " + (player2Input != null ? "Assigned" : "NULL"));

    if (player1Input != null)
        Debug.Log("Player1 Default Map: " + player1Input.currentActionMap.name);
    
    if (player2Input != null)
        Debug.Log("Player2 Default Map: " + player2Input.currentActionMap.name);


    }

    private void Update()
    {
        if (player1Input != null)
        {
            Vector2 p1move = player1Input.currentActionMap["Move"].ReadValue<Vector2>();
            HandleHorizontalNav(ref player1Index, p1move.x, 1);
            
            if (player1Input.currentActionMap["Jump"].triggered)
                OnSelectOrReady(1);

            if (player1Input.currentActionMap["CancelSelection"].triggered)
                OnBack(1);
        }

        if (player2Input != null)
        {
            Vector2 p2move = player2Input.currentActionMap["Move"].ReadValue<Vector2>();
            HandleHorizontalNav(ref player2Index, p2move.x, 2);

            if (player2Input.currentActionMap["Jump"].triggered)
                OnSelectOrReady(2);

            if (player2Input.currentActionMap["CancelSelection"].triggered)
                OnBack(2);
        }

        if (p1State == PlayerSelectState.Ready && p2State == PlayerSelectState.Ready)
        {
            GameManager.Instance.player1CharacterName = characters[player1Index].characterName;
            GameManager.Instance.player2CharacterName = characters[player2Index].characterName;

            StartCoroutine(FadeOutAndLoadScene("ObstaclePlacementScene"));
        }
    }

    private void HandleHorizontalNav(ref int currentIndex, float moveX, int playerNumber)
    {
        PlayerSelectState state = (playerNumber == 1) ? p1State : p2State;
        if (state == PlayerSelectState.Ready)
            return;

        if (Mathf.Abs(moveX) > 0.5f)
        {
            float lastInput = (playerNumber == 1) ? p1LastInputTime : p2LastInputTime;
            if (Time.time - lastInput > inputCooldown)
            {
                int dir = (moveX > 0) ? 1 : -1;
                currentIndex = GetNextAvailableCharacterIndex(currentIndex, dir);

                if (playerNumber == 1)
                {
                    UpdatePlayer1UI();
                    p1LastInputTime = Time.time;
                }
                else
                {
                    UpdatePlayer2UI();
                    p2LastInputTime = Time.time;
                }
                UpdateIndicatorUI();
            }
        }
    }

    private int GetNextAvailableCharacterIndex(int startIndex, int direction)
    {
        int newIndex = startIndex;
        int loopCount = 0;
        do
        {
            newIndex = newIndex + direction;
            if (newIndex < 0) newIndex = characters.Length - 1;
            if (newIndex >= characters.Length) newIndex = 0;

            loopCount++;
            if (loopCount > characters.Length)
                break;

        } while (isCharacterTaken[newIndex]);

        return newIndex;
    }

    private void OnSelectOrReady(int playerNumber)
    {
        if (playerNumber == 1)
        {
            switch (p1State)
            {
                case PlayerSelectState.Selecting:

                    if (!isCharacterTaken[player1Index])
                    {
                        isCharacterTaken[player1Index] = true; 
                        p1State = PlayerSelectState.Selected;
                        

                        if (p1SelectingRoutine != null) StopCoroutine(p1SelectingRoutine);
                        player1StatusText.text = "Selected";
                        player1PromptText.text = $"Press ({readyKeyP1}) to Ready!";
                    }
                    break;
                case PlayerSelectState.Selected:
                    p1State = PlayerSelectState.Ready;
                    player1StatusText.text = "Ready";
                    player1PromptText.text = $"Press ({backKeyP1}) to go back!";
                    break;
                case PlayerSelectState.Ready:
                    // hmmmmm
                    break;
            }
        }
        else 
        {
            switch (p2State)
            {
                case PlayerSelectState.Selecting:
                    if (!isCharacterTaken[player2Index])
                    {
                        isCharacterTaken[player2Index] = true;
                        p2State = PlayerSelectState.Selected;

                        // Stop the selecting animation
                        if (p2SelectingRoutine != null) StopCoroutine(p2SelectingRoutine);
                        player2StatusText.text = "Selected";
                        player2PromptText.text = $"Press ({readyKeyP2}) to Ready!";
                    }
                    break;
                case PlayerSelectState.Selected:
                    p2State = PlayerSelectState.Ready;
                    player2StatusText.text = "Ready";
                    player2PromptText.text = $"Press ({backKeyP2}) to go back!";
                    break;
                case PlayerSelectState.Ready:
                    // hmmmmmm2
                    break;
            }
        }
    }

    private void OnBack(int playerNumber)
    {
        if (playerNumber == 1)
        {
            switch (p1State)
            {
                case PlayerSelectState.Ready:
                    p1State = PlayerSelectState.Selected;
                    player1StatusText.text = "Selected";
                    player1PromptText.text = $"Press ({readyKeyP1}) to Ready!";
                    break;
                case PlayerSelectState.Selected:
                    // Unselect the character, make it available again
                    isCharacterTaken[player1Index] = false;
                    p1State = PlayerSelectState.Selecting;
                    player1PromptText.text = $"Press ({selectKeyP1}) to Choose a character";
                    // Restart the selecting animation
                    if (p1SelectingRoutine != null) StopCoroutine(p1SelectingRoutine);
                    p1SelectingRoutine = StartCoroutine(AnimateSelectingText(player1StatusText, 1));
                    break;
                case PlayerSelectState.Selecting:
                    // Possibly exit or do nothing
                    // For now, do nothing
                    break;
            }
        }
        else // playerNumber == 2
        {
            switch (p2State)
            {
                case PlayerSelectState.Ready:
                    p2State = PlayerSelectState.Selected;
                    player2StatusText.text = "Selected";
                    player2PromptText.text = $"Press ({readyKeyP2}) to Ready!";
                    break;
                case PlayerSelectState.Selected:
                    // Unselect
                    isCharacterTaken[player2Index] = false;
                    p2State = PlayerSelectState.Selecting;
                    player2PromptText.text = $"Press ({selectKeyP2}) to Choose a character";
                    if (p2SelectingRoutine != null) StopCoroutine(p2SelectingRoutine);
                    p2SelectingRoutine = StartCoroutine(AnimateSelectingText(player2StatusText, 2));
                    break;
                case PlayerSelectState.Selecting:
                    // Possibly exit or do nothing????
                    break;
            }
        }
    }

    private void UpdatePlayer1UI()
    {
        CharacterData charData = characters[player1Index];
        player1NameText.text = charData.characterName;
        player1Image.sprite = charData.characterSprite;
    }

    private void UpdatePlayer2UI()
    {
        CharacterData charData = characters[player2Index];
        player2NameText.text = charData.characterName;
        player2Image.sprite = charData.characterSprite;
    }

    private void UpdateIndicatorUI()
    {
        for (int i = 0; i < p1SelectIndicators.Length; i++)
            p1SelectIndicators[i].SetActive(i == player1Index);

        for (int i = 0; i < p2SelectIndicators.Length; i++)
            p2SelectIndicators[i].SetActive(i == player2Index);
    }

    IEnumerator AnimateSelectingText(TMP_Text statusText, int playerNum)
    {
        string baseString = "Selecting";
        string[] dotsCycle = new string[] { ".", "..", "..." };
        int idx = 0;

        while (true)
        {
            bool isSelecting = false;
            if (playerNum == 1) isSelecting = (p1State == PlayerSelectState.Selecting);
            else                isSelecting = (p2State == PlayerSelectState.Selecting);

            if (isSelecting)
            {
                statusText.text = baseString + dotsCycle[idx];
                idx = (idx + 1) % dotsCycle.Length;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
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
        SceneManager.LoadScene(sceneName);
    }
}
