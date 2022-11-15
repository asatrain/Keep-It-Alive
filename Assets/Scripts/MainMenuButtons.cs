using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject info;
    [SerializeField] private AudioClip click;

    public void NewGame()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(click);
        GameManager.Instance.GameState = GameState.Play;
    }

    public void Options()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(click);
        gameObject.SetActive(false);
        options.SetActive(true);
    }

    public void Info()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(click);
        gameObject.SetActive(false);
        info.SetActive(true);
    }

    public void Quit()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(click);
        Debug.Log("Quit!");
        Application.Quit();
    }
}