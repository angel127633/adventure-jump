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

        posicionInicial = transform.position;
    }

    private void Update()
    {
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
        timerAtaque = 0f;

        posicionInicial = transform.position;

        // Guardamos la posición del jugador en ese momento
        objetivoAtaque = jugador.position;

        // Dirección exacta hacia el jugador
        direccionAtaque = (objetivoAtaque - posicionInicial).normalized;
        Vector2 dir = (posicionInicial - (Vector2)transform.position).normalized;

        VoltearEnDireccion(dir);

        animator.SetTrigger("attack"); // opcional
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
        // Solo hace daño durante la picada
        if (!atacando) return;

        if (other.CompareTag("Player"))
        {
            // Ejemplo de daño al jugador
            MoveCharacter fins = other.GetComponent<MoveCharacter>();
            MoveGoku goku = other.GetComponent<MoveGoku>();
            MoveJack jake = other.GetComponent<MoveJack>();
            MoveCaballero caballero = other.GetComponent<MoveCaballero>();

            if (fins != null)
            {
                fins.RecibirDano(dmg);
            }
            if (goku != null)
            {
                goku.RecibirDano(dmg);
            }
            if (jake != null)
            {
                jake.RecibirDano(dmg);
            }
            if (caballero != null)
            {
                caballero.RecibirDano(dmg);
            }
        }
    }

    public void RecibirDano(float dmg)
    {
        vidaMax -= dmg;

        if (vidaMax <= 0f)
        {
            Debug.Log("Fue Morido");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
    }
}
