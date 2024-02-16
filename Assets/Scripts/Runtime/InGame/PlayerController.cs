using InGame.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InGame
{
    public class PlayerController : MoveBase
    {
        /// <summary>
        /// このやり方あんまりよくなさそう。Actionsをどこに置くかは今後気を付けないと…
        /// そもそもPlayerとUIはActionsから切り分けるべきなのか？
        /// </summary>
        public static AccelActions Actions;
        
        private InputAction _move;
        private InputAction _look;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            //InputSystemの初期化
            Actions = new();
            Actions.Player.Enable();
            
            // InputSystemの設定
            _move = Actions.Player.Move;
            _look = Actions.Player.Look;
            
            Actions.Player.Intaract.performed += OnIntaract;
            Actions.Player.Sprint.started += OnSprint;
            Actions.Player.Sprint.canceled += OnSprint;
        }

        private void OnDisable()
        {
            //InputSystemを無効化
            Actions.Player.Disable();
        }

        protected override void Update()
        {
            base.Update();
            
            #if PLATFORM_STANDALONE
            InputMove();
            InputLook();      
            #endif
        }

        private void InputMove()
        {
            //InputSystemから値を取って、Moveに移動
            var inputVector = _move.ReadValue<Vector2>();
            Move(inputVector);
        }

        private void InputLook()
        {
            #if PLATFORM_STANDALONE
            var inputVector = _look.ReadValue<Vector2>();
            Look(inputVector);
            #endif
        }

        private void OnIntaract(InputAction.CallbackContext _)
        {
            Interact();
        }

        private void OnTriggerEnter(Collider other)
        {
            //　当たり判定の親にオブジェクトがあるので、親のコンポーネントを見る。
            if (other.transform.parent is not null)
            {
                if (other.transform.parent.TryGetComponent<NPC>(out var component))
                {
                    component.OnCloseToPlayer();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //　当たり判定の親にオブジェクトがあるので、親のコンポーネントを見る。
            if (other.transform.parent is not null)
            {
                if (other.transform.parent.TryGetComponent<NPC>(out var component))
                {
                    component.OnLeavePlayer();
                }
            }
        }

        private void OnSprint(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                IsSprint = true;                
            }
            else if (ctx.canceled)
            {
                IsSprint = false;
            }
        }
    }   
}
