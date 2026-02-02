using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static bool introYaMostrada = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
