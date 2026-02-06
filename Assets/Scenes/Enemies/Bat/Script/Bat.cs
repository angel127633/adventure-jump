using UnityEngine;

public class Bat : MonoBehaviour
{
    [Header("Deteccion")]
    public Transform jugador;
    public float distanciaDeteccion;

    [Header("Ataque en Picada")]
    public float velocidadPicada = 8f;
    public float duracionPicada = 0.7f;
    public float cooldownAtaque = 2.5f;
    public float velocidadRegreso = 5f;

    [Header("Estadisticas")]
    public float vidaMax = 20;
    public float dmg = 3f;

    [Header("Aturdimiento")]
    public float tiempoAturdido = 1.5f;

    [Header("Time Stop")]
    private bool congelado = false;
    private bool estabaCongelado = false;

    private Vector2 velocidadGuardada;
    private float animatorSpeedGuardado;

    private bool aturdido;
    private float timerAturdido;
    private bool impactoRegistrado;

    private Vector2 posicionInicial;
    private Vector2 direccionAtaque;
    private Vector2 objetivoAtaque;

    private bool atacando;
    private float timerAtaque;
    private float timerCooldown;
    private bool regresando;
    private bool vueloListo;

    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                jugador = playerObj.transform;
        }

        posicionInicial = transform.position;
    }

    private void Update()
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

        if (aturdido)
        {
            timerAturdido -= Time.deltaTime;

            if (timerAturdido <= 0f)
            {
                aturdido = false;
                regresando = true;
                animator.SetBool("isStunned", false);
            }

            return;
        }

        MoverseHaciaJugador();

        if (!animator.GetBool("isFlying"))
            return;

        if (!vueloListo)
        {
            vueloListo = true;
            timerCooldown = cooldownAtaque;
        }

        if (!atacando && !regresando)
        {
            MirarAlJugador();
        }

        if (atacando)
        {
            AtaquePicada();
            return;
        }

        if (regresando)
        {
            RegresarAPosicion();
            return;
        }

        timerCooldown -= Time.deltaTime;

        if (timerCooldown <= 0f)
        {
            IniciarPicada();
        }
    }

    public void SetJugador(Transform nuevoJugador)
    {
        jugador = nuevoJugador;
        Debug.Log("mira aqui el jugador" + nuevoJugador);
    }

    public void MoverseHaciaJugador()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia > distanciaDeteccion)
        {
            vueloListo = false;
        }

        if (distancia <= distanciaDeteccion)
        {
            animator.SetBool("isSee",true);
            if (animator.speed >= 1f)
            {
                animator.SetBool("isCeil", true);
                if (animator.speed >= 1f)
                {
                    animator.SetBool("isFlying", true);
                }
            }
        }
    }

    void IniciarPicada()
    {
        if (jugador == null) return;

        atacando = true;
        impactoRegistrado = false;
        timerAtaque = 0f;

        posicionInicial = transform.position;

        // Guardamos la posición del jugador en ese momento
        objetivoAtaque = jugador.position;

        // Dirección exacta hacia el jugador
        direccionAtaque = (objetivoAtaque - posicionInicial).normalized;
        Vector2 dir = (posicionInicial - (Vector2)transform.position).normalized;

        VoltearEnDireccion(dir);

        animator.SetTrigger("isAttack"); // opcional
    }

    void MirarAlJugador()
    {
        if (jugador == null) return;

        float dx = jugador.position.x - transform.position.x;

        if (dx == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = dx > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void AtaquePicada()
    {
        timerAtaque += Time.deltaTime;

        rb.linearVelocity = direccionAtaque * velocidadPicada;

        if (timerAtaque >= duracionPicada)
        {
            TerminarAtaque();
        }
    }

    void TerminarAtaque()
    {
        atacando = false;
        regresando = true;

        rb.linearVelocity = Vector2.zero;
    }

    void VoltearEnDireccion(Vector2 dir)
    {
        if (dir.x == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = dir.x > 0
            ? Mathf.Abs(scale.x)
            : -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    void AjustarSobrePlataforma(Collider2D plataforma)
    {
        Collider2D batCol = GetComponent<Collider2D>();

        // Si el Bat está por encima de la plataforma
        if (batCol.bounds.min.y >= plataforma.bounds.center.y)
        {
            float nuevaY = plataforma.bounds.max.y + batCol.bounds.extents.y + 0.02f;
            transform.position = new Vector2(transform.position.x, nuevaY);
        }
    }

    void RegresarAPosicion()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            posicionInicial,
            velocidadRegreso * Time.deltaTime
        );

        // Voltearse mientras regresa
        Vector2 dir = (posicionInicial - (Vector2)transform.position).normalized;
        VoltearEnDireccion(dir);

        if (Vector2.Distance(transform.position, posicionInicial) < 0.05f)
        {
            transform.position = posicionInicial;
            regresando = false;
            timerCooldown = cooldownAtaque;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!atacando) return;
        if (impactoRegistrado) return;

        // 🧍 DAÑO AL JUGADOR
        if (other.CompareTag("Player"))
        {
            impactoRegistrado = true;

            MoveCharacter fins = other.GetComponent<MoveCharacter>();
            MoveGoku goku = other.GetComponent<MoveGoku>();
            MoveJack jake = other.GetComponent<MoveJack>();
            MoveCaballero caballero = other.GetComponent<MoveCaballero>();

            if (fins != null) fins.RecibirDano(dmg);
            if (goku != null) goku.RecibirDano(dmg);
            if (jake != null) jake.RecibirDano(dmg);
            if (caballero != null) caballero.RecibirDano(dmg);

            TerminarAtaque();
        }

        // CHOQUE CON PARED / SUELO / PLATAFORMAS
        if (other.CompareTag("Wall") || other.CompareTag("Suelo") || other.CompareTag("Plataformas"))
        {
            impactoRegistrado = true;

            AjustarSobrePlataforma(other);
            RecibirDano(2f);
            Aturdir();
        }
    }

    void Aturdir()
    {
        aturdido = true;
        atacando = false;
        regresando = false;

        animator.SetBool("isStunned", true);
        rb.linearVelocity = Vector2.zero;

        timerAturdido = tiempoAturdido;
    }

    public void Paralizar(float duracion)
    {
        if (vidaMax <= 0f) return;

        atacando = false;
        regresando = false;
        aturdido = true;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        animator.SetBool("isStunned", true);

        Invoke(nameof(QuitarParalisis), duracion);
    }

    void QuitarParalisis()
    {
        rb.simulated = true;
        aturdido = false;
        regresando = true;
        animator.SetBool("isStunned", false);
    }

    void Morir()
    {
        Debug.Log("Bat derrotado");
        Destroy(gameObject);
    }

    public void RecibirDano(float dmg)
    {
        Debug.Log("🔥 BAT RecibirDano LLAMADO", this);

        if (vidaMax <= 0f) return;

        vidaMax -= dmg;
        Debug.Log("vida restante: " + vidaMax);

        animator.Play("Hit", 0, 0f);
        Aturdir();

        if (vidaMax <= 0f)
            Morir();
    }

    void Congelar()
    {
        congelado = true;

        // Guardamos estado
        velocidadGuardada = rb.linearVelocity;
        animatorSpeedGuardado = animator.speed;

        // Congelamos todo
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        animator.speed = 0f;
    }

    void Descongelar()
    {
        congelado = false;

        rb.simulated = true;
        rb.linearVelocity = velocidadGuardada;
        animator.speed = animatorSpeedGuardado;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
    }
}
