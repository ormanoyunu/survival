using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalTemplatePro.InputSystem
{
    public class PlayerMovementInput : CharacterBehaviour
    {
        [SerializeField]
        private bool m_EnableOnStart = true;

        [SerializeField]
        private InputActionReference m_MoveInput;

        [SerializeField]
        private InputActionReference m_RunInput;

        [SerializeField]
        private InputActionReference m_CrouchInput;

        [SerializeField]
        private InputActionReference m_JumpInput;

        private CharMotionMask m_CurrentMotionMask;
        private ICharacterMover m_CharacterMover;


        public override void OnInitialized()
        {
            GetModule(out m_CharacterMover);

            if (m_EnableOnStart)
            {
                m_MoveInput.action.Enable();
                m_RunInput.action.Enable();
                m_CrouchInput.action.Enable();
                m_JumpInput.action.Enable();
            }
        }

        private void Update()
        {
            if (!IsInitialized)
                return;

            m_CurrentMotionMask = 0;

            if (m_RunInput.action.ReadValue<float>() > 0.1f)
                m_CurrentMotionMask = m_CurrentMotionMask.SetFlag(CharMotionMask.Run);

            if (m_CrouchInput.action.triggered)
                m_CurrentMotionMask = m_CurrentMotionMask.SetFlag(CharMotionMask.Crouch);

            if (m_JumpInput.action.triggered)
                m_CurrentMotionMask = m_CurrentMotionMask.SetFlag(CharMotionMask.Jump);

            Vector2 moveInput = m_MoveInput.action.ReadValue<Vector2>();
            m_CharacterMover.Move(Character.transform.TransformVector(new Vector3(moveInput.x, 0f, moveInput.y)), m_CurrentMotionMask);
        }
    }
}