using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    private static GameDataManager instance;
    
    public static GameDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameDataManager");
                instance = go.AddComponent<GameDataManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    public GameObject selectedCharacterPrefab { get; set; }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}