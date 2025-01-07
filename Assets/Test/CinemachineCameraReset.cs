using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineCameraReset : MonoBehaviour
{
    private CinemachineCamera _cinemachineCamera;
    private Vector2 _originalDamping;
    private EventBinding<RespawnEvent> _targetRespawnEventBinding;

    private void OnEnable()
    {
        _targetRespawnEventBinding = new EventBinding<RespawnEvent>(RespawnPlayer);
        EventBus<RespawnEvent>.Register(_targetRespawnEventBinding);
    }

    void Start()
    {
        _cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        var composer = _cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
        if (composer != null)
            _originalDamping = composer.Damping;
    }

    private void RespawnPlayer(RespawnEvent respawnEventData)
    {
        Transform target = respawnEventData.transform;
        if (target != null)
        {
            if (_cinemachineCamera != null)
            {
                // Temporarily disable damping
                DisableDamping();

                // Snap the camera to the player's position
                _cinemachineCamera.OnTargetObjectWarped(target, target.position - _cinemachineCamera.transform.position);

                // Restore original damping
                RestoreDamping();
            }
        }
    }

    private void DisableDamping()
    {
        var composer = _cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
        if (composer != null)
            composer.Damping = Vector3.zero;
    }

    private void RestoreDamping()
    {
        var composer = _cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;

        if (composer != null)
            composer.Damping = _originalDamping;
    }

    private void OnDisable()
    {
        _targetRespawnEventBinding = new EventBinding<RespawnEvent>(RespawnPlayer);
        EventBus<RespawnEvent>.Deregister(_targetRespawnEventBinding);
    }
}

