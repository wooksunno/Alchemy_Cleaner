using UnityEngine;
using System.Collections;

public class PotionPurifier : MonoBehaviour
{
    [Header("정화에 필요한 포션 개수")]
    public int requiredPotions = 2;
    private int _currentPotionCount = 0;

    [Header("사라지게 할 뿌연 연기 오브젝트들")]
    [Tooltip("오염 공간을 채우고 있는 파티클이나 연기 메시 오브젝트를 넣어주세요.")]
    public GameObject[] fogObjects;

    [Header("정화 성공 시 터뜨릴 이펙트 (선택)")]
    public ParticleSystem cleanEffect;

    private void OnTriggerEnter(Collider other)
    {
        // 던진 포션 오브젝트에 "Potion" 태그가 붙어있어야 합니다.
        if (other.CompareTag("Potion"))
        {
            _currentPotionCount++;
            Debug.Log($"[정화 시스템] 포션 적중! 현재 {_currentPotionCount} / {requiredPotions}");

            // 들어온 포션은 부딪혔으니 파괴 (또는 깨지는 이펙트)
            Destroy(other.gameObject);

            // 포션 2개가 다 모였다면 정화 시작
            if (_currentPotionCount >= requiredPotions)
            {
                StartCoroutine(PurifyRoutine());
            }
        }
    }

    private IEnumerator PurifyRoutine()
    {
        Debug.Log("[정화 시스템] 정화 조건 달성! 연기를 제거합니다.");

        if (cleanEffect != null)
        {
            cleanEffect.Play();
        }

        // 연기 오브젝트들을 서서히 끄거나 바로 비활성화
        foreach (GameObject fog in fogObjects)
        {
            if (fog != null)
            {
                // 조금 더 부드럽게 연출하고 싶다면 1초 뒤에 꺼지도록 유도
                yield return new WaitForSeconds(0.3f);
                fog.SetActive(false);
            }
        }

        // 오염 구역 자체의 콜라이더도 꺼서 중복 작동 방지
        GetComponent<Collider>().enabled = false;
    }
}