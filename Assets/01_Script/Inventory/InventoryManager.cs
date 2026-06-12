using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리 슬롯들을 총괄 관리하는 매니저입니다.
/// </summary>
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

    /// <summary>
    /// 슬롯 UI 전체를 새로고침합니다.
    /// </summary>
    public void SortAndRefreshInventory()
    {
        RefreshAllSlots();
    }

    /// <summary>
    /// 빈 슬롯에 포션을 배치합니다.
    /// </summary>
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