using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class TagManager : MonoBehaviour
{
    public static TagManager Instance;

    [Header("Tag Settings")]
    public float gracePeriodDuration = 3f;

    private PlayerTagState player1;
    private PlayerTagState player2;

    [Header("UI Notification")]
    [Tooltip("UI TextMeshPro element used for countdown and tag notifications.")]
    public TextMeshProUGUI notificationText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(WaitForPlayers());
    }

    IEnumerator WaitForPlayers()
    {
        PlayerTagState[] players = null;
        while (players == null || players.Length < 2)
        {
            yield return new WaitForSeconds(0.1f);
            players = FindObjectsOfType<PlayerTagState>();
        }

        player1 = players[0];
        player2 = players[1];
        Debug.Log("Players found and assigned.");
        AssignInitialRoles();
    }

    void AssignInitialRoles()
    {
        if (player1 == null || player2 == null)
            return;

        if (Random.value > 0.5f)
        {
            player1.isTagger = true;
            player2.isTagger = false;
        }
        else
        {
            player1.isTagger = false;
            player2.isTagger = true;
        }

        player1.UpdateIndicator();
        player2.UpdateIndicator();
    }

    public void SwapRoles(PlayerTagState currentTagger, PlayerTagState currentRunner)
    {
        if (currentTagger.isTagger && !currentRunner.isTagger)
        {
            currentTagger.isTagger = false;
            currentRunner.isTagger = true;

            currentTagger.UpdateIndicator();
            currentRunner.UpdateIndicator();

            currentTagger.ActivateShield(gracePeriodDuration);

            Debug.Log("Roles swapped: " + currentRunner.gameObject.name + " is now the tagger.");

            if (notificationText != null)
            {
                StartCoroutine(ShowTaggedNotification(currentRunner));
            }
        }
    }

    IEnumerator ShowTaggedNotification(PlayerTagState taggedPlayer)
    {
        string playerNotification = "";

        PlayerInput playerInput = taggedPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerNotification = "Player " + (playerInput.playerIndex + 1).ToString() + " TAGGED!";
        }
        else
        {
            playerNotification = taggedPlayer.gameObject.name + " TAGGED!";
        }

        notificationText.text = playerNotification;
        notificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        notificationText.text = "";
        notificationText.gameObject.SetActive(false);
    }
}
