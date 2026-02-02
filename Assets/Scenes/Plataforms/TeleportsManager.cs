using UnityEngine;

public class TeleportsManager : MonoBehaviour
{
    public static TeleportsManager Instance;

    public GameObject teleportPointPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
