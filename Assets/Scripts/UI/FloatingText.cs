using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform anchor;
    [SerializeField] private GameObject textRoot;

    [Header("Behavior")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);
    [SerializeField] private LayerMask playerLayerMask = ~0;
    [SerializeField] private bool billboardToCamera = true;

    private Transform cam;

    private void Awake()
    {
        cam = Camera.main ? Camera.main.transform : null;

        if (!anchor) anchor = transform;
        if (textRoot) textRoot.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!textRoot || !textRoot.activeSelf) return;

        textRoot.transform.position = anchor.position + offset;

        if (billboardToCamera && cam)
        {
            Vector3 toCam = textRoot.transform.position - cam.position;
            if (toCam.sqrMagnitude > 0.0001f)
                textRoot.transform.rotation = Quaternion.LookRotation(toCam);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsInLayerMask(other.gameObject.layer, playerLayerMask)) return;
        if (textRoot) textRoot.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsInLayerMask(other.gameObject.layer, playerLayerMask)) return;
        if (textRoot) textRoot.SetActive(false);
    }

    private static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
