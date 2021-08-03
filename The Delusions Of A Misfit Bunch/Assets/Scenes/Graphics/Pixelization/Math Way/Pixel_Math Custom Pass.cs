using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;



class Pixel_MathCustomPass : CustomPass
{
    [SerializeField]
    Material _pixelMat;
    [SerializeField]
    float _pixelizationStrength = 1.0f;
    [SerializeField]
    float _baseResolutionWidth = 1920.0f;
    [SerializeField]
    float _baseResolutionHeight = 1080.0f;



    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in an performance manner.
    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        // Setup code here
    }

    protected override void Execute(CustomPassContext ctx)
    {
        // Executed every frame for all the camera inside the pass volume.
        // The context contains the command buffer to use to enqueue graphics commands.

        //Vector2 ratios = Vector2.one;

        //if (Screen.currentResolution.width != 1920.0f)
        //{
        //    ratios.x = 1920.0f / Screen.currentResolution.width;
        //}

        //if (Screen.currentResolution.height != 1080.0f)
        //{
        //    ratios.y = 1080.0f / Screen.currentResolution.height;
        //}

        //ratios.x = 1920.0f / Screen.currentResolution.width;
        //ratios.y = 1080.0f / Screen.currentResolution.height;

        ctx.propertyBlock.SetFloat("pixelizationStrength", _pixelizationStrength);
        //ctx.propertyBlock.SetFloat("widthRatio", ratios.x);
        //ctx.propertyBlock.SetFloat("heightRatio", ratios.y);
        ctx.propertyBlock.SetFloat("baseResolutionWidth", _baseResolutionWidth);
        ctx.propertyBlock.SetFloat("baseResolutionHeight", _baseResolutionHeight);

        CoreUtils.SetRenderTarget(ctx.cmd, ctx.cameraColorBuffer, ClearFlag.None);
        CoreUtils.DrawFullScreen(ctx.cmd, _pixelMat, ctx.propertyBlock);
    }

    protected override void Cleanup()
    {
        // Cleanup code
    }
}