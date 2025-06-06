using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using CameraBleedEffect;

public class CustomBleedRenderPassFeature : ScriptableRendererFeature
{
    class CustomBleedRenderPass : ScriptableRenderPass
    {
        private Material material;
        private RTHandle source;
        private RTHandle tempTexture;

        public CustomBleedRenderPass(Material material)
        {
            this.material = material;
        }

        public void Setup(RTHandle source)
        {
            this.source = source;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, cameraTextureDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempBleedTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Custom Bleed Effect");
            if (source != null)
            {
                Blitter.BlitCameraTexture(cmd, source, tempTexture, material, 0);
            }
            else
            {
                Debug.LogError("Bleed material is NULL when calling Blitter.BlitCameraTexture");
            }
            Blitter.BlitCameraTexture(cmd, tempTexture, source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            tempTexture?.Release();
        }
    }

    public Shader shader;
    private Material material;
    private CustomBleedRenderPass customBleedRenderPass;

    public override void Create()
    {
        if (shader == null)
        {
            Debug.LogError("Shader is missing in CustomBleedRenderPassFeature!");
            return;
        }

        material = CoreUtils.CreateEngineMaterial(shader);
        customBleedRenderPass = new CustomBleedRenderPass(material)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
        };
    }

    /*public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null)
            return;

        customBleedRenderPass.Setup(renderer.cameraColorTargetHandle);
        Debug.Log($"Passing source to Setup: {renderer.cameraColorTargetHandle}");
        renderer.EnqueuePass(customBleedRenderPass);
    }*/

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            GameObject camera_game_object = renderingData.cameraData.camera.transform.gameObject;
            CustomBleedBehaviour custom_bleed_behaviour_component = camera_game_object.GetComponent<CustomBleedBehaviour>();
            if (custom_bleed_behaviour_component != null && custom_bleed_behaviour_component.enabled)
            {
                Debug.Log(camera_game_object);

            }
        }
            //renderer.EnqueuePass(m_RenderPass);
    }
}