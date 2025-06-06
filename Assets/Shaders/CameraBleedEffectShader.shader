Shader "CameraBleedEffectShader"
{
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Overlay" }
		LOD 100
		ZWrite Off Cull Off
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha // Прозоре накладання

			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

			#pragma vertex Vert
			#pragma fragment Frag

			TEXTURE2D_X(_BlendTex); SAMPLER(sampler_BlendTex);
			TEXTURE2D_X(_BumpMap); SAMPLER(sampler_BumpMap);

			TEXTURE2D_X(_MainTex); SAMPLER(sampler_MainTex);

			float _BlendAmount;
			float _EdgeSharpness;
			float _Distortion;
			float _ShineIntensity;
			float _DarkerColor;

			half4 Frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float2 uv = input.texcoord;

				// Отримуємо текстуру крові
				float4 blendColor = SAMPLE_TEXTURE2D_X(_BlendTex, sampler_BlendTex, uv);

				// Динамічне налаштування прозорості крові
				blendColor.a = saturate(blendColor.a + (_BlendAmount * 2 - 1));
				blendColor.a = saturate(blendColor.a * _EdgeSharpness - (_EdgeSharpness - 1) * 0.5);

				// Використання нормальної карти для спотворення UV
				half2 bump = UnpackNormal(SAMPLE_TEXTURE2D_X(_BumpMap, sampler_BumpMap, uv)).rg;
				float2 distortedUV = uv + bump * blendColor.a * _Distortion;

				// Отримуємо колір сцени
				float4 mainColor = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, distortedUV);

				// Додаємо світловідбиття (імітація мокрих крапель)
				float3 normal = normalize(float3(bump, 1.0));
				float3 viewDir = normalize(_WorldSpaceCameraPos - float4(uv, 0.0, 1)); // Коригуємо напрямок огляду
				float fresnel = pow(1.0 - dot(viewDir, normal), 3.0);

				// Додаємо дзеркальний блиск, який залежить від кута огляду
				float specular = saturate(dot(viewDir, normal)) * _ShineIntensity;
				blendColor.rgb += fresnel * _ShineIntensity + specular * 0.5;

				// Додаткове змішування кольорів для ефекту "розмазування" крові
				float4 overlayColor = blendColor;
				overlayColor.rgb = mainColor.rgb * (blendColor.rgb + 0.5) * 1.2;
				blendColor = lerp(blendColor, overlayColor, 0.3);

				// Ослаблення кольору сцени в місцях, де є кров
				mainColor.rgb *= 1 - blendColor.a * 0.5;

				// Остаточне змішування
				mainColor.rgb = lerp(mainColor.rgb, blendColor.rgb, blendColor.a);
				mainColor.a = max(blendColor.a, 0); // Збереження прозорості фону

				mainColor.r -= _DarkerColor;
				mainColor.g -= _DarkerColor;
				mainColor.b -= _DarkerColor;

				return mainColor;
			}

			ENDHLSL
		}
	}
}
