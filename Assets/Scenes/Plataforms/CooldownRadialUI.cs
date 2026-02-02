using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public static SkillCooldownUI Instance;

    public Image radialImage;
    public Button skillButton;

    public MonoBehaviour personaje;

    private float tiempoRestante;
    private float cooldownTotal;
    private bool enCooldown;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        radialImage.fillAmount = 0;
        radialImage.gameObject.SetActive(false);
    }

    public void AsignarPersonaje(MonoBehaviour nuevoPersonaje)
    {
        personaje = nuevoPersonaje;
        Debug.Log("Cooldown asignado a: " + personaje.name);
    }

    public void ActivarCooldown()
    {
        if (enCooldown || personaje == null) return;

        cooldownTotal = ObtenerCooldownDelPersonaje();
        if (cooldownTotal <= 0) return;

        tiempoRestante = cooldownTotal;
        enCooldown = true;

        radialImage.fillAmount = 1;
        radialImage.gameObject.SetActive(true);

        skillButton.interactable = false;
    }

    void Update()
    {
        if (!enCooldown) return;

        tiempoRestante -= Time.deltaTime;
        radialImage.fillAmount = tiempoRestante / cooldownTotal;
        Debug.Log("hola vcokm" +  tiempoRestante);

        if (tiempoRestante <= 0)
        {
            enCooldown = false;
            radialImage.fillAmount = 0;
            radialImage.gameObject.SetActive(false);

            skillButton.interactable = true;
        }
    }

    public void ForzarFinalizarCooldown()
    {
        if (!enCooldown) return;

        enCooldown = false;
        tiempoRestante = 0f;

        radialImage.fillAmount = 0;
        radialImage.gameObject.SetActive(false);

        skillButton.interactable = true;

        Debug.Log("⚡ Cooldown finalizado instantáneamente");
    }

    float ObtenerCooldownDelPersonaje()
    {
        if (personaje is MoveCharacter fins)
            return fins.cooldownFins;

        if (personaje is MoveJack jake)
            return jake.cooldownJake;

        if (personaje is MoveGoku goku)
            return goku.cooldownGoku;

        return 0f;
    }
}