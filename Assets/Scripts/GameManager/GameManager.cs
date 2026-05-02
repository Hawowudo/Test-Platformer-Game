using System.Collections;
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

        private GameState m_CurrentGameState;
        private GameState m_PreviousGameState;
        private bool _isSceneLoading = false;
        private bool _gameStarted;

        public float _menuOpenCloseInputBufferTime = 0.1f;
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
            SwitchGameState(GameState.UI);
        }
        private void Update()
        {
            _menuOpenCloseInputBufferCooldown += Time.unscaledDeltaTime;
        }
        #endregion

        public void SwitchGameState(GameState state)
        {
            if(m_CurrentGameState == state)
            {
                return;
            }
            m_PreviousGameState = m_CurrentGameState != GameState.UI ? m_CurrentGameState : GameState.Playing;
            m_CurrentGameState = state;
            switch (m_CurrentGameState)
            {
                case GameState.UI:
                    PauseGame();
                    break;
                case GameState.Playing:
                    ResumeGame();
                    break;
            }
        }
        #region Saves
        public void LoadNewScene(string sceneName)
        {
            if (_isSceneLoading)
            {
                Debug.LogWarning("Scene is already loading, cannot load new scene.");
                return;
            }
            _isSceneLoading = true;
            StartCoroutine(LoadScene(sceneName));
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
        public void PauseGame()
        {
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
        }
        public void PauseGameDuration(float duration = 0.2f)
        {
            // If the game is already paused, we don't want to start another pause coroutine
            StartCoroutine(PauseGameForDuration(duration));
        }
        private IEnumerator PauseGameForDuration(float duration)
        {
            PauseGame();
            yield return new WaitForSecondsRealtime(duration);
            if (m_CurrentGameState != GameState.UI)
            {
                ResumeGame();
            }
        }

    }
    [System.Serializable]
    public enum GameState
    {
        UI=1,
        Playing=2,
    }


}