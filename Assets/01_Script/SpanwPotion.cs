using UnityEngine;

// 스폰 포션 테스트용 코드
// UI의 버튼에 함수 연결해둠

public class SpanwPotion : MonoBehaviour
{
    public GameObject PotionPrefab;
    public Transform SpanwPosition;

    public void SpawnPotion()
    {
        Instantiate(PotionPrefab, SpanwPosition.position, Quaternion.identity);
    }
}
