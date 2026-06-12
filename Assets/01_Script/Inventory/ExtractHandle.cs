using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ExtractHandle : MonoBehaviour
{
    private InventorySlot _ownerSlot;
    private XRGrabInteractable _grabInteractable;
    private XRInteractionManager _interactionManager;

    public void Initialize(InventorySlot owner)
    {
        _ownerSlot = owner;
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _interactionManager = FindFirstObjectByType<XRInteractionManager>();
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (_ownerSlot == null) return;

        var interactor = args.interactorObject;

        // ✨ 먼저 이 핸들에 대한 선택을 명시적으로 해제 (Destroy 전에)
        if (_interactionManager != null)
        {
            _interactionManager.SelectExit(interactor, _grabInteractable);
        }

        // 실제 포션 스폰 + 데이터 정리
        _ownerSlot.ExtractToInteractor(interactor);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_grabInteractable != null)
            _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }
}