using UnityEngine;

/*
 * 
 * PotionBase.cs
 * 포션의 기본 속성을 정의하는 스크립트입니다.
 * 각 포션은 특정한 청소 효과를 가지고 있으며, 이를 PotionType Enum으로 구분합니다.
 * 이 클래스는 포션의 타입을 설정하는 데 사용됩니다.
 * 
 */


// 포션의 속성을 정의하는 Enum (임시 타입 추가)
public enum PotionType
{
    None,
    DustCleaner,   // 먼지 제거용
    StickyCleaner, // 끈적임 제거용
    OilCleaner     // 기름 제거용
}

// 인스펙터에서 포션의 타입을 설정할 수 있도록 하는 클래스
public class PotionBase : MonoBehaviour
{
    [Header("포션 설정")]
    public PotionType type;
}