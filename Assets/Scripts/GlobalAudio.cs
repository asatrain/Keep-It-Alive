using UnityEngine;

public class GlobalAudio : Singleton<GlobalAudio>
{
    [SerializeField] private AudioSource soundPlayer;

    public AudioSource SoundPlayer => soundPlayer;

    private void Update()
    {
        soundPlayer.volume = PlayerPrefs.GetFloat("soundVolume", 1);
    }
}