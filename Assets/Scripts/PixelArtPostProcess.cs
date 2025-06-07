using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelArtPostProcess : ScriptableRendererFeature
{
    class PixelArtPass : ScriptableRenderPass
    {
        RenderTargetIdentifier source;
        RenderTargetHandle tempTex;
        Material mat;

        public PixelArtPass(Material material)
        {
            mat = material;
            tempTex.Init("_TempTex");
        }

        public void Setup(RenderTargetIdentifier src)
        {
            source = src;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("PixelArtPass");
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempTex.id, desc);
            Blit(cmd, source, tempTex.Identifier(), mat);
            Blit(cmd, tempTex.Identifier(), source);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [SerializeField] Shader shader;
    PixelArtPass pass;
    Material mat;

    public override void Create()
    {
        if (shader == null)
            Debug.LogError("Assign the PixelArtEffect shader in the Feature settings.");
        else
            mat = CoreUtils.CreateEngineMaterial(shader);

        pass = new PixelArtPass(mat)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (mat != null)
        {
            pass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(pass);
        }
    }
}
