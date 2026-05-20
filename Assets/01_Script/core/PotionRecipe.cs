using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 포션 레시피 ScriptableObject
/// 메뉴: Alchemy → Potion Recipe
/// 재료 목록을 수정하면 자동으로 매칭 로직에 반영됩니다.
/// </summary>
[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alchemy/Potion Recipe")]
public class PotionRecipe : ScriptableObject
{
    // ── 내부 구조체 ───────────────────────────────────────────
    [System.Serializable]
    public struct IngredientEntry
    {
        public IngredientData ingredient;
        [Min(1)] public int amount;
    }

    // ── 결과 포션 ─────────────────────────────────────────────
    [Header("결과 포션")]
    public string potionName;
    [TextArea(2, 4)]
    public string description;
    public PotionType resultType;
    [Tooltip("제조 완료 시 스폰될 포션 프리팹 (PotionBase 컴포넌트 필수)")]
    public GameObject resultPrefab;
    public Sprite resultIcon;

    // ── 재료 ──────────────────────────────────────────────────
    [Header("필요 재료")]
    [Tooltip("재료와 개수를 자유롭게 추가/수정하세요.")]
    public List<IngredientEntry> requiredIngredients;

    // ── 라이선스 & 제조 설정 ──────────────────────────────────
    [Header("라이선스 요구사항")]
    public LicenseGrade requiredLicense = LicenseGrade.Apprentice;
    [Tooltip("혼합 특수 포션 여부 (시니어 이상 전용 표시용)")]
    public bool isMixedPotion = false;

    [Header("기본 생산량")]
    [Min(1)]
    [Tooltip("한 번 제조 시 기본으로 생산되는 포션 수 (대성공 보너스 제외)")]
    public int baseYield = 1;

    // ── 레시피 매칭 ───────────────────────────────────────────

    /// <summary>
    /// 솥에 담긴 재료 딕셔너리가 이 레시피와 정확히 일치하는지 확인합니다.
    /// (재료 종류 수, 각 재료의 수량 모두 일치해야 함)
    /// </summary>
    public bool IsMatch(Dictionary<IngredientData, int> cauldronIngredients)
    {
        if (requiredIngredients == null || requiredIngredients.Count == 0) return false;

        // 재료 종류 수가 다르면 즉시 실패
        if (cauldronIngredients.Count != requiredIngredients.Count) return false;

        foreach (var entry in requiredIngredients)
        {
            // 해당 재료가 없거나 수량이 다르면 실패
            if (!cauldronIngredients.TryGetValue(entry.ingredient, out int count)) return false;
            if (count != entry.amount) return false;
        }

        return true;
    }
}