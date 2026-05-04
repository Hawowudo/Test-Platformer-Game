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
        GameOver,
        GameWin,
        none
    }
    public static ScreenManager Instance;

    public ScreenType screenType;
    [SerializeField] private GameObject _mainMenuScreen, _pauseScreen, _gameplayScreen, _gameOverScreen, _gameWinScreen;

    public bool debugMode;
    private ScreenType _previousScreenType;
    #region Unity Functions
    private void Start()
    {
        ShowScreen(screenType);
        _previousScreenType = screenType;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {
        if (!debugMode)
            return;

        if (screenType != _previousScreenType)
        {
            ShowScreen(screenType);
            _previousScreenType = screenType;
        }
    }
    #endregion


    public void ShowScreen(ScreenType type)
    {
        _mainMenuScreen.SetActive(type == ScreenType.MainMenu);
        _pauseScreen.SetActive(type == ScreenType.PauseScreen);
        _gameplayScreen.SetActive(type == ScreenType.Gameplay);
        _gameOverScreen.SetActive(type == ScreenType.GameOver);
        _gameWinScreen.SetActive(type == ScreenType.GameWin);
    }


}
