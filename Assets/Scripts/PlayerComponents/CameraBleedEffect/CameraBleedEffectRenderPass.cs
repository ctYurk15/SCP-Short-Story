using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CameraBleedEffect
{
    internal class CameraBleedEffectRenderPass : ScriptableRenderPass
    {
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler("ColorBlit");
        RTHandle m_CameraColorTarget;

        public CameraBleedEffectRenderPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetTarget(RTHandle colorHandle)
        {
            m_CameraColorTarget = colorHandle;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(m_CameraColorTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            if (cameraData.camera.cameraType == CameraType.Game)
            {
                //check, if camera is configured for bleed effect
                GameObject camera_game_object = renderingData.cameraData.camera.transform.gameObject;
                CustomBleedBehaviour custom_bleed_behaviour_component = camera_game_object.GetComponent<CustomBleedBehaviour>();
                if (custom_bleed_behaviour_component != null && custom_bleed_behaviour_component.enabled)
                {
                    //get shader material
                    Material material = custom_bleed_behaviour_component.getBleedMaterial();

                    if(material != null)
                    {
                        //apply material
                        CommandBuffer cmd = CommandBufferPool.Get();
                        using (new ProfilingScope(cmd, m_ProfilingSampler))
                        {
                            Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, material, 0);
                        }
                        context.ExecuteCommandBuffer(cmd);
                        cmd.Clear();
                    }

                    
                }
            }
        }
    }
}
