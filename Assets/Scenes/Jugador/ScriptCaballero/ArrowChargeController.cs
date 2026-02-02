using UnityEngine;

public class ArrowChargeController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform puntoDisparo;
    public LineRenderer indicador;
    public Animator animator;

    [Header("Carga")]
    public float tiempoCargaMax = 2f;
    public float fuerzaMax = 25f;

    [Header("Parábola")]
    public int puntos = 20;
    public float gravedad = 9.8f;
    public float duracionVuelo = 1.2f;

    [Header("Disparo")]
    public GameObject flechaPrefab;

    private float tiempoCarga;
    private bool cargando;

    void Start()
    {
        indicador.positionCount = puntos;
        indicador.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            cargando = true;
            tiempoCarga = 0f;
            indicador.enabled = true;
            animator.SetBool("isCharging", true);
        }

        if (cargando)
        {
            tiempoCarga += Time.deltaTime;
            float porcentaje = Mathf.Clamp01(tiempoCarga / tiempoCargaMax);
            float fuerza = porcentaje * fuerzaMax;

            DibujarParabola(fuerza);

        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            cargando = false;
            indicador.enabled = false;

            float porcentaje = Mathf.Clamp01(tiempoCarga / tiempoCargaMax);
            float fuerza = porcentaje * fuerzaMax;

            // Detectar dirección correcta
            Vector3 direccionDisparo = puntoDisparo.right;
            if (transform.localScale.x < 0)
                direccionDisparo = -puntoDisparo.right;

            GameObject flecha = Instantiate(
                flechaPrefab,
                puntoDisparo.position,
                puntoDisparo.rotation
            );

            ArrowProjectile flechaScript = flecha.GetComponent<ArrowProjectile>();
            flechaScript.gravedad = gravedad;
            flechaScript.duracionVuelo = duracionVuelo;

            flechaScript.Disparar(
                puntoDisparo.position,
                direccionDisparo,
                fuerza
            );

            animator.SetBool("isCharging", false);
        }
    }

    void DibujarParabola(float fuerza)
    {
        Vector3 inicio = puntoDisparo.position;

        // En DibujarParabola()
        Vector3 direccionHorizontal = puntoDisparo.right;

        // Detectar si el personaje está mirando izquierda
        if (transform.localScale.x < 0)
        {
            direccionHorizontal = -puntoDisparo.right;
        }

        Vector3 velocidadInicial =
            direccionHorizontal * fuerza +
            Vector3.up * fuerza * 0.1f;


        for (int i = 0; i < puntos; i++)
        {
            float t = i / (float)(puntos - 1);
            float tiempo = t * duracionVuelo;

            Vector3 posicion =
                inicio +
                velocidadInicial * tiempo +
                0.5f * Vector3.down * gravedad * tiempo * tiempo;

            indicador.SetPosition(i, posicion);
        }
    }
}