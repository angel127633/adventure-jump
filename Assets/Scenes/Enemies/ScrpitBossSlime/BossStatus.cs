using System.Collections;
using UnityEngine;

public class BossStatus : MonoBehaviour
{
    [Header("Estadísticas")]
    public BossHeart barraVida;

    public float vidaMax;
    private float vidaActual;
    public float dmgImpacto;

    [Header("Movimiento")]
    public float velocidad;
    public Transform jugador;
    public float distanciaDeteccion;

    [Header("Teletransporte")]
    public float alturaTeletransporte;
    public float tiempoEncogerse;
    public float tiempoAparecer;
    public float cooldownTP;
    private bool puedeTeletransportarse = true;

    [Header("Caída en Picada")]
    public float velocidadCaida;
    private bool atacandoDesdeArriba = false;
    public float radioImpacto;
    private bool cayendo;
    public LayerMask jugadorLayer;

    [Header("Invocar Slime")]
    public GameObject slimePequenoPrefab;

    [Header("Parálisis")]
    public bool estaParalizado = false;
    public float resistenciaStun = 0.4f; // El boss solo recibe 40% del stun

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider2D;

    private bool fase2 = false;
    private bool fase3 = false;
    private bool puedeSeguirlo = true;
    private bool fueLanzado = false;

    void Start()
    {
        vidaActual = vidaMax;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        InvokeRepeating("InvocarSlime", 3f, 5f);

        StartCoroutine(EsperarPrimerTeletransporte());
    }
    void Update()
    {
        if (TimeStopManager.Instance.tiempoDetenido)
        {
            rb.linearVelocity = Vector2.zero;
            animator.speed = 0f;
            return;
        }

        animator.speed = 1f;

        if (estaParalizado) return;

        MoverseHaciaJugador();

        if(puedeTeletransportarse && !atacandoDesdeArriba)
            AtaqueDesdeElCielo();


        if (atacandoDesdeArriba)
            DetectarGolpeEnCaida();
    }

    IEnumerator EsperarPrimerTeletransporte()
    {
        puedeTeletransportarse = false;
        yield return new WaitForSeconds(5f);
        puedeTeletransportarse = true;
    }

    public void OnTimeStop()
    {
        rb.linearVelocity = Vector2.zero;
        puedeSeguirlo = false;
        atacandoDesdeArriba = false;
        cayendo = false;
        animator.speed = 0f;

        CancelInvoke();
    }

    public void OnTimeResume()
    {
        puedeSeguirlo = true;
        animator.speed = 1f;

        InvokeRepeating("InvocarSlime", 3f, 5f);
    }

    public void InicializarBarra()
    {
        if (barraVida != null)
        {
            barraVida.Actualizar(vidaActual, vidaMax);
        }
    }

    public void AsignarHUD(BossHeart hudVida)
    {
        barraVida = hudVida;
        InicializarBarra();
    }

    public void ParalizarBoss(float tiempo)
    {
        if (estaParalizado) return;
        StartCoroutine(ParalizarCoroutine(tiempo * resistenciaStun));
    }

    IEnumerator ParalizarCoroutine(float tiempo)
    {
        estaParalizado = true;

        // Detener movimiento y ataques
        rb.linearVelocity = Vector2.zero;
        puedeSeguirlo = false;
        atacandoDesdeArriba = false;
        cayendo = false;

        animator.speed = 0f; // congela animaciones

        yield return new WaitForSeconds(tiempo);

        animator.speed = 1f;
        puedeSeguirlo = true;
        estaParalizado = false;
    }

    public void Lanzar(Vector2 origen, float fuerza,float dmg)
    {
        fueLanzado = true;

        Vector2 direccion = (transform.position - (Vector3)origen).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccion * fuerza, ForceMode2D.Impulse);
        PerderVida(dmg);
    }

    public void SetJugador(Transform nuevoJugador)
    {
        jugador = nuevoJugador;
        Debug.Log("mira aqui el jugador" + nuevoJugador);
    }

    // =========================================================
    //   Perder Vida
    // =========================================================

    public void PerderVida(float dmg)
    {
        vidaActual -= dmg;
        barraVida.Actualizar(vidaActual,vidaMax);

        Debug.Log("oeee" + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        VerificarFases();
    }

    // =========================================================
    //   MOVIMIENTO
    // =========================================================
    void MoverseHaciaJugador()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= distanciaDeteccion && puedeSeguirlo)
        {
            Vector2 direccion = (jugador.position - transform.position).normalized;

            rb.linearVelocity = new Vector2(direccion.x * velocidad, rb.linearVelocity.y);

            // Voltear sprite
            if (direccion.x > 0)
            {
                transform.localScale = new Vector3(12.5f, 15f, 1f);
            }
            else
            {
                transform.localScale = new Vector3(-12.5f, 15f, 1f);
            }
                
        }
    }

    // =========================================================
    //   ATAQUE DESDE EL CIELO (TP)
    // =========================================================
    public void AtaqueDesdeElCielo()
    {
        StartCoroutine(TeletransporteYCaida());
    }

    IEnumerator TeletransporteYCaida()
    {
        puedeTeletransportarse = false;
        puedeSeguirlo = false;

        Vector3 escalaOriginal = transform.localScale;

        // 1) Encogerse
        float t = 0;
        while (t < tiempoEncogerse)
        {
            transform.localScale = Vector3.Lerp(escalaOriginal, Vector3.zero, t / tiempoEncogerse);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.zero;

        // 2) Teleport encima del jugador
        Vector3 pos = jugador.position + Vector3.up * alturaTeletransporte;
        transform.position = pos;

        // 3) Agrandarse de nuevo
        t = 0;
        while (t < tiempoAparecer)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, escalaOriginal, t / tiempoAparecer);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = escalaOriginal;

        // 4) Caída en picada
        rb.gravityScale = velocidadCaida;
        rb.linearVelocity = new Vector2(0, -velocidadCaida);
        atacandoDesdeArriba = true;
        cayendo = true;

        // 5) Esperar a tocar suelo
        yield return new WaitUntil(() => EstaEnElSuelo());

        // Reset
        atacandoDesdeArriba = false;
        puedeSeguirlo = true;
        rb.gravityScale = 1f;

        yield return new WaitForSeconds(cooldownTP);
        puedeTeletransportarse = true;
    }

    // =========================================================
    //   DETECTAR GOLPE EN CAÍDA
    // =========================================================
    void DetectarGolpeEnCaida()
    {
        if (!cayendo) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, radioImpacto, jugadorLayer);

        if (hit != null)
        {
            MoveGoku goku = hit.GetComponent<MoveGoku>();
            MoveCharacter fins = hit.gameObject.GetComponent<MoveCharacter>();
            MoveJack jake = hit.gameObject.GetComponent<MoveJack>();

            if (goku != null)
                goku.RecibirDano(dmgImpacto);

            if (fins != null)
                fins.RecibirDano(dmgImpacto);

            if (jake != null)
                jake.RecibirDano(dmgImpacto);

            cayendo = false;
            rb.gravityScale = 2f;
        }
    }



    // =========================================================
    //   DETECTAR SUELO
    // =========================================================
    bool EstaEnElSuelo()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider2D.bounds.center,
            boxCollider2D.bounds.size,
            0f,
            Vector2.down,
            0.1f,
            LayerMask.GetMask("Suelo")
        );

        return hit.collider != null;
    }

    // =========================================================
    //   INVOCAR SLIME
    // =========================================================
    void InvocarSlime()
    {
        GameObject slimePequeno = Instantiate(slimePequenoPrefab, transform.position, Quaternion.identity);

        Vector2 direccion = (jugador.position - transform.position).normalized;

        if(direccion.x > 0)
        {
            slimePequeno.transform.localScale = new Vector3(5f, 5f, 1f);
        }
        else
        {
            slimePequeno.transform.localScale = new Vector3(-5f, 5f, 1f);
        }
     
    }

    void VerificarFases()
    {
        if (vidaActual <= vidaMax * 0.5f && !fase2)
        {
            fase2 = true;
            velocidad *= 1.5f;
            GetComponent<SpriteRenderer>().color = Color.red;
        }

        if (vidaActual <= vidaMax * 0.25f && !fase3)
        {
            fase3 = true;
            velocidad *= 1.8f;
            GetComponent<SpriteRenderer>().color = Color.cyan;
        }
    }

    void Morir()
    {
        Destroy(gameObject, 1f);
    }

    // =========================================================
    //   GIZMOS
    // =========================================================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioImpacto);
    }
}


