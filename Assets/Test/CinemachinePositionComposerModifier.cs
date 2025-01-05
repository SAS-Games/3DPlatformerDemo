using SAS.SceneManagement;
using SAS.StateMachineCharacterController;
using Unity.Cinemachine;
using UnityEngine;
using static Unity.Cinemachine.ICinemachineCamera;

[RequireComponent(typeof(CinemachinePositionComposer))]
public class CinemachinePositionComposerModifier : MonoBehaviour, IObjectSpawnedListener
{
    [SerializeField] private float m_FallYDampAmount = 0.1f;
    [SerializeField] private bool m_UseScreenPositionY = true;
    [SerializeField] private float m_FallYScreenPosition = -0.25f;
    [SerializeField] private float m_FallSpeedThreshold = -15;
    [SerializeField] private float m_TargetSpeedReachMultiplier = 20f;

    private CinemachinePositionComposer _positionComposer;
    private ICinemachineCamera _currentCamera;
    private IMovementVectorHandler _target;
    private float _defaultYDamping;
    private float _defaultYScreenPosition;
    private float _targetYDamping;
    private float _targetYScreenPosition;

    private void OnCameraActivated(ActivationEventParams @params)
    {
        enabled = _currentCamera == @params.IncomingCamera;
    }

    private void Awake()
    {
        _currentCamera = GetComponent<CinemachineCamera>();
        _positionComposer = GetComponent<CinemachinePositionComposer>();
        if (_positionComposer != null)
        {
            _defaultYDamping = _positionComposer.Damping.y;
            _defaultYScreenPosition = _positionComposer.Composition.ScreenPosition.y;
        }
        CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
    }

    private void LateUpdate()
    {
        if (_positionComposer == null || _target == null) return;

        if (_target.MovementVector.y < m_FallSpeedThreshold)
        {
            _targetYDamping = m_FallYDampAmount;
            _targetYScreenPosition = m_FallYScreenPosition;
        }
        else
        {
            _targetYDamping = _defaultYDamping;
            _targetYScreenPosition = _defaultYScreenPosition;
        }

        var speed = Time.deltaTime * m_TargetSpeedReachMultiplier;
        _positionComposer.Damping.y = Mathf.Lerp(_positionComposer.Damping.y, _targetYDamping, speed);
        if (m_UseScreenPositionY)
            _positionComposer.Composition.ScreenPosition.y = Mathf.Lerp(_positionComposer.Composition.ScreenPosition.y, _targetYScreenPosition, speed);
    }

    void IObjectSpawnedListener.OnSpawn(GameObject gameObject)
    {
        (_currentCamera as CinemachineCamera).Target.TrackingTarget = gameObject.transform;
        _target = gameObject.GetComponent<IMovementVectorHandler>();

    }

    void IObjectSpawnedListener.OnDespawn(GameObject gameObject)
    {
        throw new System.NotImplementedException();
    }
}
