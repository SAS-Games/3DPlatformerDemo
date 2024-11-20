using SAS.StateMachineCharacterController;
using System.Collections.Generic;
using UnityEngine;

public class SpeedModifier : MonoBehaviour
{
    [SerializeField] private float m_VerticalModifier = 1;
    [SerializeField] private float m_HorizontalModifier = 0.5f;
    private float _cachedSpeedMultiplier;
    private List<IMovementVectorHandler> _movementVectorHandlers = new List<IMovementVectorHandler>();

    private void Update()
    {
        foreach (var movementVectorHandler in _movementVectorHandlers)
        {
            Vector3 movementVector = movementVectorHandler.MovementVector;
            movementVector.x *= m_HorizontalModifier;
            movementVector.y *= m_VerticalModifier;
            movementVector.z *= m_HorizontalModifier;
            movementVectorHandler.MovementVector = movementVector;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var movementVectorHandler = other.GetComponent<IMovementVectorHandler>();
        if (movementVectorHandler != null && !_movementVectorHandlers.Contains(movementVectorHandler))
            _movementVectorHandlers.Add(movementVectorHandler);
    }

    private void OnTriggerExit(Collider other)
    {
        var movementVectorHandler = other.GetComponent<IMovementVectorHandler>();
        if (movementVectorHandler != null)
            _movementVectorHandlers.Remove(movementVectorHandler);

    }
}

