using UnityEngine;

public class BagTrigger : MonoBehaviour
{
    [Header("🧪 허용할 태그 (기본값: Potion)")]
    [SerializeField] private string allowedTag = "Potion";

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(allowedTag)) return;

        var potionItem = other.GetComponent<PotionInventoryItem>();
        if (potionItem == null || potionItem.recipe == null) return;

        var grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) return;

        if (!grabInteractable.isSelected)
        {
            if (other.TryGetComponent<Collider>(out var col)) col.enabled = false;

            var slots = FindObjectsOfType<InventorySlot>();
            System.Array.Sort(slots, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

            bool success = false;
            foreach (var slot in slots)
            {
                if (slot.ItemCount == 0)
                {
                    slot.SetSlotData(potionItem.recipe, 1);

                    if (DataManager.Instance != null)
                        DataManager.Instance.AddItem(potionItem.recipe);

                    success = true;
                    break;
                }
            }

            if (success)
            {
                if (InventoryManager.Instance != null)
                    InventoryManager.Instance.SortAndRefreshInventory();

                potionItem.Consume();
            }
            else
            {
                col.enabled = true;
            }
        }
    }
}