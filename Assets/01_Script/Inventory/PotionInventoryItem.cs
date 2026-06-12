using UnityEngine;

/// <summary>
/// 포션 프리팹에 붙이는 인벤토리 식별 컴포넌트
/// BagTrigger가 이걸 보고 가방에 흡수합니다.
/// </summary>
public class PotionInventoryItem : MonoBehaviour
{
    [Tooltip("이 포션의 레시피 SO (resultPrefab, potionName 참조용)")]
    public PotionRecipe recipe;

    /// <summary>
    /// 가방에 흡수될 때 이 오브젝트를 파괴합니다.
    /// </summary>
    public void Consume()
    {
        Destroy(gameObject);
    }
}