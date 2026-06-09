using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("💾 양방향 씬 전환 영구 보관함 (SO 직접 저장)")]
    public List<IngredientData> savedItems = new List<IngredientData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[DataManager] SO 마스터 보관소 고정 완료.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(IngredientData data)
    {
        if (data != null)
        {
            savedItems.Add(data);
            Debug.Log($"[DataManager] SO 저장 완료: {data.name}. 현재 개수: {savedItems.Count}개");
        }
    }

    public void RemoveItem(IngredientData data)
    {
        if (savedItems.Contains(data))
        {
            savedItems.Remove(data);
            Debug.Log($"[DataManager] SO 제거 완료: {data.name}. 잔여 개수: {savedItems.Count}개");
        }
    }
}