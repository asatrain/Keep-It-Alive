using UnityEngine;

public class GameScreenController : MonoBehaviour
{
    private static readonly int Paused = Animator.StringToHash("Paused");
    private static readonly int GameOver = Animator.StringToHash("Game Over");

    [SerializeField] private Animator screenAnimator;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameStateChangeHandler;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.GameState == GameState.Play)
            GameManager.Instance.GameState = GameState.Pause;
    }

    private void OnDestroy()
    {
        if (!ReferenceEquals(GameManager.Instance, null))
            GameManager.Instance.OnGameStateChanged -= GameStateChangeHandler;
    }

    private void GameStateChangeHandler(GameState prevState, GameState newState)
    {
        switch (newState)
        {
            case GameState.Play:
                screenAnimator.SetBool(Paused, false);
                break;
            case GameState.Pause:
                screenAnimator.SetBool(Paused, true);
                break;
            case GameState.GameOver:
                screenAnimator.SetTrigger(GameOver);
                break;
        }
    }
}