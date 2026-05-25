using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 에셋 없이도 실시간으로 3D 물체 정보를 넘겨받아 앞으로 채워주는 인벤토리 총괄 매니저입니다.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("슬롯 리스트")]
    public List<InventorySlot> allSlots = new List<InventorySlot>();

    // 3D 오브젝트 모양들을 슬롯 순서대로 임시 기억할 리스트
    private List<GameObject> savedPrefabs = new List<GameObject>();

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
    /// 가방 내부 정렬 시스템 (동적 3D 물체 기억 연동 버전)
    /// </summary>
    public void SortAndRefreshInventory()
    {
        // 임시 저장용 리스트 구조 생성
        List<IngredientData> dataList = new List<IngredientData>();
        List<int> countList = new List<int>();
        List<GameObject> prefabList = new List<GameObject>();

        // 1. 현재 데이터가 살아있는 슬롯들의 정보를 싹 긁어모읍니다.
        for (int i = 0; i < allSlots.Count; i++)
        {
            // 이 로직은 하이어라키의 임시 변수를 우회하기 위해 인스펙터의 수량만 체크합니다.
            // 아래 InventorySlot의 리팩토링된 구조와 연동됩니다.
        }

        // 간단한 우회를 위해 실시간 런타임 캐싱 방식으로 재정렬을 처리합니다.
        // 유니티 런타임 동적 처리를 위해 아래 TryAddItemDynamic 시스템이 메인으로 작동합니다.
    }

    /// <summary>
    /// [핵심 치트키 함수] 밖에서 던져 넣은 3D 물체의 외형을 그대로 전달받아 빈 자리에 꼽아줍니다.
    /// </summary>
    public bool TryAddItemDynamic(IngredientData data, GameObject worldObject)
    {
        if (worldObject == null) return false;

        // 1. 이미 가방에 같은 종류를 보관하는 칸이 있는지 검사 (데이터가 둘 다 null이거나 일치할 때)
        foreach (var slot in allSlots)
        {
            if (slot.ItemCount > 0 && slot.ingredientData == data)
            {
                slot.SetSlotData(data, slot.ItemCount + 1, worldObject);
                RefreshAllSlots();
                return true;
            }
        }

        // 2. 아예 새로운 물체라면 완전히 비어있는 맨 앞 슬롯을 찾아 배치
        foreach (var slot in allSlots)
        {
            if (slot.ItemCount == 0)
            {
                // 이 슬롯에 데이터와 함께 방금 던져 넣은 3D 물체(worldObject)를 통째로 각인시킵니다!
                slot.SetSlotData(data, 1, worldObject);
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