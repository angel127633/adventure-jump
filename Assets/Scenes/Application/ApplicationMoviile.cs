using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationMovile : MonoBehaviour
{
    public static ApplicationMovile Instance;

    public static float horizontal;
    public static bool jump;
    public static bool attack;
    public static bool special;
    public static bool jumpEnabled = true;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        horizontal = 0;
        jump = false;
        attack = false;
        special = false;
    }

    public void MoveLeft(bool pressed)
    {
        horizontal = pressed ? -1 : 0;
    }

    public void MoveRight(bool pressed)
    {
        horizontal = pressed ? 1 : 0;
    }

    public void Jump()
    {
        if (!jumpEnabled)
            return;

        jump = true;
    }

    public static void SetJumpEnabled(bool estado)
    {
        jumpEnabled = estado;

        if (!estado)
            jump = false;
    }

    public void Attack()
    {
        attack = true;
    }

    public void Special()
    {
        special = true;
    }

}
