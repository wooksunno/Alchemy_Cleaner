using System.Collections;
using UnityEngine;

///////////////////////////////////////////////////////////////////////////////
///
///       바닥에 어질러진 쓰레기 책이 포션을 맞았을 때, 
///       물리 충돌을 끄고 화면에서 완전히 사라지게(On/Off) 만드는 기능을 담당합니다.
///       
///////////////////////////////////////////////////////////////////////////////

public class ToggleTrash : MonoBehaviour
{
    [Tooltip("포션 맞고 몇 초 뒤에 완전히 사라지게 할지 결정")]
    public float delayBeforeDisable = 0f;

    // 포션에 맞았을 때 호출될 함수
    public void DisableTrash()
    {
        StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        // 물리 충돌을 즉시 꺼서 연속 충돌 버그 방지 (사고 방지)
        if (TryGetComponent<Collider>(out var col)) col.enabled = false;

        // 약간의 딜레이를 주고 싶다면 설정 (0이면 즉시 통과)
        if (delayBeforeDisable > 0f)
        {
            yield return new WaitForSeconds(delayBeforeDisable);
        }

        // 오브젝트를 파괴하는 대신 비활성화 (On/Off)
        gameObject.SetActive(false);
    }
}