using UnityEngine;

/// <summary>
/// 포션이 깨질 때 범위 내 쓰레기를 감지하고 청소를 시도합니다.
/// TrashResponseDatabase를 통해 포션-쓰레기 반응을 판정합니다.
/// </summary>

public class PotionEffect : MonoBehaviour
{
    [Header("데이터 연결")] 
    public PotionType potionType;
    [SerializeField] private TrashResponseDatabase trashDatabase;

    [Header("효과 범위")]
    public float effectRadius = 1.5f;

    // PotionShatter에서 SendMessage로 호출됨
    public void ExecuteEffect()
    {
        if (trashDatabase == null)
        {
            Debug.LogError($"[PotionEffect] TrashResponseDatabase가 연결되지 않았습니다! ({gameObject.name})");
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);

        foreach (var hit in hitColliders)
        {
            var trash = hit.GetComponentInParent<TrashObject>();
            if (trash != null)
                trash.CleanUp(potionType, trashDatabase);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}