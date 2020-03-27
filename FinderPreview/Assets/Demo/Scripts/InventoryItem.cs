using UnityEngine;

public enum InventoryItemType {
    Consumable,
    NonConsumable
}

[CreateAssetMenu (fileName = "InventoryItem", menuName = "FinderDemo/InventoryItem", order = 0)]
public class InventoryItem : ScriptableObject {
    public string Id;
    public string DisplayName;
    public Sprite Icon;
    public InventoryItemType ItemType;
}