using UnityEngine;

public class KillerCollider : MonoBehaviour
{
    private void HandleOnTriggerEnter(Collider other)
    {
        Debug.Log($"Got hit by killer collider: {this.gameObject}");
        other.SendMessage("Respawn", SendMessageOptions.DontRequireReceiver);
    }
}
