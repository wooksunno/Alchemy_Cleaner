using UnityEngine;

// 스폰 포션 테스트용 코드
// UI의 버튼에 함수 연결해둠

public class SpawnPotion : MonoBehaviour
{
    public GameObject[] PotionPrefab;
    public Transform SpawnPosition;
    
    public int index = 0;

    public void SpawnPotionByIndex()
    {
        Instantiate(PotionPrefab[index], SpawnPosition.position, Quaternion.identity);
        index += index >= PotionPrefab.Length - 1 ? -PotionPrefab.Length + 1 : 1;

    }
}
