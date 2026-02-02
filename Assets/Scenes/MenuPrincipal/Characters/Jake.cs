using UnityEngine;

public class Jake : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocityJake = 5f;

    [Header("Salto automático")]
    public float puntoSaltoX = 3f;
    public float fuerzaSalto = 7f;
    public float gravedadSalto = 1.5f;

    private Rigidbody2D rb;
    private Camera cam;
    private Animator animator;

    private float yInicial;
    private bool saltoIniciado = false;
    private bool estaCayendo = false;

    private float limiteFinal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cam = Camera.main;

        rb.gravityScale = 0f;
        yInicial = transform.position.y;

        CalcularLimiteFinal();
    }

    void FixedUpdate()
    {
        // correr siempre
        rb.linearVelocity = new Vector2(velocityJake, rb.linearVelocity.y);

        // iniciar salto EXACTAMENTE en el punto
        if (!saltoIniciado && transform.position.x >= puntoSaltoX)
        {
            // 🎬 animación ANTES del impulso
            animator.SetBool("Jump", true);
            saltoIniciado = true;

            rb.gravityScale = gravedadSalto;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // detectar caída
        if (saltoIniciado && rb.linearVelocity.y < 0)
        {
            estaCayendo = true;
        }

        // aterrizar
        if (estaCayendo && transform.position.y <= yInicial)
        {
            Aterrizar();
        }
    }

    void Aterrizar()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        transform.position = new Vector3(transform.position.x, yInicial, transform.position.z);

        animator.SetBool("Jump", false);

        estaCayendo = false;
    }

    void CalcularLimiteFinal()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        limiteFinal = cam.transform.position.x + camWidth + 1f;
    }

    void Update()
    {
        if (transform.position.x > limiteFinal)
        {
            gameObject.SetActive(false);
        }
    }
}

