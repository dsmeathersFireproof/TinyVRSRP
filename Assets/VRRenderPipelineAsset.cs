using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "VR Render Pipeline")]
public class VRRenderPipelineAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        VRRenderPipeline renderPipeline = new VRRenderPipeline(this);
        return renderPipeline;
    }
}