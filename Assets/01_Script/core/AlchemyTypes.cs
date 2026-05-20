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

public enum TrashType
{
    None = 0,

    // 일반 오염
    Dust                = 1,    // 먼지
    Slime               = 2,    // 점액
    StickyTrash         = 3,    // 끈적 쓰레기
    Mold                = 4,    // 곰팡이
    ToxicMud            = 5,    // 독성 진흙

    // 물리 구조물
    Debris              = 6,    // 잔해
    BrokenFurniture     = 7,    // 부서진 가구
    CrystalCorruption   = 8,    // 마력 결정 오염
    RootGrowth          = 9,    // 오염된 덩굴/뿌리

    // 생명체 계열
    ParasiteNest        = 10,    // 기생 둥지
    CorruptedPlant      = 11,    // 타락 식물
    GhostResidue        = 12,    // 유령 잔류물

    // 특수 오염
    HiddenCorruption    = 13,    // 숨겨진 오염
    CursedObject        = 14,    // 저주 물체
    ManaLeak            = 15     // 마력 누출
};