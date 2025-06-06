using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/BloodOverlay")]
public class BleedBehavior : MonoBehaviour
{
    public static float BloodAmount = 0; //0-1 //Set this at runtime
    public float TestingBloodAmount = 0.5f; //0-1 //only in Editor (for testing purposes)

    public static float minBloodAmount = 0; //0-1 //the minimum blood amount. You could optionally increase this (at runtime), the lower the player's HP is, to show the player has low health.
    
    public float EdgeSharpness = 1; //>=1 //defines how sharp the resulting alpha map will be
    public float minAlpha = 0; //0-1
    public float maxAlpha = 1; //0-1
    public float distortion = 0.2f; //refraction: how much the original image is distorted through the blood (value depends on normal map)

    public bool autoFadeOut = true; //automatically fades out the blood effect (by lowering the BloodAmount value over time)
    public float autoFadeOutAbsReduc = 0.05f; //absolute reduction per seconde
    public float autoFadeOutRelReduc = 0.5f; //relative reduction per seconde

    public float updateSpeed = 20; // (1 / update duration) (how fast the effect updates to the new BloodAmount value)
    private float prevBloodAmount = 0;

    public Texture2D Image; //RGBA
    public Texture2D Normals; //normalmap
    public Shader Shader; //ImageBlendEffect.shader
	
	private Material _material;

	private void Awake()
	{
        if (_material)
        {
            DestroyImmediate(_material);
        }

        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
        {
            Shader.EnableKeyword("LINEAR_COLORSPACE");
        }
        else
        {
            Shader.DisableKeyword("LINEAR_COLORSPACE");
        }

        _material = new Material(Shader);
        _material.SetTexture("_BlendTex", Image);
        _material.SetTexture("_BumpMap", Normals);

	}

    public void Update()
    {
        if (autoFadeOut && BloodAmount > 0)
        {
            BloodAmount -= autoFadeOutAbsReduc * Time.deltaTime;
            BloodAmount *= Mathf.Pow(1 - autoFadeOutRelReduc, Time.deltaTime);
            BloodAmount = Mathf.Max(BloodAmount,0);
        }
    }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!Application.isPlaying)
        {
            _material.SetTexture("_BlendTex", Image);
            _material.SetTexture("_BumpMap", Normals);

            //float newBlendAmount = Mathf.Clamp01(Mathf.Clamp01(TestingBloodAmount) * (maxAlpha - minAlpha) + minAlpha);
            //newBlendAmount = newBlendAmount * (1 - minBloodAmount) + minBloodAmount;
            float newBlendAmount = Mathf.Clamp01(TestingBloodAmount) * (1 - minBloodAmount) + minBloodAmount;
            newBlendAmount = Mathf.Clamp01(newBlendAmount * (maxAlpha - minAlpha) + minAlpha);
            newBlendAmount = Mathf.Lerp(prevBloodAmount, newBlendAmount, Mathf.Clamp01(updateSpeed * Time.deltaTime));
            _material.SetFloat("_BlendAmount", newBlendAmount);
            prevBloodAmount = newBlendAmount;
        }
        else
        {
            //float newBlendAmount = Mathf.Clamp01(Mathf.Clamp01(BloodAmount) * (maxAlpha - minAlpha) + minAlpha);
            //newBlendAmount = newBlendAmount * (1 - minBloodAmount) + minBloodAmount;
            float newBlendAmount = Mathf.Clamp01(BloodAmount) * (1 - minBloodAmount) + minBloodAmount;
            newBlendAmount = Mathf.Clamp01(newBlendAmount * (maxAlpha - minAlpha) + minAlpha);
            newBlendAmount = Mathf.Lerp(prevBloodAmount, newBlendAmount, Mathf.Clamp01(updateSpeed * Time.deltaTime));
            _material.SetFloat("_BlendAmount", newBlendAmount);
            prevBloodAmount = newBlendAmount;
        }
        _material.SetFloat("_EdgeSharpness", EdgeSharpness);
        _material.SetFloat("_Distortion", distortion);

		Graphics.Blit(source, destination, _material);
	}
}