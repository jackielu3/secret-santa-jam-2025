using System.Collections;
using TMPro;
using UnityEngine;

public class EndUI : MonoBehaviour
{
    [SerializeField] private TMP_Text endText;

    private void OnEnable()
    {
        RhythmPlatform.OnEnd += OnEnd;
    }

    private void OnDisable()
    {
        RhythmPlatform.OnEnd -= OnEnd;
    }

    private void OnEnd(int score)
    {
        StartCoroutine(EndSequence(score));
    }

    private IEnumerator EndSequence(int score)
    {
        endText.text = $"Woot Woot!!!\r\n{score} Points Scored!";
        yield return new WaitForSeconds(3f);

        this.gameObject.SetActive(false);
    }
}
