using SAS.StateMachineGraph;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private string m_State = "Dizzy";
    private void OnSpikeHit(Collider other)
    {
        if (other.GetComponentInParent<Actor>() is Actor actor)
        {
            actor.SetState(m_State);
            Debug.Log($"{actor.gameObject.name} is got hit by the spike {this.gameObject.name}");
        }
    }
}
