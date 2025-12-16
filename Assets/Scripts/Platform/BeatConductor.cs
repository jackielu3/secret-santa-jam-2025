using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeatConductor : MonoBehaviour
{
    [Header("Song and Tempo")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float bpm = 120f;
    [SerializeField] private float songOffsetSeconds = 0f;

    public static BeatConductor Instance { get; private set; }

    public float BeatDuration => 60f / bpm;
    public float SongPositionSeconds => Time.fixedTime - songOffsetSeconds; // TODO
    public float SongPositionBeats => SongPositionSeconds / BeatDuration;


    public event Action<long> OnBeat;
    [ReadOnly] private long _lastBeatIndex = -1;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // MUSIC SOURCE CHECK
    }

    private void Update()
    {
        // Music check(?)

        float songPosBeats = SongPositionBeats;
        long beatIndex = Mathf.FloorToInt(songPosBeats);

        if (beatIndex > _lastBeatIndex)
        {
            _lastBeatIndex = beatIndex;
            OnBeat?.Invoke(beatIndex);
            Debug.Log($"{beatIndex}");
        }
    }

    public float GetOffsetSeconds()
    {
        float beats = SongPositionBeats;
        float nearestBeat = Mathf.Round(beats);
        float deltaBeats = beats - nearestBeat;
        return deltaBeats * BeatDuration;
    }

    public float GetOffsetBeats()
    {
        float beats = SongPositionBeats;
        float nearestBeat = Mathf.Round(beats);
        return beats - nearestBeat;
    }
}
