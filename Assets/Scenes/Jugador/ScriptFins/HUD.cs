using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public static HUD Instance;

    [Header("Jugador")]
    public HeartAll vidaJugador;

    [Header("Boss")]
    public BossHeart bossCanvas;

    [Header("Cooldown UI")]
    public SkillCooldownUI skillCooldownUI;

    public TextMeshProUGUI puntos;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name);

        // ❌ Escenas SIN HUD
        if (scene.name == "SelectCharracter" || scene.name == "MenuPrincipal" || scene.name == "Tienda")
        {
            gameObject.SetActive(false);
            return;
        }

        // ✅ Escenas de juego
        gameObject.SetActive(true);
        StartCoroutine(ConfigurarHUDPorEscena(scene.name));
    }

    IEnumerator ConfigurarHUDPorEscena(string sceneName)
    {
        bool esBoss = sceneName == "BossFirts";

        // 🔁 Esperar hasta que el BossHeart exista
        while (bossCanvas == null)
        {
            bossCanvas = FindObjectOfType<BossHeart>();
            yield return null;
        }

        bossCanvas.gameObject.SetActive(esBoss);
        Debug.Log("BossCanvas encontrado y configurado");

        if (esBoss)
        {
            // 🔁 Esperar hasta que el boss exista
            BossStatus boss = null;
            while (boss == null)
            {
                boss = FindObjectOfType<BossStatus>();
                yield return null;
            }

            boss.AsignarHUD(bossCanvas);
            Debug.Log("Boss ya tiene su HUD");
        }

        StartCoroutine(AsignarJugadorActivo());
    }

    void Start()
    {
        StartCoroutine(AsignarJugadorActivo());
    }

    IEnumerator AsignarJugadorActivo()
    {
        while (true)
        {
            // Prioridad a Goku
            MoveGoku goku = FindObjectOfType<MoveGoku>();
            if (goku != null)
            {
                goku.AsignarHUD(vidaJugador);

                goku.teleportPointPrefab = TeleportsManager.Instance.teleportPointPrefab;
                skillCooldownUI.AsignarPersonaje(goku);
                Debug.Log("Goku ya tiene su heart" + vidaJugador);
                yield break;
            }

            // Si no hay Goku, usar Fins
            MoveCharacter fins = FindObjectOfType<MoveCharacter>();
            if (fins != null)
            {
                fins.AsignarHUD(vidaJugador);
                skillCooldownUI.AsignarPersonaje(fins);
                yield break;
            }

            MoveJack jack = FindObjectOfType<MoveJack>();
            if (jack != null)
            {
                jack.AsignarHUD(vidaJugador);
                SkillCooldownUI.Instance.AsignarPersonaje(jack);
                yield break;
            }

            MoveCaballero caballero = FindObjectOfType<MoveCaballero>();
            if (caballero != null)
            {
                caballero.AsignarHUD(vidaJugador);
                yield break;
            }

            yield return null;
        }
    }

    void Update()
    {
        puntos.text = GameManager.Instance.PuntosTatales.ToString();
    }
}

