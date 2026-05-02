using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public enum ScreenType
    {
        MainMenu,
        PauseScreen,
        Gameplay,
        GameOver
    }
    public static ScreenManager Instance;
    public ScreenType screenType;
    [SerializeField] private GameObject _mainMenuScreen, _pauseScreen, _gameplayScreen, _gameOverScreen;

    #region Unity Functions
    private void Start()
    {
        ShowScreen(screenType);
    }
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
    #endregion


    public void ShowScreen(ScreenType type)
    {
        _mainMenuScreen.SetActive(type == ScreenType.MainMenu);
        _pauseScreen.SetActive(type == ScreenType.PauseScreen);
        _gameplayScreen.SetActive(type == ScreenType.Gameplay);
        _gameOverScreen.SetActive(type == ScreenType.GameOver);
    }


}
