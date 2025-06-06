using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CameraBleedEffect
{
    internal class CameraBleedEffectRendererFeature : ScriptableRendererFeature
    {
        CameraBleedEffectRenderPass m_RenderPass = null;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(m_RenderPass);
            }
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
                m_RenderPass.SetTarget(renderer.cameraColorTargetHandle);
            }
        }

        public override void Create()
        {
            m_RenderPass = new CameraBleedEffectRenderPass();
        }
    }
}