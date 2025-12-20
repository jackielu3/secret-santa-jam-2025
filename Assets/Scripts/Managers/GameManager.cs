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

        endUIRoot.SetActive(true);
        RhythmPlatform.OnEnd?.Invoke(score);

        RespawnInitialPlatform();
        Debug.Log("Game Over");
    }

    private void RespawnInitialPlatform()
    {
        foreach (var platform in FindObjectsByType<RhythmPlatform>(FindObjectsSortMode.None))
        {
            if (!platform.Behavior.first)
                Destroy(platform.gameObject);
        }

        if (_currentInitialPlatform != null)
            Destroy(_currentInitialPlatform);

        if (initialPlatformPrefab && initialPlatformSpawnPoint)
        {
            _currentInitialPlatform = Instantiate(
                initialPlatformPrefab,
                initialPlatformSpawnPoint.position,
                initialPlatformSpawnPoint.rotation
                );
        }
    }
}
