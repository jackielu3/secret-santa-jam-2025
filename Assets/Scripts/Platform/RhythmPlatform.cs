using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RhythmPlatform : MonoBehaviour
{
    [SerializeField] private PlatformBehavior behavior;

    private Renderer _renderer;
    private bool _used = false;

    public PlatformBehavior Behavior => behavior;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        ApplyColor();
    }

    private void OnValidate()
    {
        if (!_renderer) _renderer = GetComponent<Renderer>();
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (_renderer && behavior != null)
        {
            _renderer.material.color = behavior.platformColor;
        }
    }

    public void OnPlayerLanded(Transform playerTransform)
    {
        if (_used) return;
        _used = true;

        if (BeatConductor.Instance == null)
        {
            PlatformSpawner.Instance.SpawnFromPlatform(this, playerTransform);
            return;
        }

        float offsetBeats = BeatConductor.Instance.GetOffsetBeats();

        bool success = Mathf.Abs(offsetBeats) <= behavior.hitWindowBeats;

        if (success)
        {
            PlatformSpawner.Instance.SpawnFromPlatform(this, playerTransform);
        }
        else
        {
            Debug.Log($"Miss! Offset {offsetBeats:F3} beats");
        }
    }

    public void SetBehavior(PlatformBehavior newBehavior)
    {
        behavior = newBehavior;
        ApplyColor();
    }
}
