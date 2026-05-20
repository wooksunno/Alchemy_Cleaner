using UnityEngine;
using DG.Tweening;

/// <summary>
/// 쓰레기 오브젝트 컴포넌트
/// PotionEffect → CleanUp() 호출 시 TrashResponseDatabase로 포션 반응 여부를 판정합니다.
/// </summary>
public class TrashObject : MonoBehaviour
{
    [Header("쓰레기 설정")]
    public TrashType trashType;

    [Header("페이드아웃 설정")]
    [SerializeField] private float duration = 0.5f;

    private OutlineVision _outlineControl;

    private void Awake()
    {
        _outlineControl = GetComponent<OutlineVision>();
    }

    /// <summary>
    /// PotionEffect에서 호출됩니다.
    /// DB를 통해 해당 포션이 이 쓰레기에 유효한지 판정합니다.
    /// </summary>
    public void CleanUp(PotionType potionType, TrashResponseDatabase database)
    {
        if (database.CanClean(trashType, potionType))
        {
            ProcessCleaning();
        }
        else
        {
            _outlineControl?.EnableOutline();
            //Debug.Log($"[TrashObject] 반응 없음 | 쓰레기: {trashType} / 포션: {potionType}");
        }
    }

    private void ProcessCleaning()
    {
        // 페이드아웃 후 삭제
        // 이거 해도 페이드아웃이 안되데? 그래서 머티리얼 설정을 바꾸라네? 그래서 바꾸니까 아웃라인이 안보임ㅋ
        // Renderer renderer = GetComponent<Renderer>();
        // renderer.material.DOFade(0f, duration).OnComplete(() => Destroy(gameObject));

        
        Debug.Log($"청소 완료 : {trashType}");
        Destroy(gameObject);
    }
}