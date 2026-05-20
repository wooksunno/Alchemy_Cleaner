using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 모든 레시피를 보관하는 ScriptableObject 컨테이너
/// 메뉴: Alchemy → Recipe Database
/// 프로젝트에 하나만 생성하여 CauldronController에 연결하세요.
/// </summary>
[CreateAssetMenu(fileName = "RecipeDatabase", menuName = "Alchemy/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    [SerializeField]
    private List<PotionRecipe> recipes = new List<PotionRecipe>();

    public IReadOnlyList<PotionRecipe> AllRecipes => recipes;

    /// <summary>
    /// 현재 라이선스로 해금된 레시피만 반환합니다.
    /// </summary>
    public List<PotionRecipe> GetAvailableRecipes(LicenseGrade license)
    {
        return recipes.FindAll(r => r.requiredLicense <= license);
    }

    /// <summary>
    /// 솥의 재료 조합과 일치하는 레시피를 탐색합니다.
    /// 라이선스 미달 레시피는 건너뜁니다.
    /// </summary>
    public PotionRecipe FindMatchingRecipe(
        Dictionary<IngredientData, int> cauldronIngredients,
        LicenseGrade playerLicense)
    {
        foreach (var recipe in recipes)
        {
            if (recipe.requiredLicense > playerLicense) continue;
            if (recipe.IsMatch(cauldronIngredients)) return recipe;
        }
        return null;
    }
}