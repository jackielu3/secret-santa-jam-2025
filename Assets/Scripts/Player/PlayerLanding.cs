using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerLanding : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & platformLayerMask) == 0)
            return;

        RhythmPlatform platform = collision.collider.GetComponent<RhythmPlatform>();
        if (platform != null)
        {
            platform = collision.collider.GetComponentInParent<RhythmPlatform>();
        }

        if (platform != null)
        {
            platform.OnPlayerLanded(transform);
        }
    }
}
