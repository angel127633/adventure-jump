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

    private TipoAtaque ataqueActual = TipoAtaque.Suelo;

    private Rigidbody2D rb;
    private Animator animator;

    private bool atacando;
    private bool puedeAtacar = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
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
        transform.localScale = new Vector3(Mathf.Sign(dir), 1, 1);
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
                animator.SetTrigger("AttackAire");
                rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
                break;

            case TipoAtaque.Flecha:
                animator.SetTrigger("AttackFlecha");
                break;
        }
    }

    // ================= EVENTS =================
    // Llamado al FINAL de cada animación de ataque
    public void FinAtaque()
    {
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
}

