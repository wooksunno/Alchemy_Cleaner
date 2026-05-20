/// <summary>
/// 프로젝트 전반에서 사용되는 연금술 관련 열거형 정의
/// PotionBase.cs의 기존 PotionType enum을 여기로 옮김
/// </summary>

public enum PotionType
{
    None = 0,

    // ── 기본 6종 포션 ──────────────────────────────
    Melter  = 1,  // 검은색  | 대형 오염물 용해
    Jelly   = 2,  // 주황색  | 쓰레기 응집
    Crusher = 3,  // 빨간색  | 잔해 폭발 분쇄
    Oracle  = 4,  // 파란색  | 시야 확장 (음용)
    Heal    = 5,  // 초록색  | 마법 식물 치유
    Purify  = 6,  // 보라색  | 최종 정화

    // ── 혼합 특수 포션 (시니어 이상 전용) ─────────
    BlackHole = 101,  // Jelly  + Melter  → 흡수 후 즉시 용해
    Radiance  = 102   // Oracle + Purify  → 감지 + 즉각 정화
}

public enum LicenseGrade
{
    Apprentice = 0,  // 견습생
    Junior     = 1,  // 주니어
    Senior     = 2,  // 시니어
    Master     = 3   // 마스터
}