using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemInfo", fileName = "ItemInfo")]
public class ItemInfo : ScriptableObject
{
    public enum ItemId
    {
        Firewood,
        Jerrycan,
        Beer,
        Wine,
        Whiskey
    }

    public ItemId id;
    public bool stackable;
    public int maxCount;
    public Sprite icon;
    public WorldItem worldItem;
    public float bonfireHealthAddition;
    public bool isStaminaBooster;
    public Color staminaSliderColor;
    public float staminaBoostTime;
    public float staminaBoostMultiplier;
    public int chopPowerBoost;
}