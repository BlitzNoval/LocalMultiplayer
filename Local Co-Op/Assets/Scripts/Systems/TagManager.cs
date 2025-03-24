using UnityEngine;
using System.Collections;

public class TagManager : MonoBehaviour
{
    public static TagManager Instance;

    [Header("Tag Settings")]
    public float gracePeriodDuration = 3f; // Duration of shield effect

    private PlayerTagState player1;
    private PlayerTagState player2;

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

            // Activate grace period shield for new runner
            currentTagger.ActivateShield(gracePeriodDuration);

            Debug.Log("Roles swapped: " + currentRunner.gameObject.name + " is now the tagger.");
        }
    }
}
