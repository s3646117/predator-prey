using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripple : MonoBehaviour
{
    public Camera camera;
    public float seaLevel = -0.1f;

    /* Preset ripple amplitude curve */
    public AnimationCurve waveform = new AnimationCurve(
        new Keyframe(0.00f, 0.50f, 0, 0), new Keyframe(0.05f, 1.00f, 0, 0), new Keyframe(0.15f, 0.10f, 0, 0), new Keyframe(0.25f, 0.80f, 0, 0),
        new Keyframe(0.35f, 0.30f, 0, 0), new Keyframe(0.45f, 0.60f, 0, 0), new Keyframe(0.55f, 0.40f, 0, 0), new Keyframe(0.65f, 0.55f, 0, 0),
        new Keyframe(0.75f, 0.46f, 0, 0), new Keyframe(0.85f, 0.52f, 0, 0), new Keyframe(0.99f, 0.50f, 0, 0)
    );

    [Range(0.01f, 1.0f)]

    /* Refraction intensity, which is the intensity of the ripple effect */
    public float refractionStrength = 0.5f;

    /* Reflective default color gray */
    public Color reflectionColor = Color.gray;

    [Range(0.01f, 1.0f)]
    /* The intensity of the reflection effect can be understood as the shadow of the ripple */
    public float reflectionStrength = 0.7f;

    [Range(0.0f, 3.0f)]
    /* transmission speed */
    public float waveSpeed = 1.25f;
    [SerializeField]
    Shader shader;


    class Droplet
    {
        Vector2 position;
        float time = 1000.0f;

        public Droplet() { }

        public void Reset(Vector2 pos)
        {
            position += new Vector2(0.5f, 0.5f);
            time = 0;
        }

        public void Update()
        {
            time += Time.deltaTime;
        }

        public Vector4 MakeShaderParameter(float aspect)
        {
            return new Vector4(position.x * aspect, position.y, time, 0);
        }
    }

    Droplet[] droplets;
    Texture2D gradTexture;
    Material material;
    float timer;
    int dropCount;

    void UpdateShaderParameters()
    {
        material.SetVector("_Drop1", droplets[0].MakeShaderParameter(camera.aspect));
        material.SetVector("_Drop2", droplets[1].MakeShaderParameter(camera.aspect));
        material.SetVector("_Drop3", droplets[2].MakeShaderParameter(camera.aspect));
        material.SetFloat("_SeaLevel", seaLevel);
        material.SetColor("_Reflection", reflectionColor);
        material.SetVector("_Params1", new Vector4(camera.aspect, 1, 1 / waveSpeed, 0));
        material.SetVector("_Params2", new Vector4(1, 1 / camera.aspect, refractionStrength, reflectionStrength));
    }
    void Start()
    {
        if (camera == null)
            Debug.Log(" Camera is null ");
        droplets = new Droplet[3];

        /* Initialize ripple data */
        for(int i=0; i<droplets.Length; i++)
        {
            droplets[i] = new Droplet();
        }
        gradTexture = new Texture2D(2048, 1, TextureFormat.Alpha8, false);
        gradTexture.wrapMode = TextureWrapMode.Clamp;
        gradTexture.filterMode = FilterMode.Bilinear;

        /* Initialize the amplitude map (that is, initialize the waveform curve to gradTexture) */
        for (var i = 0; i < gradTexture.width; i++)
        {
            var x = 1.0f / gradTexture.width * i;
            var a = waveform.Evaluate(x);
            gradTexture.SetPixel(i, 0, new Color(a, a, a, a));
        }
        gradTexture.Apply();
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        material.SetTexture("_GradTex", gradTexture);
        UpdateShaderParameters();
    }

    // Update is called once per frame
    void Update()
    {
        /* Update every ripple */
        foreach (var d in droplets) d.Update();
        UpdateShaderParameters();
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        /* Effect on the screen */
        Graphics.Blit(source, destination, material);
    }
    public void SetSeaLevel(float seaLevel_)
    {
        seaLevel = seaLevel_;
    }

    /* Call this to emit a ripple */
    public void Emit(Vector2 pos)
    {
        droplets[dropCount++ % droplets.Length].Reset(pos);
    }
}

