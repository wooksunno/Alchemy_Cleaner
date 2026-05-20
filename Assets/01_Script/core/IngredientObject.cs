using UnityEngine;

/// <summary>
/// 씬에 존재하는 물리 재료 오브젝트입니다.
/// VR 컨트롤러로 집어 들어 Cauldron 트리거 안에 넣으면 자동 투입됩니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class IngredientObject : MonoBehaviour
{
    [Header("재료 데이터 (SO 연결 필수)")]
    public IngredientData data;

    private bool _consumed = false;

    /// <summary>
    /// CauldronController에서 호출 → 재료를 소모하고 오브젝트를 제거합니다.
    /// </summary>
    public void Consume()
    {
        if (_consumed) return;
        _consumed = true;

        // TODO: 투입 파티클 / 사운드 재생

        Destroy(gameObject);
    }
}