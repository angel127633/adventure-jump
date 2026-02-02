using UnityEngine;

public class TimeStopManager : MonoBehaviour
{
    public static TimeStopManager Instance;

    public bool tiempoDetenido = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // evita duplicados
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // hace que no se destruya al cambiar de escena
    }

    public void DetenerTiempo()
    {
        tiempoDetenido = true;

        foreach (var boss in FindObjectsOfType<BossStatus>())
            boss.OnTimeStop();
    }

    public void ReanudarTiempo()
    {
        tiempoDetenido = false;

        foreach (var boss in FindObjectsOfType<BossStatus>())
            boss.OnTimeResume();
    }

}

