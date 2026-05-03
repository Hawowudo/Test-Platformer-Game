using CombatSystem;
using InputManagerScripts;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace GameManagerScripts
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager Instance;
        public static GameManager Get()
        {
            return Instance;
        }

        public GameObject playerPrefab;
        public float _menuOpenCloseInputBufferTime = 0.1f;
        public bool _quickPlay;

        private GameState m_CurrentGameState;
        private GameObject _playerInstance;
        private float _menuOpenCloseInputBufferCooldown = 0f;

        #region Unity Functions
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            SwitchGameState(GameState.MainMenu);
            if (_quickPlay)
            {
                StartGame();
            }
        }
        private void Update()
        {
            _menuOpenCloseInputBufferCooldown += Time.unscaledDeltaTime;

            if (PlayerInputManager.Instance.MenuOpenClosePressed() && _menuOpenCloseInputBufferCooldown > _menuOpenCloseInputBufferTime)
            {
                _menuOpenCloseInputBufferCooldown = 0f;
                TogglePauseScreen();
            }
        }
        #endregion
        public void CheckForPlayerSpawner()
        {
            PlayerSpawner.OnPlayerSpawned
                .Take(1)
                .Subscribe(spawner =>
                {
                    spawner.SpawnPlayer();
                    spawner.LinkCinemachineCamera();

                    Cinemachine.CinemachineVirtualCamera virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
                    virtualCamera.GetComponentInChildren<CameraFadeHandler>().StartFadeOut(() =>
                    {
                        SwitchGameState(GameState.Playing);

                        _playerInstance.GetComponent<CombatEntity>().currentHealth
                        .Subscribe(hp => {
                            if (FindAnyObjectByType<HealthBarScript>() == null) 
                                return;
                            FindAnyObjectByType<HealthBarScript>().UpdateHealthBar(hp, _playerInstance.GetComponent<CombatEntity>().maxHealth.Value); })
                        .AddTo(this);
                    });
                })
                .AddTo(this);
        }
        public void SwitchGameState(GameState state)
        {
            if(m_CurrentGameState == state)
            {
                return;
            }
            m_CurrentGameState = state;
            switch (m_CurrentGameState)
            {
                case GameState.Playing:
                    ResumeGame();
                    PlayerInputManager.Instance.ActivateActionMap(InputActionMapType.Gameplay);
                    ScreenManager.Instance.ShowScreen(ScreenManager.ScreenType.Gameplay);
                    break;
                case GameState.GameOver:
                    PauseGame();
                    PlayerInputManager.Instance.ActivateActionMap(InputActionMapType.UI);
                    ScreenManager.Instance.ShowScreen(ScreenManager.ScreenType.GameOver);
                    break;
                case GameState.MainMenu:
                    PauseGame();
                    PlayerInputManager.Instance.ActivateActionMap(InputActionMapType.UI);
                    ScreenManager.Instance.ShowScreen(ScreenManager.ScreenType.MainMenu);
                    break;
                case GameState.PauseScreen:
                    PauseGame();
                    PlayerInputManager.Instance.ActivateActionMap(InputActionMapType.UI);
                    ScreenManager.Instance.ShowScreen(ScreenManager.ScreenType.PauseScreen);
                    break;
            }
        }
        #region Saves
        public void LoadNewScene(string sceneName)
        {
            //StartCoroutine(LoadScene(sceneName));
            SceneManager.LoadScene(sceneName);
        }
        IEnumerator LoadScene(string sceneName)
        {
            string activeSceneName = SceneManager.GetActiveScene().name;
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(
            sceneName,
            LoadSceneMode.Additive
            );
            loadScene.allowSceneActivation = false;
            
            while (!loadScene.isDone)
            {
                if (loadScene.progress >= 0.9f)
                {
                    break;
                }
                yield return null;
            }
            loadScene.allowSceneActivation = true;
            while (!loadScene.isDone)
            {
                yield return null;
            }

            AsyncOperation unloadScene = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(activeSceneName));
            while (!unloadScene.isDone)
            {
                yield return null;
            }
        }
        #endregion

        #region Pause
        public void TogglePauseScreen()
        {
            if (m_CurrentGameState == GameState.PauseScreen)
            {
                SwitchGameState(GameState.Playing);
            }
            else if (m_CurrentGameState == GameState.Playing)
            {
                SwitchGameState(GameState.PauseScreen);
            }
        }
        public void PauseGame()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
        }
        public void PauseGameDuration(float duration = 0.2f, float delay = 0.1f)
        {
            // If the game is already paused, we don't want to start another pause coroutine
            StartCoroutine(PauseGameForDuration(duration, delay));;
        }
        private IEnumerator PauseGameForDuration(float duration, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            PauseGame();
            yield return new WaitForSecondsRealtime(duration);
            if (m_CurrentGameState != GameState.PauseScreen)
            {
                ResumeGame();
            }
        }
        #endregion

        public GameObject GetPlayer()
        {
            return _playerInstance;
        }
        public void StartGame()
        {
            if(_playerInstance != null)
            Destroy(_playerInstance);

            StartPlayerSetup();
            ResumeGame();

            CheckForPlayerSpawner();
            LoadNewScene("FOREST_CHUNK_1");
            ScreenManager.Instance.ShowScreen(ScreenManager.ScreenType.none);
        }
        public void OnPlayerFound()
        {
            _playerInstance.GetComponent<CombatEntity>().currentHealth
            .Subscribe(hp => { if (hp <= 0) OnGameOver(); })
            .AddTo(this);
        }
        public void StartPlayerSetup()
        {
            Observable.EveryUpdate()
                .Select(_ => FindObjectsOfType<CombatEntity>()
                    .FirstOrDefault(x => x.team == Team.Player))
                .Where(x => x != null)
                .Take(1)
                .Subscribe(entity =>
                {
                    _playerInstance = entity.gameObject;
                    OnPlayerFound();
                })
                .AddTo(this);
        }
        public void OnGameOver()
        {
            SwitchGameState(GameState.GameOver);
        }
    }
    [System.Serializable]
    public enum GameState
    {
        Playing=2,
        GameOver = 3,
        MainMenu = 4,
        PauseScreen = 5
    }


}