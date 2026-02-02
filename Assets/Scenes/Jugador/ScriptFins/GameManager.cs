using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // 💎 gemas del nivel (temporales)
    public int gemasNivel = 0;

    // 🪙 monedas totales (permanentes)
    public int monedasTotales = 0;

    public int conversionGema = 50;

    public static GameManager Instance { get; private set; }
    public HUD hud;
    public int characterSelect = 0;
    public int enemigoskill = 0;
    public string nivelPendiente = "";

    private void Awake()
    {
        PlayerPrefs.SetInt("IntroMostrada", 0);

        if (!PlayerPrefs.HasKey("Inicializado"))
        {
            PlayerPrefs.SetInt("Character_1", 1); // Fins desbloqueado
            PlayerPrefs.SetInt("Inicializado", 1);
            PlayerPrefs.Save();
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // cargar monedas guardadas
            monedasTotales = PlayerPrefs.GetInt("Monedas", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hud = FindObjectOfType<HUD>();
        if (hud == null)
            Debug.Log("HUD no encontrado en la escena " + scene.name);
    }
    public int PuntosTatales
    {
        get { return gemasNivel; }
    }

    public void SumarGema(int cantidad)
    {
        gemasNivel += cantidad;
    }

    // 🔄 convertir gemas → monedas
    public void ConvertirGemasEnMonedas()
    {
        int monedasGanadas = gemasNivel * conversionGema;
        monedasTotales += monedasGanadas;

        PlayerPrefs.SetInt("Monedas", monedasTotales);
        PlayerPrefs.Save();

        Debug.Log($"💎 {gemasNivel} gemas → 🪙 {monedasGanadas} monedas");

        gemasNivel = 0; // 🔁 reset para siguiente nivel
    }

    public void SetNivelPendiente(string nombreNivel)
    {
        nivelPendiente = nombreNivel;
    }

    public void ReinicioNivel()
    {
        // Recargar la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SelectedCharacter(int index)
    {
        characterSelect = index;
        Debug.Log("Personaje seleccionado: " + index);
    }

    public void EnemigosDerrotados()
    {
        enemigoskill++;
        Debug.Log("Enemigos derrotados " + enemigoskill);
    }

    public void PlayerMurio()
    {
        StartCoroutine(EsperarYReiniciar());
    }

    public void SumarMonedas(int cantidad)
    {
        monedasTotales += cantidad;
        PlayerPrefs.SetInt("Monedas", monedasTotales);
        PlayerPrefs.Save();

        // 🔔 AVISAR AL HUD
        MenuHUD menuHUD = FindObjectOfType<MenuHUD>();
        if (menuHUD != null)
        {
            menuHUD.ActualizarMonedas();
        }
    }

    IEnumerator EsperarYReiniciar()
    {
        // ⏳ espera para que la animación se vea completa
        yield return new WaitForSeconds(1.4f);

        ReinicioNivel();
    }

    public void CompletarNivel(int nivelActual)
    {
        int nivelMax = PlayerPrefs.GetInt("NivelMax", 1);

        if (nivelActual >= nivelMax)
        {
            PlayerPrefs.SetInt("NivelMax", nivelActual + 1);
            PlayerPrefs.Save();
        }
    }

    public static implicit operator GameManager(MenuHUD v)
    {
        throw new NotImplementedException();
    }
}
