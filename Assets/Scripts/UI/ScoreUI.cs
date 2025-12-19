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
        RhythmPlatform.OnEnd += OnEnd;

        if (ScoreManager.Instance != null)
        {
            OnScoreChanged(ScoreManager.Instance.TotalScore, ScoreManager.Instance.LastScore, ScoreManager.Instance.LastJudgement);
        }
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= OnScoreChanged;
        RhythmPlatform.OnEnd -= OnEnd;
    }

    private void OnScoreChanged(int total, int last, RhythmPlatform.Judgement judgement)
    {
        if (scoreText) scoreText.text = $"Score: {total}";

        if (lastScoreText)
        {
            if (last > 0)
                lastScoreText.text = $"+{last}";
            else
                lastScoreText.text = string.Empty;
        }

        if (judgementText)
        {
            if (judgement != RhythmPlatform.Judgement.NA)
                judgementText.text = judgement.ToString();
            else
                judgementText.text = string.Empty;
        }
    }

    private void OnEnd()
    {

    }
}
