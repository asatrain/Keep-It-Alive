using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField] private AudioSource source;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        source.volume = PlayerPrefs.GetFloat("musicVolume", 1);
        UpdateMusicPlayerTransform();
    }

    private void UpdateMusicPlayerTransform()
    {
        var globalAudioTransform = GlobalAudio.Instance.transform;
        var playerTransform = transform;
        playerTransform.position = globalAudioTransform.position;
        playerTransform.rotation = globalAudioTransform.rotation;
    }
}