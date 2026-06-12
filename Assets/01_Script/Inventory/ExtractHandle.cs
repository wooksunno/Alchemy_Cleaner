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
        Debug.Log("[ExtractHandle] OnGrabbed 호출됨!");

        if (_ownerSlot == null) return;

        var interactor = args.interactorObject;

        if (_interactionManager != null)
        {
            _interactionManager.SelectExit(interactor, _grabInteractable);
        }

        _ownerSlot.ExtractToInteractor(interactor);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_grabInteractable != null)
            _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }
}