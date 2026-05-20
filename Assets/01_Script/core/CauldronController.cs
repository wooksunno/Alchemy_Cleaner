using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 연금술 솥(Cauldron) 핵심 컨트롤러
/// 
/// [동작 흐름]
/// 1. IngredientObject가 트리거 진입 → AddIngredient() 자동 호출
/// 2. 레시피 미리 인식 후 OnRecipeMatched 이벤트 발생 (UI 피드백)
/// 3. VR 젓기 동작 → Stir() 호출 → stirRequiredCount 충족 시 자동 양조
///    또는 버튼 방식 사용 시 TryBrew() 직접 호출
/// 4. 성공 시 포션 프리팹 스폰 + OnPotionCreated 이벤트 발생
/// 5. 실패(레시피 불일치) 시 OnRecipeFailed 이벤트 발생 → 솥 초기화
/// </summary>
public class CauldronController : MonoBehaviour
{
    // ── 인스펙터 설정 ─────────────────────────────────────────

    [Header("데이터 연결")]
    [SerializeField] private RecipeDatabase recipeDatabase;

    [Header("제조 설정")]
    [Tooltip("완성된 포션이 나타날 위치 (비워두면 솥 위치에 스폰)")]
    [SerializeField] private Transform potionSpawnPoint;
    [Tooltip("양조 완료에 필요한 젓기 횟수")]
    [SerializeField] private int stirRequiredCount = 3;
    [Tooltip("여러 개 생성 시 포션 간 높이 오프셋")]
    [SerializeField] private float yieldSpawnOffset = 0.12f;

    [Header("연금술 대성공 (연구소 업그레이드 레벨 0~3)")]
    [Range(0, 3)]
    [SerializeField] private int criticalLevel = 0;
    [Range(0f, 1f)]
    [Tooltip("대성공 기본 확률 (업그레이드 레벨과 함께 작동)")]
    [SerializeField] private float criticalChance = 0.20f;
    
    [Header("VFX")]
    [Tooltip("제조 실패 VFX")]
    [SerializeField] private GameObject RecipeFailedEffectPrefab;

    // ── UnityEvent ────────────────────────────────────────────

    [Header("이벤트")]
    [Tooltip("레시피가 인식되었을 때 (UI 미리보기 등에 사용)")]
    public UnityEvent<PotionRecipe> OnRecipeMatched;
    [Tooltip("재료 불일치로 제조 실패했을 때")]
    public UnityEvent OnRecipeFailed;
    [Tooltip("포션 제조 완료 (포션 타입, 생산 수량)")]
    public UnityEvent<PotionType, int> OnPotionCreated;
    [Tooltip("재료가 추가될 때마다")]
    public UnityEvent<IngredientData> OnIngredientAdded;
    [Tooltip("솥이 초기화되었을 때")]
    public UnityEvent OnCauldronCleared;

    // ── 내부 상태 ─────────────────────────────────────────────

    // 현재 솥의 재료 (재료 SO : 개수)
    private readonly Dictionary<IngredientData, int> _cauldronIngredients
        = new Dictionary<IngredientData, int>();

    private int _stirCount = 0;
    private PotionRecipe _previewedRecipe = null;
    private LicenseGrade _playerLicense = LicenseGrade.Apprentice;

    // ── Public API ────────────────────────────────────────────

    /// <summary>플레이어 라이선스 등급을 갱신합니다. (라이선스 시스템과 연동)</summary>
    public void SetPlayerLicense(LicenseGrade grade) => _playerLicense = grade;

    /// <summary>연구소 업그레이드 레벨을 적용합니다. (0~3)</summary>
    public void SetCriticalLevel(int level) => criticalLevel = Mathf.Clamp(level, 0, 3);

    /// <summary>
    /// 재료를 솥에 추가합니다.
    /// IngredientObject가 트리거에 진입하면 자동 호출되거나, 외부에서 직접 호출 가능합니다.
    /// </summary>
    public void AddIngredient(IngredientObject ingredientObj)
    {
        if (ingredientObj == null || ingredientObj.data == null)
        {
            Debug.LogWarning("[Cauldron] 유효하지 않은 재료입니다.");
            return;
        }

        IngredientData data = ingredientObj.data;

        if (_cauldronIngredients.ContainsKey(data))
            _cauldronIngredients[data]++;
        else
            _cauldronIngredients[data] = 1;

        ingredientObj.Consume();

        OnIngredientAdded?.Invoke(data);

        // 레시피 미리 탐색 → UI 힌트용
        _previewedRecipe = recipeDatabase.FindMatchingRecipe(_cauldronIngredients, _playerLicense);
        if (_previewedRecipe != null)
            OnRecipeMatched?.Invoke(_previewedRecipe);

        Debug.Log($"[Cauldron] '{data.ingredientName}' 투입 | {GetIngredientLog()}");
    }

    /// <summary>
    /// 젓기 1회 동작을 기록합니다.
    /// VR 컨트롤러 스틱/국자 오브젝트의 충돌 이벤트 등에서 호출하세요.
    /// stirRequiredCount에 도달하면 자동으로 양조를 시도합니다.
    /// </summary>
    public void Stir()
    {
        if (_cauldronIngredients.Count == 0)
        {
            Debug.Log("[Cauldron] 재료를 먼저 넣어주세요.");
            return;
        }

        _stirCount++;
        Debug.Log($"[Cauldron] 젓기 {_stirCount} / {stirRequiredCount}");

        if (_stirCount >= stirRequiredCount)
            TryBrew();
    }

    /// <summary>
    /// 즉시 양조를 시도합니다. (버튼 UI 방식의 대안)
    /// </summary>
    public void TryBrew()
    {
        _previewedRecipe = recipeDatabase.FindMatchingRecipe(_cauldronIngredients, _playerLicense);

        if (_previewedRecipe == null)
        {
            Debug.Log("[Cauldron] 일치하는 레시피 없음. 솥을 초기화합니다.");
            OnRecipeFailed?.Invoke();
            ClearCauldron();

            // 시각 효과 생성
            if (RecipeFailedEffectPrefab != null)
            {
                GameObject instance = Instantiate(RecipeFailedEffectPrefab, transform.position, Quaternion.identity);
                Transform _transform = instance.GetComponent<Transform>();
                _transform.localScale = new Vector3(5,5,5);
                Destroy(instance, 2f);
            }
            return;
        }

        ExecuteBrewing(_previewedRecipe);
    }

    /// <summary>솥을 비우고 초기 상태로 되돌립니다.</summary>
    public void ClearCauldron()
    {
        _cauldronIngredients.Clear();
        _stirCount = 0;
        _previewedRecipe = null;
        OnCauldronCleared?.Invoke();
        Debug.Log("[Cauldron] 솥 초기화 완료.");
    }

    /// <summary>현재 솥의 재료 목록을 읽기 전용으로 반환합니다.</summary>
    public IReadOnlyDictionary<IngredientData, int> GetCurrentIngredients()
        => _cauldronIngredients;

    // ── 내부 로직 ─────────────────────────────────────────────

    private void ExecuteBrewing(PotionRecipe recipe)
    {
        int totalYield = CalculateYield(recipe.baseYield);

        Debug.Log($"[Cauldron] ✅ '{recipe.potionName}' 제조 성공! 생산량: {totalYield}개");

        SpawnPotions(recipe.resultPrefab, totalYield);

        OnPotionCreated?.Invoke(recipe.resultType, totalYield);

        ClearCauldron();
    }

    private void SpawnPotions(GameObject prefab, int count)
    {
        if (prefab == null) return;

        Vector3 basePos = potionSpawnPoint != null
            ? potionSpawnPoint.position
            : transform.position + Vector3.up * 0.5f;

        for (int i = 0; i < count; i++)
        {
            // 여러 개 생성 시 높이 오프셋으로 겹침 방지
            Vector3 spawnPos = basePos + Vector3.up * (i * yieldSpawnOffset);
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// 연금술 대성공 확률 계산 (기획서 연구소 업그레이드 반영)
    /// Level 0 : 기본 생산 + 확률적 +1
    /// Level 1 : 기본 생산 + 확정 +1 + 확률적 +1
    /// Level 2 : 기본 생산 + 확정 +1 + 확률적 +1 (Lv1과 동일 구간, 추후 확률 조정)
    /// Level 3 : 기본 생산 + 확률적 +1 & +2 동시 적용
    /// </summary>
    private int CalculateYield(int baseYield)
    {
        int bonus = 0;

        switch (criticalLevel)
        {
            case 0:
                if (Roll()) bonus += 1;
                break;
            case 1:
                bonus += 1;                   // 확정 +1
                if (Roll()) bonus += 1;       // 확률 +1
                break;
            case 2:
                bonus += 1;
                if (Roll()) bonus += 1;
                break;
            case 3:
                if (Roll()) bonus += 1;       // 확률 +1
                if (Roll()) bonus += 2;       // 확률 +2 (별도 판정)
                break;
        }

        int total = baseYield + bonus;
        Debug.Log($"[Cauldron] 생산량 계산: 기본 {baseYield} + 보너스 {bonus} = {total}");
        return Mathf.Max(1, total);
    }

    private bool Roll() => Random.value < criticalChance;

    private string GetIngredientLog()
    {
        var parts = new List<string>();
        foreach (var kv in _cauldronIngredients)
            parts.Add($"{kv.Key.ingredientName} ×{kv.Value}");
        return string.Join(", ", parts);
    }

    // ── 에디터 기즈모 ─────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        if (potionSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(potionSpawnPoint.position, 0.1f);
            Gizmos.DrawLine(transform.position, potionSpawnPoint.position);
        }
    }
}