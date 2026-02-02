using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [SerializeField] private GameObject pausePanel;
    private bool isPaused;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 🔥 CLAVE
    }

    void Update()
    {
        if (!EsEscenaJugable()) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            TogglePause();
        }
    }

    bool EsEscenaJugable()
    {
        string escena = SceneManager.GetActiveScene().name;
        return escena.StartsWith("Worlds") || escena.StartsWith("Boss");
    }

    public void TogglePause()
    {
        if (isPaused)
            Reanudar();
        else
            Pausar();
    }

    void Pausar()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reanudar()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ReiniciarNivel()
    {
        Reanudar();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void VolverMenu()
    {
        GameManager.Instance.ConvertirGemasEnMonedas();
        Reanudar();
        SceneManager.LoadScene("MenuPrincipal");
    }
}