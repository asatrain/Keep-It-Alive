using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private TextMeshProUGUI gameTimeText;
    [SerializeField] private AudioClip clickBack;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) MainMenu();
    }

    private void OnEnable()
    {
        var record = Mathf.Clamp(PlayerPrefs.GetInt("record", 0), 0, int.MaxValue);
        var gameTime = (int) GameManager.Instance.GameTime;
        recordText.text = $"{record / 60:D2} : {record % 60:D2}";
        gameTimeText.text = $"{gameTime / 60:D2} : {gameTime % 60:D2}";
        if (gameTime > record) PlayerPrefs.SetInt("record", gameTime);
    }

    public void MainMenu()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(clickBack);
        GameManager.Instance.GameState = GameState.MainMenu;
    }
}