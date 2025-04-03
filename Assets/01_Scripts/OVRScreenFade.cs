using UnityEngine;
using System.Collections; 

[HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_screen_fade")]
public class OVRScreenFade : MonoBehaviour
{
    public static OVRScreenFade Instance { get; private set; }

    [Tooltip("Fade duration")]
    public float fadeTime = 1.5f;
    [Tooltip("Screen color at maximum fade")]
    public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);
    public bool fadeOnStart = true;
    public int renderQueue = 5000;
    public Shader fadeShader;
    
    public float currentAlpha
    {
        get { return Mathf.Max(explicitFadeAlpha, animatedFadeAlpha, uiFadeAlpha); }
    }

    private float explicitFadeAlpha = 0.0f;
    private float animatedFadeAlpha = 0.0f;
    private float uiFadeAlpha = 0.0f;

    private MeshRenderer fadeRenderer;
    private MeshFilter fadeMesh;
    private Material fadeMaterial = null;
    private bool isFading = false;
    
    void Start()
    {
        // create the fade material
        fadeMaterial = new Material(fadeShader);
        fadeMesh = gameObject.AddComponent<MeshFilter>();
        fadeRenderer = gameObject.AddComponent<MeshRenderer>();

        var mesh = new Mesh();
        fadeMesh.mesh = mesh;

        Vector3[] vertices = new Vector3[4];

        float width = 2f;
        float height = 2f;
        float depth = 1f;

        vertices[0] = new Vector3(-width, -height, depth);
        vertices[1] = new Vector3(width, -height, depth);
        vertices[2] = new Vector3(-width, height, depth);
        vertices[3] = new Vector3(width, height, depth);

        mesh.vertices = vertices;

        int[] tri = new int[6];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        explicitFadeAlpha = 0.0f;
        animatedFadeAlpha = 0.0f;
        uiFadeAlpha = 0.0f;

        if (fadeOnStart)
        {
            StartCoroutine(Fade(1.0f, 0.0f));
        }

        Instance = this;
    }
    
    void OnEnable()
    {
        if (!fadeOnStart)
        {
            explicitFadeAlpha = 0.0f;
            animatedFadeAlpha = 0.0f;
            uiFadeAlpha = 0.0f;
        }
    }
    
    public IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            animatedFadeAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeTime));
            SetMaterialAlpha();
            yield return new WaitForEndOfFrame();
        }

        animatedFadeAlpha = endAlpha;
        SetMaterialAlpha();
    }
    
    private void SetMaterialAlpha()
    {
        Color color = fadeColor;
        color.a = currentAlpha;
        isFading = color.a > 0;
        if (fadeMaterial != null)
        {
            fadeMaterial.color = color;
            fadeMaterial.renderQueue = renderQueue;
            fadeRenderer.material = fadeMaterial;
            fadeRenderer.enabled = isFading;
        }
    }
}
