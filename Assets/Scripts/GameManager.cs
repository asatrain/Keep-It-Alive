using UnityEngine;

public enum GameState
{
    MainMenu,
    Play,
    Pause,
    GameOver
}

public delegate void GameStateChangeHandler(GameState prevState, GameState newState);

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameState startState;

    private GameState gameState;

    public event GameStateChangeHandler OnGameStateChanged;

    public GameState GameState
    {
        get => gameState;
        set
        {
            if (value == gameState) return;

            var prevState = gameState;
            gameState = value;
            Time.timeScale = gameState == GameState.Pause ? 0 : 1;
            if (gameState == GameState.MainMenu) GameTime = 0;
            OnGameStateChanged?.Invoke(prevState, gameState);
        }
    }

    public float GameTime { get; private set; }


    private void Start()
    {
        DontDestroyOnLoad(this);
        gameState = startState;
        if (gameState == GameState.MainMenu)
            GameTime = 0;
    }

    private void Update()
    {
        if (GameState != GameState.Play)
            return;

        GameTime += Time.deltaTime;
    }
}