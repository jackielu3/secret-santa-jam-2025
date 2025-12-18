using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int TotalScore { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        RhythmPlatform.OnScored += HandleScored;
    }

    private void OnDisable()
    {
        RhythmPlatform.OnScored -= HandleScored;
    }

    private void HandleScored(int score, float deltaBeats, float targetBeat)
    {
        TotalScore += score;

        Debug.Log($"Score: {score} (Delta {deltaBeats:0.000} beats, target {targetBeat:0.000}) Total={TotalScore}");
    }
}
