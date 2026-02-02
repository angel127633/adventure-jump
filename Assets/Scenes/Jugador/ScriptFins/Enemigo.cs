using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemigo : MonoBehaviour
{
    [Header("tiempo de Ataque")]
    public float cooldownAttack;
    public float velocityEnemies;

    [Header("tiempo de Ataque")]
    private float dmg = 2f;

    [Header("Vida")]
    public float vidaMax = 2f;
    private float vidaActual;

    [Header("Disparo del Rayo")]
    public float distanciaRayo = 0.5f;

    [Header("Moviemiento")]
    public bool movingRight = true;
    private bool puedeAtacar = true;

    [Header("Sprites")]
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    [Header("Paredes")]
    public LayerMask paredLayer;
    private Animator anim;

    [Header("Detección de Bordes")]
    public LayerMask sueloLayer;
    public float distanciaSuelo = 0.2f;

    private Collider2D col;

    private bool estaParalizado = false;
    private bool fueLanzado = false;


    void Start()
    {
        vidaActual = vidaMax;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        VerificarScene();

    }

    void Update()
    {

        if (TimeStopManager.Instance.tiempoDetenido)
        {
            if (anim != null) anim.speed = 0;
            rb.linearVelocity = Vector2.zero;
            return;
        }
        else
        {
            if (anim != null)
            {
                anim.speed = TimeStopManager.Instance.tiempoDetenido ? 0f : 1f;
            }
        }

        if (estaParalizado) return;

        if (movingRight)
        {
            rb.linearVelocity  = new Vector2(velocityEnemies, rb.linearVelocity.y);
            transform.localScale = new Vector3(1,1,1);
        }else
        {
            rb.linearVelocity = new Vector2(-velocityEnemies, rb.linearVelocity.y);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        DetectarParedes();
        DetectarBorde();
    }

    void VerificarScene()
    {
        string escena = SceneManager.GetActiveScene().name;

        if (escena == "BossFirts")   // ← nombre exacto de tu escena
        {
            // Cambiar valores automáticamente
            transform.localScale = new Vector3(1.3f, 1.3f, 1f);
        }
        else
        {
            // Volver al modo normal
        }
    }

    public void Paralizar(float tiempo)
    {
        if (estaParalizado) return;
        StartCoroutine(StunCoroutine(tiempo));
    }

    void DetectarBorde()
    {
        if (col == null) return;

        Bounds bounds = col.bounds;

        float xOffset = movingRight ? bounds.extents.x : -bounds.extents.x;

        Vector2 origen = new Vector2(
            bounds.center.x + xOffset,
            bounds.min.y + 0.05f
        );

        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.down, distanciaSuelo, sueloLayer);

        Debug.DrawRay(origen, Vector2.down * distanciaSuelo, Color.green);

        if (hit.collider == null)
        {
            movingRight = !movingRight;
        }
    }

    IEnumerator StunCoroutine(float tiempo)
    {
        estaParalizado = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(tiempo);
        estaParalizado = false;
    }
    public void Lanzar(Vector2 origen, float fuerza,float dmg)
    {
        fueLanzado = true;

        Vector2 direccion = (transform.position - (Vector3)origen).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccion * fuerza, ForceMode2D.Impulse);
        RecibirDano(dmg);

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        // ⏸ TIME STOP
        if (TimeStopManager.Instance != null &&
            TimeStopManager.Instance.tiempoDetenido)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Wall") && fueLanzado)
        {
            RecibirDano(2f);
            Debug.Log("mas dano para ti" + vidaActual);
            fueLanzado = false;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (!puedeAtacar) return;
            puedeAtacar = false;

            MoveCharacter fins = collision.gameObject.GetComponent<MoveCharacter>();
            MoveGoku goku = collision.gameObject.GetComponent<MoveGoku>();
            MoveJack jack = collision.gameObject.GetComponent<MoveJack>();
            MoveCaballero caballero = collision.gameObject.GetComponent<MoveCaballero>();

            //  Cambiar color para indicar ataque
            Color color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;

            // 🔥 Intentar obtener cualquiera de los dos scripts de movimiento

            if (fins != null)
            {
                fins.RecibirDano(dmg);
                fins.AplicarGolpe();
            }
            else if (goku != null)
            {
                goku.RecibirDano(dmg);
                goku.AplicarGolpe();
            }
            else if (jack != null)
            {
                jack.RecibirDano(dmg);
                jack.AplicarGolpe();
            }
            else if (caballero != null)
            {
                caballero.RecibirDano(dmg);
                caballero.AplicarGolpe();
            }
            else
            {
                Debug.LogWarning(" El jugador no tiene ni MoveCharacter ni MoveGoku adjunto.");
            }

            //  Rehabilitar ataque luego del cooldown
            Invoke(nameof(ReactivarAttack), cooldownAttack);
        }
    }

    public void DetectarParedes()
    {
        Vector2 dirrecion = movingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirrecion,distanciaRayo,paredLayer);

        Debug.DrawRay(transform.position, dirrecion * distanciaRayo, Color.red);

        if (hit.collider != null)
        {
            movingRight = !movingRight;
        }
    }

    void ReactivarAttack()
    {
        if (TimeStopManager.Instance != null &&
        TimeStopManager.Instance.tiempoDetenido)
        {
            Invoke(nameof(ReactivarAttack), 0.1f);
            return;
        }

        puedeAtacar = true;

        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    public void RecibirDano(float dano)
    {
        vidaActual -= dano;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }
    void Morir()
    {
        GameManager.Instance.EnemigosDerrotados();
        Destroy(gameObject);
    }

}