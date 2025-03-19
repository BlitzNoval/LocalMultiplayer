using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string player1CharacterName;
    public string player2CharacterName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
