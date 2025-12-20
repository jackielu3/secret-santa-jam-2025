using System.Collections;
using UnityEngine;

public interface IPlatformLandingAction
{
    void OnLanded(RhythmPlatform platform, Transform player, float offsetBeats, bool lastInChain);
}

public class RhythmPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlatformBehavior behavior;
    [SerializeField] private Renderer rendererOverride;

    public PlatformBehavior Behavior => behavior;

    [Header("Spawn Logic")]
    [SerializeField] private bool lastInChain;

    private Renderer _renderer;
    private bool _used = false;
    private IPlatformLandingAction _action;

    [Header("Beat Logic")]
    private float _spawnBeat;
    private float _targetBeat;
    private float _expireBeat;
    private Coroutine _despawnRoutine;
    public float TargetBeat => _targetBeat;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock _mpb;

    public enum Judgement { Perfect, Okay, Meh, Miss, NA }
    public static System.Action<int, float, float, Judgement> OnScored;
    public static System.Action<int> OnEnd;

    private void Awake()
    {
        _renderer = rendererOverride != null ? rendererOverride : GetComponent<Renderer>();
        _action = GetComponent<IPlatformLandingAction>();

        ApplyColor();
    }
    private void OnValidate()
    {
        if (rendererOverride == null)
            _renderer = GetComponent<Renderer>();
        else
            _renderer = rendererOverride;

        ApplyColor();
    }

    private void OnEnable()
    {
        ApplyColor();

        if (BeatConductor.Instance == null || behavior == null || behavior.first || behavior.last) return;

        _spawnBeat = BeatConductor.Instance.SongPositionBeats;
        _targetBeat = _spawnBeat + Mathf.Max(0f, behavior.despawnAfterBeats);
        float lifeBeats = Mathf.Max(0f, behavior.despawnAfterBeats) + Mathf.Max(0f, behavior.hitWindowBeats);
        _expireBeat = _targetBeat + Mathf.Max(0f, behavior.hitWindowBeats);

        if (_despawnRoutine != null) StopCoroutine(_despawnRoutine);
        _despawnRoutine = StartCoroutine(DespawnWhenPastExpireBeat());
    }

    private void OnDisable()
    {
        if (_despawnRoutine != null) StopCoroutine(_despawnRoutine);
        _despawnRoutine = null;
    }

    private IEnumerator DespawnWhenPastExpireBeat()
    {
        while (BeatConductor.Instance != null && BeatConductor.Instance.SongPositionBeats < _expireBeat)
            yield return null;

        Destroy(gameObject);
    }

    private void ApplyColor()
    {
        if (_renderer == null || behavior == null) return;

        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetColor(BaseColorId, behavior.platformColor);
        _mpb.SetColor(ColorId, behavior.platformColor);

        _renderer.SetPropertyBlock(_mpb);
    }

    public void OnPlayerLanded(Transform player)
    {
        if (_used) return;
        _used = true;

        float offsetBeats = 0f;
        if (BeatConductor.Instance != null) offsetBeats = BeatConductor.Instance.GetOffsetBeats();

        if (BeatConductor.Instance != null && !behavior.first && !behavior.last)
        {
            float currentBeats = BeatConductor.Instance.SongPositionBeats;
            int score = ComputeScore(currentBeats);
            float deltaBeats = Mathf.Abs(currentBeats - _targetBeat);

            var judgement = GetJudgement(deltaBeats);
            OnScored?.Invoke(score, deltaBeats, _targetBeat, judgement);
        }

        if (_action != null)
        {
            _action.OnLanded(this, player, offsetBeats, lastInChain);
        }

        if (lastInChain) PlatformSpawner.Instance.SpawnFromPlatform(this, player); // default fallback

    }

    public void SetBehavior(PlatformBehavior newBehavior)
    {
        behavior = newBehavior;
        ApplyColor();
    }

    public void SetLastInChain(bool last) => lastInChain = last;

    private int ComputeScore(float currentBeat)
    {
        if (behavior == null) return 0;

        float delta = Mathf.Abs(currentBeat - _targetBeat);
        float t = 1f - Mathf.Clamp01(delta / Mathf.Max(1e-6f, behavior.hitWindowBeats));
        return Mathf.RoundToInt(t * 1000f);
    }

    private Judgement GetJudgement(float deltaBeats)
    {
        if (behavior == null) return Judgement.Meh;

        float perfect = Mathf.Max(0f, behavior.perfectWindowBeats);
        float okay = Mathf.Max(perfect, behavior.okayWindowBeats);

        if (deltaBeats <= perfect) return Judgement.Perfect;
        if (deltaBeats <= okay) return Judgement.Okay;
        return Judgement.Meh;
    }
}
