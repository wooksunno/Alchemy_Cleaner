using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 에셋 데이터(SO) 없이, 필드의 3D 오브젝트를 직접 기억하고 실시간으로 복제하여 추출하는 슬롯 클래스입니다.
/// </summary>
public class InventorySlot : MonoBehaviour
{
    [Header("슬롯 상태")]
    [Tooltip("기억용 데이터 (지금은 비워두셔도 됩니다)")]
    public IngredientData ingredientData;

    [SerializeField] private int itemCount = 0;
    public int ItemCount => itemCount;

    [Header("실시간 기억 장치 (치트키)")]
    // 유저가 가방에 집어넣은 3D 오브젝트의 원본 모양을 임시로 저장해 두는 변수입니다.
    private GameObject savedWorldPrefab;

    [Header("UI 컴포넌트 연결")]
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI countText;

    private XRInteractionManager _interactionManager;

    private void Start()
    {
        _interactionManager = FindFirstObjectByType<XRInteractionManager>();
        UpdateSlotUI();
    }

    public void SetSlotData(IngredientData data, int count, GameObject prefab)
    {
        ingredientData = data;
        itemCount = count;
        savedWorldPrefab = prefab; // 들어온 3D 물체의 원본 형태를 기억합니다.
    }

    public void ClearSlot()
    {
        ingredientData = null;
        itemCount = 0;
        savedWorldPrefab = null; // 기억 삭제
    }

    /// <summary>
    /// 슬롯의 UI 비주얼을 새로고침합니다. 에셋 그림이 없으면 유니티 기본 하얀 네모로 채웁니다.
    /// </summary>
    public void UpdateSlotUI()
    {
        if (itemCount > 0)
        {
            if (itemIconImage != null)
            {
                // 만약 에셋에 그림이 등록되어 있다면 쓰고, 없으면 그냥 하얀색 네모로 채워서 칸이 찼음을 알려줍니다.
                if (ingredientData != null && ingredientData.icon != null)
                {
                    itemIconImage.sprite = ingredientData.icon;
                }
                else
                {
                    itemIconImage.sprite = null; // 기본 하얀 네모 이미지 유지
                }

                itemIconImage.color = Color.white; // 불투명하게 만들기
                itemIconImage.enabled = true;
            }

            if (countText != null) countText.text = itemCount.ToString();
        }
        else
        {
            // 완전히 빈 칸일 때 처리
            if (itemIconImage != null) itemIconImage.enabled = false;
            if (countText != null) countText.text = "";
        }

        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// 가방에서 끄집어낼 때: 기억해 둔 3D 오브젝트 모양 그대로 복제해서 손에 쥐여줍니다.
    /// </summary>
    public void TryExtractIngredient(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor)
    {
        // 꺼낼 개수가 없거나, 기억해 둔 3D 원본 모양이 없으면 취소
        if (itemCount <= 0 || savedWorldPrefab == null) return;

        // 기억해 둔 3D 원본 모양을 그대로 실시간 복제(Instantiate)합니다!
        GameObject spawnedObj = Instantiate(savedWorldPrefab, interactor.transform.position, interactor.transform.rotation);

        // 크기가 너무 거대해지는 것을 방지하기 위해 일반적인 3D 물체 크기(1,1,1)로 리셋
        spawnedObj.transform.localScale = Vector3.one;

        var grabInteractable = spawnedObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // 손에 강제 그랩 처리
        if (grabInteractable != null && _interactionManager != null)
        {
            _interactionManager.SelectEnter(interactor, grabInteractable);

            itemCount--;
            if (itemCount <= 0) ClearSlot();

            // 소모 후 매니저에게 전체 정렬 지시
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SortAndRefreshInventory();
            }
        }
    }

    public void TryExtractIngredientXRI(SelectEnterEventArgs args)
    {
        TryExtractIngredient(args.interactorObject);
    }

    /// <summary>
    /// 밖에서 3D 오브젝트를 던져 넣을 때 호출되는 영역입니다.
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        var ingredientObj = other.GetComponent<IngredientObject>();
        if (ingredientObj == null) return; // 주울 수 있는 물체인지 검사

        var grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null) return;

        // 유저가 손에서 놓았을 때 가방으로 흡수
        if (!grabInteractable.isSelected)
        {
            if (other.TryGetComponent<Collider>(out var col)) col.enabled = false;

            // [핵심 변경] 매니저에게 아이템 데이터뿐만 아니라, 이 3D 오브젝트의 원본 모양(other.gameObject)까지 같이 넘겨줍니다!
            if (InventoryManager.Instance != null)
            {
                bool success = InventoryManager.Instance.TryAddItemDynamic(ingredientObj.data, other.gameObject);
                if (success)
                {
                    ingredientObj.Consume(); // 흡수 성공 시 원래 월드에 있던 물체 파괴
                }
                else
                {
                    col.enabled = true;
                }
            }
        }
    }
}