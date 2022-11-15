using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI upperText;
    [SerializeField] private TextMeshProUGUI middleText;
    [SerializeField] private TextMeshProUGUI lowerText;
    [SerializeField] private ItemInfo itemInfo;

    private void Start()
    {
        image.sprite = itemInfo.icon;
        if (itemInfo.isStaminaBooster)
        {
            upperText.text = $"Duration: {itemInfo.staminaBoostTime}";
            middleText.text = $"Speed + stamina boost: {itemInfo.staminaBoostMultiplier}";
            lowerText.text = $"Chop power: {itemInfo.chopPowerBoost}";
        }
        else
        {
            upperText.text = lowerText.text = string.Empty;
            middleText.text = $"+ {itemInfo.bonfireHealthAddition} HP";
        }
    }
}