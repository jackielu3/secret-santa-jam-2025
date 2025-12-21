using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerLanding : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;

    [SerializeField] private LayerMask waterLayerMask;
    [SerializeField] private GameObject floatObject;

    private void Awake()
    {
        if (floatObject == null)
        {
            Transform t = transform.Find("float");
            if (t != null) floatObject = t.gameObject;
        }

        if (floatObject != null)
            floatObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & waterLayerMask) != 0)
        {
            if (floatObject != null)
            {
                Debug.Log("FLOAT ON");
                floatObject.SetActive(true);
            }

            return;
        }
    }

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

            if (LoseBarrierManager.Instance != null)
            {
                LoseBarrierManager.Instance.SetCurrentPlatform(platform);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & waterLayerMask) != 0)
        {
            if (floatObject != null)
            {
                Debug.Log("FLOAT OFF");
                floatObject.SetActive(false);
            }

        }
    }
}
