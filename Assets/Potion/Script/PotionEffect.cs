using UnityEngine;

/*
 * 
 * 이 스크립트는 포션이 깨질 때 실행되며
 * 포션 범위 안의 쓰레기 오브젝트들을 감지하여
 * 올바른 타입의 포션이 깨졌을 때 해당 쓰레기 오브젝트를 청소하는 역할을 합니다.
 * 
 */
[RequireComponent(typeof(PotionBase))] // PotionBase 컴포넌트가 반드시 필요함을 명시
public class PotionEffect : MonoBehaviour
{
    private PotionBase baseInfo;
    public float effectRadius = 1.5f; // 포션이 퍼지는 범위

    private void Awake()
    {
        baseInfo = GetComponent<PotionBase>();
    }

    // PotionShatter에서 SendMessage로 호출됨
    public void ExecuteEffect()
    {
        // 주변의 모든 충돌체 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, effectRadius);

        foreach (var hit in hitColliders)
        {
            // TrashObject 컴포넌트가 있는지 확인
            // 추후에 TrashObject 클래스가 추가되면 주석 해제하여 사용
            var trash = hit.GetComponentInParent<TrashObject>();

            if (trash != null)
            {
                // 타입이 일치하면 청소 요청
                trash.CleanUp(baseInfo.type);
            }
        }
    }

    // 에디터 뷰에서 범위를 보기 위한 함수
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}