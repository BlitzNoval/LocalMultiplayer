using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class GameLoopManager : MonoBehaviour
{
    [Header("Round Settings")]
    public float roundDuration = 90f;
    private float timer;
    private bool roundActive = false;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText; // UI TextMeshPro element for the countdown

    [Header("Player Spawner Reference")]
    public PlayerSpawner playerSpawner; // Assign the PlayerSpawner object in the Inspector

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Begin with the pre-game countdown.
        StartCoroutine(PreGameCountdown());
    }

    IEnumerator PreGameCountdown()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "TAG!!!";
        yield return new WaitForSeconds(1f);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // Delay the spawn of players until the countdown finishes.
        if (playerSpawner != null)
        {
            playerSpawner.SpawnAllPlayers();
        }
        else
        {
            Debug.LogError("PlayerSpawner reference not assigned in GameLoopManager.");
        }

        StartRound();
    }

    void StartRound()
    {
        timer = roundDuration;
        roundActive = true;
        StartCoroutine(RoundTimer());
    }

    IEnumerator RoundTimer()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(timer).ToString();
            }
            yield return null;
        }

        roundActive = false;
        EndRound();
    }

    void EndRound()
    {
        PlayerTagState[] players = FindObjectsOfType<PlayerTagState>();
        string winnerDisplay = "";

        foreach (PlayerTagState player in players)
        {
            if (!player.isTagger)
            {
                PlayerInput playerInput = player.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    winnerDisplay = "Player " + (playerInput.playerIndex + 1).ToString() + " wins!";
                }
                else
                {
                    winnerDisplay = player.gameObject.name + " wins!";
                }
                break;
            }
        }

        if (string.IsNullOrEmpty(winnerDisplay))
            winnerDisplay = "Tie!";

        if (winnerText != null)
            winnerText.text = winnerDisplay;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
