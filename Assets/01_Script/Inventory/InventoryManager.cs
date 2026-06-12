using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("슬롯 리스트")]
    public List<InventorySlot> allSlots = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GetComponentsInChildren<InventorySlot>(allSlots);
        RefreshAllSlots();
    }

    public void SortAndRefreshInventory()
    {
        RefreshAllSlots();
    }

    public bool TryAddItemDynamic(PotionRecipe recipe)
    {
        if (recipe == null) return false;

        foreach (var slot in allSlots)
        {
            if (slot.ItemCount == 0)
            {
                slot.SetSlotData(recipe, 1);
                RefreshAllSlots();
                return true;
            }
        }

        return false;
    }

    public void RefreshAllSlots()
    {
        foreach (var slot in allSlots)
        {
            slot.UpdateSlotUI();
        }
    }
}