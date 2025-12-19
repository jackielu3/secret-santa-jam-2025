using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int TotalScore { get; private set; }
    public int LastScore { get; private set; }
    public RhythmPlatform.Judgement LastJudgement { get; private set; } = RhythmPlatform.Judgement.Meh;

    public static System.Action<int, int, RhythmPlatform.Judgement> OnScoreChanged;

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

    private void HandleScored(int score, float deltaBeats, float targetBeat, RhythmPlatform.Judgement judgement)
    {
        LastScore = score;
        LastJudgement = judgement;
        TotalScore += score;

        OnScoreChanged?.Invoke(TotalScore, LastScore, LastJudgement);

        Debug.Log($"Score: {score} ({judgement}) Delta {deltaBeats:0.000} beats, target {targetBeat:0.000} Total={TotalScore}");
    }
}
