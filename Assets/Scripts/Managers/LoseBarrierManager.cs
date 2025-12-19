using UnityEngine;

public class LoseBarrierManager : MonoBehaviour
{
    public static LoseBarrierManager Instance {  get; private set; }

    public float CurrentLoseHeight { get; private set; } = float.NegativeInfinity;

    private RhythmPlatform _current;
    private RhythmPlatform _next;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetCurrentPlatform(RhythmPlatform platform)
    {
        _next = platform;
        Recompute();
    }

    public void SetNextPlatform(RhythmPlatform platform)
    {
        _next = platform;
        Recompute();
    }

    private void Recompute()
    {
        float a = ComputeLoseHeight(_current);
        float b = ComputeLoseHeight(_next);

        CurrentLoseHeight = Mathf.Min(a, b);
    }

    private float ComputeLoseHeight(RhythmPlatform platform)
    {
        if (platform == null || platform.Behavior == null) return float.PositiveInfinity;

        return platform.transform.position.y - Mathf.Max(0f, platform.Behavior.loseOffset);
    }
}
