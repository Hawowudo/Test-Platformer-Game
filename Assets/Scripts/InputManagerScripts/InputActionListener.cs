using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace InputManagerScripts
{
    public class InputActionListener : MonoBehaviour
    {
        [Tooltip("Optional")]
        public Blackboard _blackboard;
        public UnityEvent<Vector2> OnMovePerform;
        public UnityEvent OnAttackPerform;
        public UnityEvent OnAttackCancel;
        public UnityEvent OnJumpPress;
        public UnityEvent OnJumpCancel;
        private void Start()
        {
            PlayerInputManager.Instance.Move.performed += HandleMove;
            PlayerInputManager.Instance.Move.canceled += HandleMove;
            PlayerInputManager.Instance.Attack.performed += HandleAttack;
            PlayerInputManager.Instance.Attack.canceled += HandleAttack;
            PlayerInputManager.Instance.Jump.performed += HandleJump;
            PlayerInputManager.Instance.Jump.canceled += HandleJump;

            _blackboard?.Set("previousmoveinput", Vector2.right);
        }
        private void OnDestroy()
        {
            if (PlayerInputManager.Instance == null) return;
            if (PlayerInputManager.Instance.Move == null) return;

            PlayerInputManager.Instance.Move.performed -= HandleMove;
            PlayerInputManager.Instance.Move.canceled -= HandleMove;
            PlayerInputManager.Instance.Attack.performed -= HandleAttack;
            PlayerInputManager.Instance.Attack.canceled -= HandleAttack;
            PlayerInputManager.Instance.Jump.performed -= HandleJump;
            PlayerInputManager.Instance.Jump.canceled -= HandleJump;
        }
        #region Action handlers
        private void HandleMove(InputAction.CallbackContext ctx)
        {
            Vector2 move = ctx.ReadValue<Vector2>();
            OnMove(move);
        }
        public void OnMove(Vector2 vector)
        {
            Debug.Log($"Move: {vector}");
            OnMovePerform?.Invoke(vector);
            if(_blackboard?.Get<Vector2>("moveinput") != Vector2.zero )
                _blackboard?.Set("previousmoveinput", _blackboard?.Get<Vector2>("moveinput") );
            _blackboard?.Set("moveinput", vector);
        }

        private void HandleAttack(InputAction.CallbackContext ctx)
        {
            bool attackpressed = ctx.ReadValueAsButton();
            if (attackpressed)
            {
                OnAttackPerform?.Invoke();
            }
            else
            {
                OnAttackCancel?.Invoke();
            }

            _blackboard?.Set("attackpressed", attackpressed);
        }

        private void HandleJump(InputAction.CallbackContext ctx)
        {
            bool jumpressed = ctx.ReadValueAsButton();
            if (jumpressed)
            {
                OnJumpPress?.Invoke();
            }
            else
            {
                OnJumpCancel?.Invoke();
            }
            _blackboard?.Set("jumppressed", jumpressed);
        }


        #endregion



    }
}