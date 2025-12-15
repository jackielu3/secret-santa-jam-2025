#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public interface IPlatformLandingAction
{
    void OnLanded(RhythmPlatform platform, Transform player, float offsetBeats, bool lastInChain);
}

[RequireComponent(typeof(Renderer))]
public class RhythmPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlatformBehavior behavior;
    public PlatformBehavior Behavior => behavior;

    [Header("Spawn Logic")]
    [SerializeField] private bool lastInChain;

    private Renderer _renderer;
    private bool _used = false;
    private IPlatformLandingAction _action;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _action = GetComponent<IPlatformLandingAction>();

        ApplyColorRuntime();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;

        CacheRenderer();
        ApplyColorEditor();
    }
#endif

    private void CacheRenderer()
    {
        if (_renderer == null || behavior == null) return;

        _renderer.material.color = behavior.platformColor;
    }

    private void ApplyColorRuntime()
    {
        if (_renderer && behavior != null)
        {
            _renderer.material.color = behavior.platformColor;
        }
    }

#if UNITY_EDITOR
    private void ApplyColorEditor()
    {
        if (_renderer == null || behavior == null) return;
        if (_mpb == null) _mpb = new MaterialPropertyBlock();

        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetColor(BaseColorId, behavior.platformColor);

        _renderer.SetPropertyBlock(_mpb);
    }
#endif

    public void OnPlayerLanded(Transform player)
    {
        if (_used) return;
        _used = true;

        float offsetBeats = 0f;
        if (BeatConductor.Instance != null) offsetBeats = BeatConductor.Instance.GetOffsetBeats();

        if (_action != null)
        {
            _action.OnLanded(this, player, offsetBeats, lastInChain);
        }

        if (lastInChain) PlatformSpawner.Instance.SpawnFromPlatform(this, player); // default fallback

    }

    public void SetBehavior(PlatformBehavior newBehavior)
    {
        behavior = newBehavior;
        CacheRenderer();
        ApplyColorRuntime();
    }

    public void SetLastInChain(bool last) => lastInChain = last;
}
