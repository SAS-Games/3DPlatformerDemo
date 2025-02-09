using SAS.StateMachineCharacterController;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraTargetSetter : MonoBehaviour, IObjectSpawnedListener
{
    private CinemachineCamera _currentCamera;
    public IMovementVectorHandler MovementVectorHandler { get; private set; }
    public ICameraLookAt CameraLookAt { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _currentCamera = GetComponent<CinemachineCamera>();
    }

    void IObjectSpawnedListener.OnSpawn(GameObject gameObject)
    {
        MovementVectorHandler = gameObject.GetComponent<IMovementVectorHandler>();
        CameraLookAt = gameObject.GetComponent<ICameraLookAt>();
        _currentCamera.Target.TrackingTarget = CameraLookAt.Target;
    }

    void IObjectSpawnedListener.OnDespawn(GameObject gameObject)
    {
    }
}
