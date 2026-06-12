using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("💾 양방향 씬 전환 영구 보관함")]
    public List<PotionRecipe> savedItems = new List<PotionRecipe>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(PotionRecipe data)
    {
        if (data != null)
        {
            savedItems.Add(data);
            Debug.Log($"[DataManager] 저장: {data.potionName}. 현재 {savedItems.Count}개");
        }
    }

    public void RemoveItem(PotionRecipe data)
    {
        if (savedItems.Contains(data))
        {
            savedItems.Remove(data);
            Debug.Log($"[DataManager] 제거: {data.potionName}. 잔여 {savedItems.Count}개");
        }
    }
}