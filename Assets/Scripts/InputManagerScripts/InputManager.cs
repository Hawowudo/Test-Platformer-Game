using UnityEngine;
using UnityEngine.InputSystem;

namespace InputManagerScripts
{
    public enum InputActionMapType
    {
        Gameplay,
        UI
    }

    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager Instance { get; private set; }

        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private InputActionMapType activeActionMap;
        [SerializeField] private bool debugMode;

        public InputAction Move { get; private set; }
        public InputAction Attack { get; private set; }
        public InputAction Jump { get; private set; }

        private InputAction gameplayPause;
        private InputAction uiPause;

        private InputActionMap gameplayMap;
        private InputActionMap uiMap;


        private void Awake()
        {
            SetupSingleton();

            SetupActionMaps();
            SetupActions();

            ActivateActionMap(activeActionMap);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }


        private void Update()
        {
            if (debugMode && Time.frameCount % 60 == 0)
            {
                DebugDisplayCurrentActions();
            }
        }


        #region Setup

        private void SetupSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


        private void SetupActionMaps()
        {
            gameplayMap = inputActionAsset.FindActionMap("Gameplay");
            uiMap = inputActionAsset.FindActionMap("UI");

            if (gameplayMap == null)
                Debug.LogError("Gameplay action map not found.");

            if (uiMap == null)
                Debug.LogError("UI action map not found.");
        }


        private void SetupActions()
        {
            if (gameplayMap != null)
            {
                Move = gameplayMap.FindAction("Move");
                Attack = gameplayMap.FindAction("Attack");
                Jump = gameplayMap.FindAction("Jump");
                gameplayPause = gameplayMap.FindAction("PauseUnpause");
            }

            if (uiMap != null)
            {
                uiPause = uiMap.FindAction("PauseUnpause");
            }

            ValidateActions();
        }


        private void ValidateActions()
        {
            if (Move == null) Debug.LogError("Move action missing.");
            if (Attack == null) Debug.LogError("Attack action missing.");
            if (Jump == null) Debug.LogError("Jump action missing.");
            if (gameplayPause == null) Debug.LogError("Gameplay Pause action missing.");
            if (uiPause == null) Debug.LogError("UI Pause action missing.");
        }

        #endregion


        #region Action Map Switching

        public void SwitchToGameplayInputActionMap()
        {
            ActivateActionMap(InputActionMapType.Gameplay);
        }

        public void SwitchToUIInputActionMap()
        {
            ActivateActionMap(InputActionMapType.UI);
        }


        public void ActivateActionMap(InputActionMapType mapType)
        {
            DisableAllMaps();

            activeActionMap = mapType;

            switch (mapType)
            {
                case InputActionMapType.Gameplay:
                    gameplayMap?.Enable();
                    break;

                case InputActionMapType.UI:
                    uiMap?.Enable();
                    break;
            }
        }


        private void DisableAllMaps()
        {
            gameplayMap?.Disable();
            uiMap?.Disable();
        }

        #endregion


        #region Input Queries

        public bool MenuOpenClosePressed()
        {
            switch (activeActionMap)
            {
                case InputActionMapType.Gameplay:
                    return gameplayPause != null && gameplayPause.IsPressed();

                case InputActionMapType.UI:
                    return uiPause != null && uiPause.IsPressed();

                default:
                    return false;
            }
        }

        #endregion


        #region Debug

        private void DebugDisplayCurrentActions()
        {
            if (activeActionMap == InputActionMapType.Gameplay)
            {
                Debug.Log(
                    $"Move: {Move?.ReadValue<Vector2>()} " +
                    $"Attack: {Attack?.IsPressed()} " +
                    $"Jump: {Jump?.IsPressed()} " +
                    $"Pause: {MenuOpenClosePressed()}");
                return;
            }
            if (activeActionMap == InputActionMapType.UI)
            {
                Debug.Log($"UI Pause: {MenuOpenClosePressed()}");
            }

        }

        #endregion
    }
}