using UnityEngine;

public class BagTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        var ingredientObj = other.GetComponent<IngredientObject>();
        if (ingredientObj == null || ingredientObj.data == null) return;

        var grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) return;

        // 유저가 손에서 물건을 놓았을 때만 가방으로 쏙 흡수
        if (!grabInteractable.isSelected)
        {
            if (other.TryGetComponent<Collider>(out var col)) col.enabled = false;

            var slots = FindObjectsOfType<InventorySlot>();
            System.Array.Sort(slots, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

            bool success = false;
            foreach (var slot in slots)
            {
                if (slot.ItemCount == 0) // 비어있는 첫 번째 칸 찾기
                {
                    slot.SetSlotData(ingredientObj.data, 1, null);

                    // ✨ 이름 문자열이 아닌 SO 데이터 객체 주소를 다이렉트로 저장!
                    if (DataManager.Instance != null)
                    {
                        DataManager.Instance.AddItem(ingredientObj.data);
                    }

                    success = true;
                    break;
                }
            }

            if (success)
            {
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.SortAndRefreshInventory();
                }
                ingredientObj.Consume(); // 필드에 있던 원래 물체 파괴
            }
            else
            {
                col.enabled = true; // 가방 꽉 찼으면 튕겨내기
            }
        }
    }
}