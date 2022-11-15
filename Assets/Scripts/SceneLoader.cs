using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private const string MainMenuSceneName = "MainMenu";
    private const string GameSceneName = "Game";

    private static readonly int StartTrigger = Animator.StringToHash("Start");
    private static readonly int EndTrigger = Animator.StringToHash("End");
    [SerializeField] private GameObject canvas;
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime;

    private void Start()
    {
        DontDestroyOnLoad(this);
        canvas.SetActive(true);
        GameManager.Instance.OnGameStateChanged += GameStateChangeHandler;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (!ReferenceEquals(GameManager.Instance, null))
            GameManager.Instance.OnGameStateChanged -= GameStateChangeHandler;
    }

    private void GameStateChangeHandler(GameState prevState, GameState newState)
    {
        switch (newState)
        {
            case GameState.Play when prevState == GameState.MainMenu:
                StartCoroutine(LoadScene(GameSceneName));
                break;
            case GameState.MainMenu when prevState == GameState.Pause || prevState == GameState.GameOver:
                StartCoroutine(LoadScene(MainMenuSceneName));
                break;
        }
    }

    private IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger(StartTrigger);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
        transition.SetTrigger(EndTrigger);
    }
}