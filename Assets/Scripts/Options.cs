using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Image soundIcon;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Image musicIcon;
    [SerializeField] private Sprite volumeOnSprite;
    [SerializeField] private Sprite volumeOffSprite;
    [SerializeField] private AudioClip clickBack;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Back();
    }

    private void OnEnable()
    {
        var soundVolume = PlayerPrefs.GetFloat("soundVolume", 1);
        var musicVolume = PlayerPrefs.GetFloat("musicVolume", 1);
        soundVolumeSlider.value = soundVolume;
        soundIcon.sprite = soundVolume > 0 ? volumeOnSprite : volumeOffSprite;
        musicVolumeSlider.value = musicVolume;
        musicIcon.sprite = musicVolume > 0 ? volumeOnSprite : volumeOffSprite;
    }

    public void SoundVolumeChanged()
    {
        PlayerPrefs.SetFloat("soundVolume", soundVolumeSlider.value);
        soundIcon.sprite = soundVolumeSlider.value > 0 ? volumeOnSprite : volumeOffSprite;
    }

    public void MusicVolumeChanged()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
        musicIcon.sprite = musicVolumeSlider.value > 0 ? volumeOnSprite : volumeOffSprite;
    }

    public void Back()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(clickBack);
        gameObject.SetActive(false);
        parent.SetActive(true);
    }
}