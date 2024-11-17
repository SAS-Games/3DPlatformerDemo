using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OneWayBoxCollider : MonoBehaviour
{
    [Tooltip("The direction that the other object should be coming from for entry.")]
    [SerializeField] private Vector3 m_EntryDirection = Vector3.up;
    [Tooltip("Should the entry direction be used as a local direction?")]
    [SerializeField] private bool m_LocalDirection = false;
    [Tooltip("How large should the trigger be in comparison to the original collider?")]
    [SerializeField] private Vector3 m_TriggerScale = Vector3.one * 1.25f;
    [Tooltip("The collision will activate only when the penetration depth of the intruder is smaller than this threshold.")]
    [SerializeField] private float m_PenetrationDepthThreshold = 0.2f;

    [Tooltip("The original collider. Will always be present thanks to the RequireComponent attribute.")]
    private BoxCollider _collider = null;
    [Tooltip("The trigger that we add ourselves once the game starts up.")]
    private BoxCollider _collisionCheckTrigger = null;

    /// <summary> The entry direction, calculated accordingly based on whether it is a local direction or not. 
    /// This is pretty much what I've done in the video when copy-pasting the local direction check, but written as a property. </summary>
    public Vector3 PassthroughDirection => m_LocalDirection ? transform.TransformDirection(m_EntryDirection.normalized) : m_EntryDirection.normalized;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();

        // Adding the BoxCollider and making sure that its sizes match the ones
        // of the OG collider.
        _collisionCheckTrigger = gameObject.AddComponent<BoxCollider>();
        _collisionCheckTrigger.size = new Vector3(
            _collider.size.x * m_TriggerScale.x,
            _collider.size.y * m_TriggerScale.y,
            _collider.size.z * m_TriggerScale.z
        );
        _collisionCheckTrigger.center = _collider.center;
        _collisionCheckTrigger.isTrigger = true;
        _collisionCheckTrigger.hideFlags = HideFlags.HideAndDontSave;
    }

    private void OnValidate()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = false;
    }

    private void OnTriggerStay(Collider other)
    {
        TryIgnoreCollision(other);
    }

    public void TryIgnoreCollision(Collider other)
    {
        // Simulate a collision between our trigger and the intruder to check
        // the direction that the latter is coming from. The method returns true
        // if any collision has been detected.
        if (Physics.ComputePenetration(
            _collisionCheckTrigger, _collisionCheckTrigger.bounds.center, transform.rotation,
            other, other.bounds.center, other.transform.rotation,
            out Vector3 collisionDirection, out float penetrationDepth))
        {
            float dot = Vector3.Dot(PassthroughDirection, collisionDirection);
            if (dot < 0)
            {
                // Activate collison only once the intruder is close enough to the trigger border, to avoid teleportation
                if (penetrationDepth < m_PenetrationDepthThreshold)
                    Physics.IgnoreCollision(_collider, other, false);
                // Making sure that the two object are NOT ignoring each other.
            }
            else
                Physics.IgnoreCollision(_collider, other, true);
            // Making the colliders ignore each other, and thus allow passage from one way.
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.TransformPoint(_collider.center), PassthroughDirection * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.TransformPoint(_collider.center), -PassthroughDirection * 2);
    }
}