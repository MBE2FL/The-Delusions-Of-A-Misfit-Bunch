using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;



class LowRenderCustomPass : CustomPass
{
    //RTHandle _lowResBuffer;
    [SerializeField]
    RenderTexture _renderTexture;
    [SerializeField]
    Vector2Int _resolution;
    [SerializeField]
    Material _fullscreenMat;
    [SerializeField]
    Camera _camera;



    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in an performance manner.
    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        // Setup code here

        //_lowResBuffer = RTHandles.Alloc(new Vector2(0.5f, 0.5f), TextureXR.slices, dimension: TextureXR.dimension, colorFormat: GraphicsFormat.B10G11R11_UFloatPack32,
        //    name: "Low Res Buffer");

        //_renderTexture = RenderTexture.GetTemporary(_resolution.x, _resolution.y, 0, GraphicsFormat.R32G32B32A32_SFloat);
        //_renderTexture.filterMode = FilterMode.Point;
    }

    protected override void Execute(CustomPassContext ctx)
    {
        // Executed every frame for all the camera inside the pass volume.
        // The context contains the command buffer to use to enqueue graphics commands.


        //if (ctx.hdCamera.camera == _camera)
        //{
        //    return;
        //}

        if (ctx.hdCamera.camera.tag == "Bullet")
        {
            return;
        }

        ctx.propertyBlock.SetTexture("lowRes", _renderTexture);
        CoreUtils.SetRenderTarget(ctx.cmd, ctx.cameraColorBuffer, ClearFlag.Color);
        CoreUtils.DrawFullScreen(ctx.cmd, _fullscreenMat, ctx.propertyBlock);
    }

    protected override void Cleanup()
    {
        // Cleanup code

        //RenderTexture.ReleaseTemporary(_renderTexture);
    }
}