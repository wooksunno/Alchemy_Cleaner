using UnityEngine;

/// <summary>
/// IngredientTrigger 오브젝트에 부착 → 재료 감지를 CauldronController에 중계합니다.
/// CauldronController의 OnTriggerEnter 대신 이쪽에서 처리합니다.
/// </summary>
public class IngredientTriggerRelay : MonoBehaviour
{
    [SerializeField] private CauldronController cauldron;

    private void OnTriggerEnter(Collider other)
    {
        var ingredient = other.GetComponent<IngredientObject>();
        if (ingredient != null)
            cauldron.AddIngredient(ingredient);
    }
}