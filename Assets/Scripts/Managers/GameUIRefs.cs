using TMPro;
using UnityEngine;

public class GameUIRefs : MonoBehaviour
{
    public static GameUIRefs Instance { get; private set; }

    [Header("Countdown")]
    public GameObject countdownRoot;
    public TMP_Text countdownText;

    [Header("Score UI")]
    public GameObject scoreUIRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
