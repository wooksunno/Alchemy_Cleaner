using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashResponseDatabase", menuName = "Alchemy/Trash Response Database")]
public class TrashResponseDatabase : ScriptableObject
{
    [System.Serializable]
    public class TrashResponseEntry
    {
        public TrashType trashType;

        [Tooltip("이 쓰레기를 청소할 수 있는 포션 목록 (복수 지정 가능)")]
        public List<PotionType> effectivePotion = new();
    }

    [SerializeField]
    private List<TrashResponseEntry> entries = new();

    private Dictionary<TrashType, HashSet<PotionType>> _cache;

    private void OnEnable() => BuildCache();

#if UNITY_EDITOR
    private void OnValidate()
    {
        AutoGenerateEntries();
    }
#endif

    /// <summary>
    /// TrashType enum의 모든 값을 entries에 자동 추가
    /// </summary>
    private void AutoGenerateEntries()
    {
        if (entries == null)
            entries = new List<TrashResponseEntry>();

        // 현재 존재하는 trashType 목록
        HashSet<TrashType> existing = new();

        foreach (var entry in entries)
        {
            existing.Add(entry.trashType);
        }

        // enum 전체 순회
        foreach (TrashType type in Enum.GetValues(typeof(TrashType)))
        {
            if (existing.Contains(type))
                continue;

            entries.Add(new TrashResponseEntry
            {
                trashType = type,
                effectivePotion = new List<PotionType>
                {
                    PotionType.None
                }
            });
        }
    }

    private void BuildCache()
    {
        _cache = new Dictionary<TrashType, HashSet<PotionType>>();

        foreach (var entry in entries)
        {
            if (entry.effectivePotion == null)
                continue;

            _cache[entry.trashType] = new HashSet<PotionType>(entry.effectivePotion);
        }
    }

    public bool CanClean(TrashType trashType, PotionType potionType)
    {
        if (_cache == null)
            BuildCache();

        return _cache.TryGetValue(trashType, out var set)
               && set.Contains(potionType);
    }

    public IReadOnlyCollection<PotionType> GetEffectivePotion(TrashType trashType)
    {
        if (_cache == null)
            BuildCache();

        return _cache.TryGetValue(trashType, out var set)
            ? set
            : null;
    }
}