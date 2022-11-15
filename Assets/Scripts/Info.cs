using TMPro;
using UnityEngine;

public class Info : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private AudioClip clickBack;
    [SerializeField] private TextMeshProUGUI recordText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Back();
    }

    private void OnEnable()
    {
        var record = Mathf.Clamp(PlayerPrefs.GetInt("record", 0), 0, int.MaxValue);
        recordText.text = $"RECORD {record / 60:D2} : {record % 60:D2}";
    }

    public void Back()
    {
        GlobalAudio.Instance.SoundPlayer.PlayOneShot(clickBack);
        gameObject.SetActive(false);
        parent.SetActive(true);
    }
}