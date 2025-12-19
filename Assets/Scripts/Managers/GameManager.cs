using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGameRunning { get; private set; }
    public event Action OnGameOver;

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

        RhythmPlatform.OnEnd?.Invoke();

        OnGameOver?.Invoke();
        Debug.Log("Game Over");
    }
}
