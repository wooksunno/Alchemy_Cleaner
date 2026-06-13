using UnityEngine;

/*
 * 
 * 이 스크립트는 포션이 깨질 때의 시각 효과와 효과 실행, 그리고 포션 모델 삭제를 담당합니다.
 * 
*/


[RequireComponent(typeof(PotionEffect))] // PotionBase 컴포넌트가 반드시 필요함을 명시
public class PotionShatter : MonoBehaviour
{
    public GameObject breakEffectPrefab; // 깨질 때 나올 파티클
    public float destroyDelay = 2f; // 파티클 오브젝트 삭제 딜레이

    [Header("정화 구역 감지")]
    [Tooltip("포션이 깨질 때 주변의 PotionPurifier를 감지할 반경")]
    public float purifierDetectRadius = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        // 씬 시작하자마자 깨져서 일단 바닥에만 반응하게끔 임시로 해둠
        // 이펙트 테스트 용도
        if (!collision.collider.CompareTag("floor")) return;

        // 깨지는 시각 효과 생성
        if (breakEffectPrefab != null)
        {
            GameObject instance = Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
            Destroy(instance, 2f);
        }

        // 이 오브젝트에 붙어있는 '효과 스크립트'를 실행시킴
        SendMessage("ExecuteEffect", SendMessageOptions.DontRequireReceiver);

        // ✨ 주변 PotionPurifier(Poison_Zone)에 직접 알림
        // OnTriggerEnter가 Destroy 타이밍과 겹쳐 호출되지 않는 문제를 회피하기 위함
        Collider[] nearby = Physics.OverlapSphere(transform.position, purifierDetectRadius);
        foreach (var col in nearby)
        {
            var purifier = col.GetComponent<PotionPurifier>();
            if (purifier != null)
            {
                purifier.RegisterPotionHit();
            }
        }

        // 포션 모델 삭제
        Destroy(gameObject);
    }
}