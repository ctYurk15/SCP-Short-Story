using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CameraBleedEffect
{
    public class CustomBleedBehaviour : MonoBehaviour
    {
        public static float BloodAmount = 0; //0-1 //Set this at runtime
        public float TestingBloodAmount = 0.5f; //0-1 //only in Editor (for testing purposes)

        public static float minBloodAmount = 0; //0-1 //the minimum blood amount. You could optionally increase this (at runtime), the lower the player's HP is, to show the player has low health.

        public float EdgeSharpness = 1; //>=1 //defines how sharp the resulting alpha map will be
        public float minAlpha = 0; //0-1
        public float maxAlpha = 1; //0-1
        public float distortion = 0.2f; //refraction: how much the original image is distorted through the blood (value depends on normal map)
        public float darker_color = 0.2f; //0-1

        public bool autoFadeOut = true; //automatically fades out the blood effect (by lowering the BloodAmount value over time)
        public float autoFadeOutAbsReduc = 0.05f; //absolute reduction per seconde
        public float autoFadeOutRelReduc = 0.5f; //relative reduction per seconde

        public float updateSpeed = 20; // (1 / update duration) (how fast the effect updates to the new BloodAmount value)
        private float prevBloodAmount = 0;

        public Texture2D Image; //RGBA
        public Texture2D Normals; //normalmap
        public Shader Shader; //ImageBlendEffect.shader

        private Material _material;

        private ScriptableRendererFeature renderFeature;

        private void Awake()
        {
            if (Shader == null)
            {
                Debug.LogError("Shader is missing!");
                return;
            }

            if (_material)
            {
                DestroyImmediate(_material);
            }

            _material = new Material(Shader);
            if (!_material)
            {
                Debug.LogError("Failed to create material!");
                return;
            }

            _material.SetTexture("_BlendTex", Image);
            _material.SetTexture("_MainTex", Image);
            _material.SetTexture("_BumpMap", Normals);
        }

        private void Update()
        {
            if (autoFadeOut && BloodAmount > 0)
            {
                BloodAmount -= autoFadeOutAbsReduc * Time.deltaTime;
                BloodAmount *= Mathf.Pow(1 - autoFadeOutRelReduc, Time.deltaTime);
                BloodAmount = Mathf.Max(BloodAmount, 0);
            }
        }

        public Material getBleedMaterial()
        {
            if (_material == null )
            {
                return null;
            }
            

            float newBlendAmount = (Application.isPlaying ? BloodAmount : TestingBloodAmount);
            newBlendAmount = Mathf.Clamp01(newBlendAmount * (1 - minBloodAmount) + minBloodAmount);
            newBlendAmount = Mathf.Clamp01(newBlendAmount * (maxAlpha - minAlpha) + minAlpha);
            newBlendAmount = Mathf.Lerp(prevBloodAmount, newBlendAmount, Mathf.Clamp01(updateSpeed * Time.deltaTime));

            _material.SetFloat("_BlendAmount", newBlendAmount);
            _material.SetFloat("_EdgeSharpness", EdgeSharpness);
            _material.SetFloat("_Distortion", distortion);
            _material.SetFloat("_DarkerColor", darker_color);
            //_material.SetFloat("_ShineIntensity", shine_intensity);
            prevBloodAmount = newBlendAmount;

            return _material;
        }
    }
}


