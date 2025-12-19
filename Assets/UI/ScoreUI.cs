using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text lastScoreText;
    [SerializeField] private TMP_Text judgementText;

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += OnScoreChanged;
        
        if (ScoreManager.Instance != null)
        {
            OnScoreChanged(ScoreManager.Instance.TotalScore, ScoreManager.Instance.LastScore, ScoreManager.Instance.LastJudgement);
        }
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int total, int last, RhythmPlatform.Judgement judgement)
    {
        if (scoreText) scoreText.text = $"Score {total}";
        if (lastScoreText) lastScoreText.text = $"+{last}";
        if (judgementText) judgementText.text = judgement.ToString();
    }
}
