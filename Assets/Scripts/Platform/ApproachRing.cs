using UnityEngine;

public class ApproachRing : MonoBehaviour
{
    [SerializeField] private RhythmPlatform platform;
    [SerializeField] private float leadBeats = 0f;

    private Vector3 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
        if (!platform) platform = GetComponentInParent<RhythmPlatform>();
    }

    private void OnEnable()
    {
        transform.localScale = startScale;
    }

    private void Update()
    {
        if (!platform || BeatConductor.Instance == null) return;

        float now = BeatConductor.Instance.SongPositionBeats;
        float target = platform.TargetBeat;

        float t = Mathf.InverseLerp(target - leadBeats, target, now);
        t = t * t * (3f - 2f * t);
        float s = Mathf.Lerp(1f, 0f, t);

        transform.localScale = startScale * s;
    }
}
