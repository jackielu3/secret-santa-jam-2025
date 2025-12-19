using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerLanding : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;

    private void OnTriggerEnter(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & platformLayerMask) == 0)
            return;

        RhythmPlatform platform = collision.GetComponent<RhythmPlatform>();
        if (platform != null)
        {
            platform = collision.GetComponentInParent<RhythmPlatform>();
        }

        if (platform != null)
        {
            platform.OnPlayerLanded(transform);
        }
    }
}
