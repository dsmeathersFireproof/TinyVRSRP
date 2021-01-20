using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class VRRenderPipeline : RenderPipeline
{
    public VRRenderPipelineAsset Settings;

    private CullingResults CullResults;

    private CommandBuffer CommandBuffer = new CommandBuffer() { name = "Init" };

    public VRRenderPipeline(VRRenderPipelineAsset settings)
    {
        Settings = settings;
    }

    protected override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        foreach (Camera cam in cameras)
        {
            Render(renderContext, cam);
        }
    }

    public void Render(ScriptableRenderContext renderContext, Camera camera)
    {
#if UNITY_EDITOR
        if (camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
        }
#endif // UNITY_EDITOR

        renderContext.SetupCameraProperties(camera, camera.stereoEnabled);

        if (camera.TryGetCullingParameters(true, out ScriptableCullingParameters cullingParameters) == false)
        {
            return;
        }
        CullResults = renderContext.Cull(ref cullingParameters);

        CommandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, 0, CubemapFace.Unknown, -1);

        if (camera.stereoEnabled)
        {
            CommandBuffer.SetSinglePassStereo(SinglePassStereoMode.Instancing);
            renderContext.StartMultiEye(camera);
        }
        else
        {
            CommandBuffer.SetSinglePassStereo(SinglePassStereoMode.None);
        }

        CommandBuffer.ClearRenderTarget(true, true, camera.backgroundColor, 1.0f);
        renderContext.ExecuteCommandBuffer(CommandBuffer);
        CommandBuffer.Clear();

        SortingSettings sortingSettings = new SortingSettings(camera);
        sortingSettings.criteria = SortingCriteria.CommonOpaque;

        DrawingSettings drawSettings = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"), sortingSettings);
        drawSettings.enableDynamicBatching = false;
        drawSettings.enableInstancing = true;
        drawSettings.sortingSettings = sortingSettings;
        drawSettings.perObjectData = 0;
        FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.opaque);
        renderContext.DrawRenderers(CullResults, ref drawSettings, ref filterSettings);
        if (camera.stereoEnabled)
        {
            renderContext.StopMultiEye(camera);
        }

#if UNITY_EDITOR
        if (UnityEditor.Handles.ShouldRenderGizmos())
        {
            renderContext.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            renderContext.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
#endif // UNITY_EDITOR

        if (camera.stereoEnabled)
        {
            renderContext.StereoEndRender(camera);
        }
        renderContext.Submit();
    }
}