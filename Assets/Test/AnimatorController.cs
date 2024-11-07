using UnityEngine;
using SAS.Utilities.TagSystem;
using SAS.StateMachineCharacterController;

public class AnimatorController : MonoBehaviour
{
    [FieldRequiresSelf] private Animator m_Animator;
    [FieldRequiresParent] private FSMCharacterController m_FSMCharacterController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetFloat("Speed", m_FSMCharacterController.speed);
    }

    private void Reset()
    {
        this.Initialize();
    }
}
