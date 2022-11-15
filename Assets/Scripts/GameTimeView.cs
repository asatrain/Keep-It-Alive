using TMPro;
using UnityEngine;

public class GameTimeView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameTimeText;

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Play)
            return;

        UpdateGameTimeView();
    }

    private void UpdateGameTimeView()
    {
        var gameTime = GameManager.Instance.GameTime;
        var minutes = (int) gameTime / 60;
        var seconds = (int) gameTime % 60;
        gameTimeText.text = $"{minutes:D2} : {seconds:D2}";
    }
}