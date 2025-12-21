using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsGameRunning { get; private set; }

    [Header("Initial Platform Respawn")]
    [SerializeField] private GameObject initialPlatformPrefab;
    [SerializeField] private Transform initialPlatformSpawnPoint;

    [Header("UI Roots")]
    [SerializeField] private GameObject endUIRoot;

    [SerializeField] private AudioClip defaultMusic;

    private GameObject _currentInitialPlatform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartGame() => IsGameRunning = true;

    public void GameOver()
    {
        if (!IsGameRunning) return;
        IsGameRunning = false;

        int score = 0;
        if (ScoreManager.Instance != null) score = ScoreManager.Instance.TotalScore;

        if (endUIRoot != null) endUIRoot.SetActive(true);
        else Debug.LogWarning("GameOver: endUIRoot is not assigned.");

        RhythmPlatform.OnEnd?.Invoke(score);

        if (BeatConductor.Instance != null)
            BeatConductor.Instance.SwitchSong(defaultMusic, 70, 0f, true);
        else
            Debug.LogWarning("GameOver: BeatConductor.Instance is null (can't switch song).");

        RespawnInitialPlatform();
        Debug.Log("Game Over");
    }

    private void RespawnInitialPlatform()
    {
        foreach (var platform in FindObjectsByType<RhythmPlatform>(FindObjectsSortMode.None))
        {
            if (platform != null && platform.Behavior != null && !platform.Behavior.first)
                Destroy(platform.gameObject);
        }

        if (initialPlatformPrefab == null)
        {
            Debug.LogWarning("RespawnInitialPlatform: initialPlatformPrefab is not assigned.");
            return;
        }

        if (initialPlatformSpawnPoint == null)
        {
            Debug.LogWarning("RespawnInitialPlatform: initialPlatformSpawnPoint is not assigned.");
            return;
        }

        if (_currentInitialPlatform != null)
            Destroy(_currentInitialPlatform);

        _currentInitialPlatform = Instantiate(
            initialPlatformPrefab,
            initialPlatformSpawnPoint.position,
            initialPlatformSpawnPoint.rotation
        );

        Debug.Log($"RespawnInitialPlatform: Spawned '{_currentInitialPlatform.name}' at {initialPlatformSpawnPoint.position}");
    }

}
