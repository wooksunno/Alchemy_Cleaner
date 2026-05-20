using UnityEngine;

/// <summary>
/// 연금술 재료 데이터 ScriptableObject
/// 메뉴: Alchemy → Ingredient Data
/// </summary>
[CreateAssetMenu(fileName = "NewIngredient", menuName = "Alchemy/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    [Header("기본 정보")]
    public string ingredientName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("VR 씬 오브젝트")]
    [Tooltip("씬에 스폰될 물리 재료 프리팹 (IngredientObject 컴포넌트 필수)")]
    public GameObject worldPrefab;

    [Header("상점")]
    [Tooltip("상점에서 구입할 가격")]
    [Min(0)]
    public int purchasePrice;
}