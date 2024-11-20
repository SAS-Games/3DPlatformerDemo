using SAS.StateMachineCharacterController;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private Portal m_LinkedPortal;
    [SerializeField] private MeshRenderer m_Screen;
    [SerializeField] private int m_RecursionLimit = 5;

    [Header("Advanced Settings")]
    [SerializeField] private float m_NearClipOffset = 0.05f;
    [SerializeField] private float m_NearClipLimit = 0.2f;

    // Private variables
    private RenderTexture _viewTexture;
    private Camera _portalCam;
    private Camera _playerCam;
    private Material _firstRecursionMat;
    private List<PortalTraveller> _trackedTravellers;
    private MeshFilter _screenMeshFilter;

    void Awake()
    {
        _playerCam = Camera.main;// GameObject.FindAnyObjectByType<FSMCharacterController>().GetComponentInChildren<Camera>(true);
        _portalCam = GetComponentInChildren<Camera>();
        _portalCam.enabled = false;
        _trackedTravellers = new List<PortalTraveller>();
        _screenMeshFilter = m_Screen.GetComponent<MeshFilter>();
        m_Screen.material.SetInt("_DisplayMask", 1);
    }

    void LateUpdate() => HandleTravellers();

    void HandleTravellers()
    {
        for (int i = 0; i < _trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = _trackedTravellers[i];
            Transform travellerT = traveller.transform;
            var m = m_LinkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));
            // Teleport the traveller if it has crossed from one side of the portal to the other
            if (portalSide != portalSideOld)
            {
                var positionOld = travellerT.position;
                var rotOld = travellerT.rotation;
                traveller.Teleport(transform, m_LinkedPortal.transform, m.GetColumn(3), m.rotation);
                traveller.GraphicsClone.transform.SetPositionAndRotation(positionOld, rotOld);
                // Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
                m_LinkedPortal.OnTravellerEnterPortal(traveller);
                _trackedTravellers.RemoveAt(i);
                i--;

            }
            else
            {
                traveller.GraphicsClone.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    // Called before any portal cameras are rendered for the current frame
    public void PrePortalRender()
    {
        foreach (var traveller in _trackedTravellers)
            UpdateSliceParams(traveller);
    }

    // Manually render the camera attached to this portal
    // Called after PrePortalRender, and before PostPortalRender
    public void Render()
    {
        // Skip rendering the view from this portal if player is not looking at the linked portal
        if (!CameraUtility.VisibleFromCamera(m_LinkedPortal.m_Screen, _playerCam))
            return;

        CreateViewTexture();

        var localToWorldMatrix = _playerCam.transform.localToWorldMatrix;// GameObject.FindAnyObjectByType<FSMCharacterController>().transform.localToWorldMatrix; //
        var renderPositions = new Vector3[m_RecursionLimit];
        var renderRotations = new Quaternion[m_RecursionLimit];

        int startIndex = 0;
        _portalCam.projectionMatrix = _playerCam.projectionMatrix;
        for (int i = 0; i < m_RecursionLimit; i++)
        {
            if (i > 0)
            {
                // No need for recursive rendering if linked portal is not visible through this portal
                if (!CameraUtility.BoundsOverlap(_screenMeshFilter, m_LinkedPortal._screenMeshFilter, _portalCam))
                    break;
            }
            localToWorldMatrix = transform.localToWorldMatrix * m_LinkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
            int renderOrderIndex = m_RecursionLimit - i - 1;
            renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
            renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

            _portalCam.transform.SetPositionAndRotation(renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
            startIndex = renderOrderIndex;
        }

        // Hide screen so that camera can see through portal
        m_Screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        m_LinkedPortal.m_Screen.material.SetInt("_DisplayMask", 0);

        for (int i = startIndex; i < m_RecursionLimit; i++)
        {
            _portalCam.transform.SetPositionAndRotation(renderPositions[i], renderRotations[i]);
            SetNearClipPlane();
            HandleClipping();
            _portalCam.Render();

            if (i == startIndex)
                m_LinkedPortal.m_Screen.material.SetInt("_DisplayMask", 1);
        }

        // Unhide objects hidden at start of render
        m_Screen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    void HandleClipping()
    {
        // There are two main graphical issues when slicing travellers
        // 1. Tiny sliver of mesh drawn on backside of portal
        //    Ideally the oblique clip plane would sort this out, but even with 0 offset, tiny sliver still visible
        // 2. Tiny seam between the sliced mesh, and the rest of the model drawn onto the portal screen
        // This function tries to address these issues by modifying the slice parameters when rendering the view from the portal
        // Would be great if this could be fixed more elegantly, but this is the best I can figure out for now
        const float hideDst = -1000;
        const float showDst = 1000;
        float screenThickness = m_LinkedPortal.ProtectScreenFromClipping(_portalCam.transform.position);

        foreach (var traveller in _trackedTravellers)
        {
            // Addresses issue 1
            if (SameSideOfPortal(traveller.transform.position, portalCamPos))
                traveller.SetSliceOffsetDst(hideDst, false);
            else // Addresses issue 2
                traveller.SetSliceOffsetDst(showDst, false);

            // Ensure clone is properly sliced, in case it's visible through this portal:
            int cloneSideOfLinkedPortal = -SideOfPortal(traveller.transform.position);
            bool camSameSideAsClone = m_LinkedPortal.SideOfPortal(portalCamPos) == cloneSideOfLinkedPortal;
            if (camSameSideAsClone)
                traveller.SetSliceOffsetDst(screenThickness, true);
            else
                traveller.SetSliceOffsetDst(-screenThickness, true);
        }

        var offsetFromPortalToCam = portalCamPos - transform.position;
        foreach (var linkedTraveller in m_LinkedPortal._trackedTravellers)
        {
            var travellerPos = linkedTraveller.GraphicsObject.transform.position;
            var clonePos = linkedTraveller.GraphicsClone.transform.position;
            // Handle clone of linked portal coming through this portal:
            bool cloneOnSameSideAsCam = m_LinkedPortal.SideOfPortal(travellerPos) != SideOfPortal(portalCamPos);
            if (cloneOnSameSideAsCam) // Addresses issue 1
                linkedTraveller.SetSliceOffsetDst(hideDst, true);
            else  // Addresses issue 2
                linkedTraveller.SetSliceOffsetDst(showDst, true);

            // Ensure traveller of linked portal is properly sliced, in case it's visible through this portal:
            bool camSameSideAsTraveller = m_LinkedPortal.SameSideOfPortal(linkedTraveller.transform.position, portalCamPos);
            if (camSameSideAsTraveller)
                linkedTraveller.SetSliceOffsetDst(screenThickness, false);
            else
                linkedTraveller.SetSliceOffsetDst(-screenThickness, false);
        }
    }

    // Called once all portals have been rendered, but before the player camera renders
    public void PostPortalRender()
    {
        foreach (var traveller in _trackedTravellers)
            UpdateSliceParams(traveller);
        ProtectScreenFromClipping(_playerCam.transform.position);
    }
    void CreateViewTexture()
    {
        if (_viewTexture == null || _viewTexture.width != Screen.width || _viewTexture.height != Screen.height)
        {
            if (_viewTexture != null)
                _viewTexture.Release();

            _viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            // Render the view from the portal camera to the view texture
            _portalCam.targetTexture = _viewTexture;
            // Display the view texture on the screen of the linked portal
            m_LinkedPortal.m_Screen.material.SetTexture("_MainTex", _viewTexture);
        }
    }

    // Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
    float ProtectScreenFromClipping(Vector3 viewPoint)
    {
        float halfHeight = _playerCam.nearClipPlane * Mathf.Tan(_playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * _playerCam.aspect;
        float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, _playerCam.nearClipPlane).magnitude;
        float screenThickness = dstToNearClipPlaneCorner;

        Transform screenT = m_Screen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - viewPoint) > 0;
        screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
        return screenThickness;
    }

    void UpdateSliceParams(PortalTraveller traveller)
    {
        // Calculate slice normal
        int side = SideOfPortal(traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = m_LinkedPortal.transform.forward * side;

        // Calculate slice centre
        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = m_LinkedPortal.transform.position;

        // Adjust slice offset so that when player standing on other side of portal to the object, the slice doesn't clip through
        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;
        float screenThickness = m_Screen.transform.localScale.z;

        bool playerSameSideAsTraveller = SameSideOfPortal(_playerCam.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller)
            sliceOffsetDst = -screenThickness;
        bool playerSameSideAsCloneAppearing = side != m_LinkedPortal.SideOfPortal(_playerCam.transform.position);
        if (!playerSameSideAsCloneAppearing)
            cloneSliceOffsetDst = -screenThickness;

        // Apply parameters
        for (int i = 0; i < traveller.OriginalMaterials.Length; i++)
        {
            traveller.OriginalMaterials[i].SetVector("sliceCentre", slicePos);
            traveller.OriginalMaterials[i].SetVector("sliceNormal", sliceNormal);
            traveller.OriginalMaterials[i].SetFloat("sliceOffsetDst", sliceOffsetDst);

            traveller.CloneMaterials[i].SetVector("sliceCentre", cloneSlicePos);
            traveller.CloneMaterials[i].SetVector("sliceNormal", cloneSliceNormal);
            traveller.CloneMaterials[i].SetFloat("sliceOffsetDst", cloneSliceOffsetDst);
        }
    }

    // Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
    // Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
    void SetNearClipPlane()
    {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - _portalCam.transform.position));

        Vector3 camSpacePos = _portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = _portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + m_NearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > m_NearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            _portalCam.projectionMatrix = _playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
            _portalCam.projectionMatrix = _playerCam.projectionMatrix;
    }

    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!_trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            _trackedTravellers.Add(traveller);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller)
            OnTravellerEnterPortal(traveller);
    }

    void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && _trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            _trackedTravellers.Remove(traveller);
        }
    }

    /*
     ** Some helper/convenience stuff:
     */

    int SideOfPortal(Vector3 pos)
    {
        return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal(Vector3 posA, Vector3 posB)
    {
        return SideOfPortal(posA) == SideOfPortal(posB);
    }

    Vector3 portalCamPos => _portalCam.transform.position;

    void OnValidate()
    {
        if (m_LinkedPortal != null)
            m_LinkedPortal.m_LinkedPortal = this;
    }
}