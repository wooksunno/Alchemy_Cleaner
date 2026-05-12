using UnityEngine;

/*
 * 
 * 이 스크립트는 포션이 깨질 때의 시각 효과와 효과 실행, 그리고 포션 모델 삭제를 담당합니다.
 * 
*/


[RequireComponent(typeof(PotionBase))] // PotionBase 컴포넌트가 반드시 필요함을 명시
public class PotionShatter : MonoBehaviour
{
    public GameObject breakEffectPrefab; // 깨질 때 나올 파티클

    private void OnCollisionEnter(Collision collision)
    {
        // 깨지는 시각 효과 생성
        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
        }

        // 이 오브젝트에 붙어있는 '효과 스크립트'를 실행시킴
        SendMessage("ExecuteEffect", SendMessageOptions.DontRequireReceiver);

        // 포션 모델 삭제
        Destroy(gameObject);
    }
}