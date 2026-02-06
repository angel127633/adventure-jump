using System.Collections;
using UnityEngine;

public class CaballeroEnemyIA : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;

    [Header("Detección")]
    public float distanciaDeteccion = 8f;
    public float distanciaAtaque = 1.8f;

    [Header("Movimiento")]
    public float velocidad = 3.5f;
    public float fuerzaSalto = 6f;

    [Header("Ataques")]
    public float cooldownEntreAtaques = 1.2f;

    [Header("Ataque Flecha")]
    public float distanciaRetroceso = 4f;
    public float velocidadRetroceso = 7f;

    [Header("Time Stop")]
    public bool congelado = false;

    [Header("Estadisticas")]
    public float vidaMax = 100f;

    private Vector2 velocidadGuardada;
    private float gravedadGuardada;

    public GameObject hitboxSuelo;
    public GameObject hitboxAire;
    public Transform puntoFlecha;
    public GameObject flechaPrefab;

    private TipoAtaque ataqueActual = TipoAtaque.Suelo;

    private Rigidbody2D rb;
    private Animator animator;
    Coroutine rutinaActual;

    private bool atacando;
    private bool puedeAtacar = true;
    private bool esperandoCaida;
    private Vector3 escalaOriginal;
    private bool estabaCongelado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        DesactivarHitboxAire();
        DesactivarHitboxSuelo();
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        bool tiempoDetenido = TimeStopManager.Instance.tiempoDetenido;

        if (tiempoDetenido && !estabaCongelado)
        {
            Congelar();
            estabaCongelado = true;
            return;
        }

        if (!tiempoDetenido && estabaCongelado)
        {
            Descongelar();
            estabaCongelado = false;
        }

        if (congelado) return;

        if (esperandoCaida && rb.linearVelocity.y <= 0)
        {
            esperandoCaida = false;
            animator.SetTrigger("AttackAire");
        }

        if (jugador == null || atacando) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= distanciaDeteccion)
        {
            MirarJugador();

            if (distancia > distanciaAtaque)
            {
                CorrerHaciaJugador();
            }
            else if (puedeAtacar)
            {
                EjecutarAtaque();
            }
        }
        else
        {
            animator.SetBool("Run", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void CorrerHaciaJugador()
    {
        animator.SetBool("Run", true);

        float dir = Mathf.Sign(jugador.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * velocidad, rb.linearVelocity.y);
    }

    void MirarJugador()
    {
        float dir = jugador.position.x - transform.position.x;

        if (dir != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(dir) * Mathf.Abs(escalaOriginal.x),
                escalaOriginal.y,
                escalaOriginal.z
            );
        }
    }

    // ================= ATAQUES =================

    void EjecutarAtaque()
    {
        atacando = true;
        puedeAtacar = false;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("Run", false);

        switch (ataqueActual)
        {
            case TipoAtaque.Suelo:
                animator.SetTrigger("AttackSuelo");
                break;

            case TipoAtaque.Aire:
                esperandoCaida = true;
                rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
                break;

            case TipoAtaque.Flecha:
                StartCoroutine(AtaqueFlechaConRetroceso());
                break;
        }
    }

    public void SetJugador(Transform nuevoJugador)
    {
        jugador = nuevoJugador;
        Debug.Log("mira aqui el jugador" + nuevoJugador);
    }

    // ================= EVENTS =================
    // Llamado al FINAL de cada animación de ataque
    public void FinAtaque()
    {
        Debug.Log("✅ FinAtaque ejecutado");

        atacando = false;
        Invoke(nameof(ResetCooldown), cooldownEntreAtaques);
        SiguienteAtaque();
    }

    void ResetCooldown()
    {
        puedeAtacar = true;
    }

    void SiguienteAtaque()
    {
        if (ataqueActual == TipoAtaque.Suelo)
            ataqueActual = TipoAtaque.Aire;
        else if (ataqueActual == TipoAtaque.Aire)
            ataqueActual = TipoAtaque.Flecha;
        else
            ataqueActual = TipoAtaque.Suelo;
    }

    public void ActivarHitboxSuelo()
    {
        hitboxSuelo.SetActive(true);
    }

    public void DesactivarHitboxSuelo()
    {
        hitboxSuelo.SetActive(false);
    }

    public void ActivarHitboxAire()
    {
        hitboxAire.SetActive(true);
    }

    public void DesactivarHitboxAire()
    {
        hitboxAire.SetActive(false);
    }

    public void DispararFlecha()
    {
        GameObject flecha = Instantiate(
            flechaPrefab,
            puntoFlecha.position,
            Quaternion.identity
        );

        Debug.Log("🏹 Flecha instanciada en: " + flecha.transform.position);

        Vector2 dir = (jugador.position - puntoFlecha.position).normalized;

        FlechaCaballero fc = flecha.GetComponent<FlechaCaballero>();
        if (fc != null)
        {
            fc.Disparar(dir);
        }
    }

    IEnumerator AtaqueFlechaConRetroceso()
    {
        // Dirección contraria al jugador
        float dir = Mathf.Sign(transform.position.x - jugador.position.x);

        // Retroceso
        float distanciaRecorrida = 0f;

        while (distanciaRecorrida < distanciaRetroceso)
        {
            float movimiento = dir * velocidadRetroceso * Time.deltaTime;
            rb.linearVelocity = new Vector2(movimiento / Time.deltaTime, rb.linearVelocity.y);
            distanciaRecorrida += Mathf.Abs(movimiento);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        // Ejecuta animación de flecha
        animator.SetTrigger("AttackFlecha");
    }

    public void Congelar()
    {
        if (congelado) return;

        congelado = true;

        // Guarda estado
        velocidadGuardada = rb.linearVelocity;
        gravedadGuardada = rb.gravityScale;

        // Congela física
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.simulated = false;

        // Congela animaciones
        animator.speed = 0f;

        // Cancela ataques
        StopAllCoroutines();
        atacando = false;
        esperandoCaida = false;
    }

    public void RecibirDano(float dano)
    {
        vidaMax -= dano;

        if (vidaMax < 0)
        {
            vidaMax = 0;
        }

        Debug.Log("caballero recibio dano" + vidaMax);

        if (vidaMax <= 0)
        {
            Morir();
        }
    }

    public void Paralizar(float duracion)
    {
        Debug.Log("Se paralizo al enemigo");
        if (congelado) return;   // ⛔ no se mezcla con Time Stop
        Debug.Log("Se paralizo al enemigo");
        if (vidaMax <= 0) return;
        Debug.Log("Se paralizo al enemigo");
        StopAllCoroutines();

        Debug.Log("Se paralizo al enemigo");
        atacando = false;
        esperandoCaida = false;
        puedeAtacar = false;

        rb.linearVelocity = Vector2.zero;

        animator.speed = 0f;

        StartCoroutine(ParalisisRutina(duracion));
    }

    IEnumerator ParalisisRutina(float duracion)
    {
        yield return new WaitForSeconds(duracion);

        VolverARutinaNormal();
    }

    public void Lanzar(Vector2 origen, float fuerza, float dano)
    {
        if (congelado || vidaMax <= 0) return;

        Debug.Log("💥 Lanzamiento aplicado");

        if (rutinaActual != null)
            StopCoroutine(rutinaActual);

        atacando = false;
        esperandoCaida = false;
        puedeAtacar = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = true;

        Vector2 direccion = ((Vector2)transform.position - origen).normalized;
        rb.AddForce(direccion * fuerza, ForceMode2D.Impulse);

        RecibirDano(dano);
        animator.SetTrigger("Hit");

        rutinaActual = StartCoroutine(RecuperarTrasLanzamiento());
    }

    IEnumerator RecuperarTrasLanzamiento()
    {
        yield return new WaitForSeconds(0.6f);
        VolverARutinaNormal();
    }

    void VolverARutinaNormal()
    {
        atacando = false;
        esperandoCaida = false;
        puedeAtacar = true;

        animator.speed = 1f;
        rb.linearVelocity = Vector2.zero;

    }

    public void Morir()
    {
        Debug.Log("Caballero murió — animación activada");

        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("deatch");
    }

    public void Descongelar()
    {
        congelado = false;

        // Restaura física
        rb.simulated = true;
        rb.gravityScale = gravedadGuardada;
        rb.linearVelocity = velocidadGuardada;

        // Restaura animaciones
        animator.speed = 1f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
    }
}