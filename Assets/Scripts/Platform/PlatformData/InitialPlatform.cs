using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InitialPlatform : MonoBehaviour, IPlatformLandingAction
{
    [Header("Lift")]
    [SerializeField] private Transform platformRoot;
    [SerializeField] private float liftDuration = 5f;
    [SerializeField] private float liftSpeed = 1.5f;
    [SerializeField] private float maxLiftHeight = 8f;

    [Header("Countdown")]
    [SerializeField] private float countdownSeconds = 3f;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject countdownRoot;

    [Header("Game Start")]
    [SerializeField] private GameObject scoreUIRoot;
    [SerializeField] private bool resetScoreOnStart = true;

    [Header("Music Switch")]
    [SerializeField] private AudioClip gameThemeClip;
    [SerializeField] private float gameThemeBpm = 90f;
    [SerializeField] private float gameThemeOffsetSeconds = 0f;

    bool _started;

    public void Awake()
    {
        var ui = GameUIRefs.Instance;
        if (ui != null)
        {
            if (countdownRoot == null) countdownRoot = ui.countdownRoot;
            if (countdownText == null) countdownText = ui.countdownText;
            if (scoreUIRoot == null) scoreUIRoot = ui.scoreUIRoot;
        }
    }

    public void OnLanded(RhythmPlatform platform, Transform player, float offsetBeats, bool lastInChain)
    {
        if (_started) return;
        _started = true;

        if (!platformRoot) platformRoot = transform;

        StartCoroutine(StartSequence(platform, player));
    }

    private IEnumerator StartSequence(RhythmPlatform platform, Transform player)
    {
        float startY = platformRoot.position.y;
        float targetY = startY + maxLiftHeight;

        float t = 0f;
        while (t < liftDuration && platformRoot.position.y < targetY)
        {
            platformRoot.position += Vector3.up * (liftSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        var p = platformRoot.position;
        p.y = Mathf.Min(p.y, targetY);
        platformRoot.position = p;

        if (countdownRoot) countdownRoot.SetActive(true);
        if (countdownText) countdownText.gameObject.SetActive(true);

        float remaining = countdownSeconds;
        while (remaining > 0f)
        {
            int shown = Mathf.CeilToInt(remaining);
            if (countdownText) countdownText.text = shown.ToString();
            remaining -= Time.deltaTime;
            yield return null;
        }

        if (countdownText) countdownText.text = "Go!";

        yield return new WaitForSeconds(0.2f);

        if (countdownRoot) countdownRoot.SetActive(false);

        if (BeatConductor.Instance != null)
        {
            BeatConductor.Instance.SwitchSong(gameThemeClip, gameThemeBpm, gameThemeOffsetSeconds, playImmediately: true);
        }

        if (resetScoreOnStart && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        if (scoreUIRoot) scoreUIRoot.SetActive(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }

        if (PlatformSpawner.Instance != null && LoseBarrierManager.Instance != null)
        {
            PlatformSpawner.Instance.SpawnFromPlatform(platform, player);
            LoseBarrierManager.Instance.SetCurrentPlatform(platform);
        }

        yield return new WaitForSeconds(1.6f);
        Destroy(gameObject);
    }
}
