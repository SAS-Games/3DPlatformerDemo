using UnityEngine;

public class IdleStateMachineBehaviour : StateMachineBehaviour
{
    private float timeSinceLastChange;
    public float changeInterval = 5f; // Time interval to switch between Idle A and Idle B
    private Animator animator;

    // Called when entering a state inside the Idle sub-state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animator = animator;

        // If we're entering from the Run state, set Idle_C first
        if (stateInfo.IsName("Idle_C"))
        {
            animator.SetInteger("IdleIndex", 1); // Move to Idle_B next after Idle_C
        }
    }

    // Called each frame while inside any state in the sub-state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Handle random transitions between Idle_A and Idle_B
        if (stateInfo.IsName("Idle_A") || stateInfo.IsName("Idle_B"))
        {
            timeSinceLastChange += Time.deltaTime;

            if (timeSinceLastChange > changeInterval)
            {
                timeSinceLastChange = 0f;
                int randomIndex = Random.Range(0, 2); // Randomly pick 0 or 1
                animator.SetInteger("IdleIndex", randomIndex);
            }
        }
    }
}
