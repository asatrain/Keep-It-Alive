using UnityEngine;

public class PauseMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip clickBack;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Continue();
    }

    public void Continue()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(clickBack);
        GameManager.Instance.GameState = GameState.Play;
    }

    public void Options()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(click);
        gameObject.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void MainMenu()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(clickBack);
        GameManager.Instance.GameState = GameState.MainMenu;
    }
}