using System.Collections.Generic;
using UnityEngine;

public class DottedLinkDots : MonoBehaviour
{
    [Header("Endpoints")]
    public Transform a;
    public Transform b;

    public Vector3 aLocalOffset;
    public Vector3 bLocalOffset;

    public Vector3 aWorldOffset;
    public Vector3 bWorldOffset;
    public bool useWorldOffsets = false;

    [Header("Dot Settings")]
    public Transform dotPrefab;
    public float width = 0.08f;
    public float spacingMultiplier = 2.5f;

    private readonly List<Transform> dots = new();
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (a == null || b == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 p0 = useWorldOffsets ? (a.position + aWorldOffset) : a.TransformPoint(aLocalOffset);
        Vector3 p1 = useWorldOffsets ? (b.position + bWorldOffset) : b.TransformPoint(bLocalOffset);

        float dist = Vector3.Distance(p0, p1);
        if (dist < 0.001f) return;

        float spacing = Mathf.Max(0.001f, width * spacingMultiplier);
        int count = Mathf.Max(2, Mathf.FloorToInt(dist / spacing) + 1);

        EnsureDotCount(count);

        Vector3 camForward = cam ? cam.transform.forward : Vector3.forward;

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0f : (float)i / (count - 1);
            Vector3 pos = Vector3.Lerp(p0, p1, t);

            Transform d = dots[i];
            d.position = pos;
            d.localScale = Vector3.one * width;

            d.forward = camForward;
        }
    }

    private void EnsureDotCount(int count)
    {
        while (dots.Count < count)
        {
            Transform d = Instantiate(dotPrefab, transform);
            dots.Add(d);
        }

        while (dots.Count > count)
        {
            Transform d = dots[^1];
            dots.RemoveAt(dots.Count - 1);
            Destroy(d.gameObject);
        }
    }
}
