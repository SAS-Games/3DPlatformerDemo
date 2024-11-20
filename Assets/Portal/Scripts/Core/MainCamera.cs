using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class MainCamera : MonoBehaviour
{

    private Portal[] _portals;

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;

    }

    void Awake()
    {
        _portals = FindObjectsByType<Portal>(FindObjectsSortMode.InstanceID);
    }

    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (camera != Camera.main)
            return;
        for (int i = 0; i < _portals.Length; i++)
            _portals[i].PrePortalRender();

        for (int i = 0; i < _portals.Length; i++)
            _portals[i].Render();

        for (int i = 0; i < _portals.Length; i++)
            _portals[i].PostPortalRender();
    }

}