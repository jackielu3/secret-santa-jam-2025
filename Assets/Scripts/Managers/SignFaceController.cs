using UnityEngine;

[ExecuteAlways]
public class SignFaceController : MonoBehaviour
{
    [Header("Renderer that draws the sign face")]
    [SerializeField] private Renderer signFaceRenderer;

    [Header("Per-sign settings")]
    [SerializeField] private Texture2D signTexture;
    [SerializeField] private Color fillColor = new Color(1f, 1f, 1f, 1f);

    [SerializeField] private float faceAspectOverride = 0f;

    private static readonly int SignTextureId = Shader.PropertyToID("_SignTexture");
    private static readonly int FillColorId = Shader.PropertyToID("_FillColor");
    private static readonly int FitSizeId = Shader.PropertyToID("_FitSize");

    private MaterialPropertyBlock mpb;

    private void OnEnable() => Apply();
    private void OnValidate() => Apply();

    public void Apply()
    {
        if (!signFaceRenderer) return;

        mpb ??= new MaterialPropertyBlock();
        signFaceRenderer.GetPropertyBlock(mpb);

        mpb.SetTexture(SignTextureId, signTexture);
        mpb.SetColor(FillColorId, fillColor);

        Vector2 fitSize = ComputeContainFitSize();
        mpb.SetVector(FitSizeId, fitSize);

        signFaceRenderer.SetPropertyBlock(mpb);
    }

    private Vector2 ComputeContainFitSize()
    {
        if (!signTexture) return Vector2.one;

        float texAspect = (float)signTexture.width / Mathf.Max(1, signTexture.height);
        float faceAspect = faceAspectOverride > 0f ? faceAspectOverride : EstimateFaceAspectFromBounds();

        if (texAspect > faceAspect)
        {
            float height = faceAspect / texAspect;
            return new Vector2(1f, height);
        }
        else
        {
            float width = texAspect / faceAspect;
            return new Vector2(width, 1f);
        }
    }

    private float EstimateFaceAspectFromBounds()
    {
        Bounds b = signFaceRenderer.bounds;
        Vector3 s = b.size;

        float max = Mathf.Max(s.x, Mathf.Max(s.y, s.z));
        float min = Mathf.Min(s.x, Mathf.Min(s.y, s.z));
        float mid = s.x + s.y + s.z - max - min;

        float width = max;
        float height = mid;

        if (height <= 0.0001f) height = 1f;
        return width / height;
    }

#if UNITY_EDITOR
    [ContextMenu("Apply Now")]
    private void ApplyNow() => Apply();
#endif
}
