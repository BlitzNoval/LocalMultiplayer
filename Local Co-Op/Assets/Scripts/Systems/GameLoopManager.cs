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

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

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

        // If no runner is found, default to a tie.
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
