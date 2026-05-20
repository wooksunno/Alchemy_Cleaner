using UnityEngine;

/*
 * 
 * 쓰레기 청소 스크립트
 * PotionEffect에서 CleanUp() 함수를 호출할 때 실행됩니다.
 * 스크립트가 실행되면 포션의 속성과 쓰레기의 속성을 비교하여 맞는 경우 청소를 진행하고,
 * 맞지 않는 경우 아웃라인으로 표시합니다. 
 * 
 */


// 사고 방지: 아웃라인 기능이 반드시 함께 있도록 강제합니다.
[RequireComponent(typeof(OutlineVision))]
public class TrashObject : MonoBehaviour
{
    [Header("쓰레기 설정")]
    public PotionType trashType; // 이 쓰레기의 속성 (먼지, 기름 등)

    private OutlineVision outlineControl;

    private void Awake()
    {
        // 아웃라인 제어 컴포넌트 가져오기
        outlineControl = GetComponent<OutlineVision>();
    }

    /// 
    /// 포션이 폭발하며 이 함수를 호출합니다.
    /// 
    /// <param name="potionType">던져진 포션의 속성</param>
 
    public void CleanUp(PotionType potionType)
    {
        // 1. 속성 일치 확인
        if (this.trashType == potionType)
        {
            ProcessCleaning();
        }
        else
        {
            // 2. 쓰레기의 속성과 포션의 속성이 다르면 아웃라인으로 출력
            if (outlineControl != null)
            {
                outlineControl.EnableOutline();
            }
            Debug.Log($"속성이 맞지 않습니다. (쓰레기: {trashType} / 포션: {potionType})");
        }
    }

    private void ProcessCleaning()
    {
        Debug.Log($"{trashType} 쓰레기 청소 완료!");

        // 여기에 나중에 청소 파티클 생성 로직을 넣으면 좋습니다.

        // 최종적으로 오브젝트 삭제
        Destroy(gameObject);
    }
}