using UnityEngine;

public class FlechaCaballero : MonoBehaviour
{
    public float velocidad = 12f;
    public float dmg = 5f;
    public float stun = 3f;
    public float tiempoClavada = 2.5f;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool yaImpacto = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
    }

    void Start()
    {
        Destroy(gameObject, 6f); // seguridad
    }

    public void Disparar(Vector2 dir)
    {
        dir = dir.normalized;
        rb.linearVelocity = dir * velocidad;

        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaImpacto) return;

        // 🔴 HIT A JUGADOR
        if (collision.CompareTag("Player"))
        {
            yaImpacto = true;

            // daño + stun
            if (collision.TryGetComponent(out MoveCharacter fins))
            {
                fins.RecibirDano(dmg);
                fins.Paralizar(stun);
            }

            if (collision.TryGetComponent(out MoveJack jake))
            {
                jake.RecibirDano(dmg);
                jake.Paralizar(stun);
            }

            if (collision.TryGetComponent(out MoveGoku goku))
            {
                goku.RecibirDano(dmg);
                goku.Paralizar(stun);
            }

            Clavarse(collision.transform);
        }

        // 🧱 HIT A ENTORNO
        if (collision.CompareTag("Wall") ||
            collision.CompareTag("Suelo") ||
            collision.CompareTag("Plataformas"))
        {
            yaImpacto = true;
            Clavarse(collision.transform);
        }
    }

    void Clavarse(Transform objetivo)
    {
        // detener flecha
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // evitar más colisiones
        col.enabled = false;

        // pegar al objetivo
        transform.SetParent(objetivo);

        // destruir luego
        Destroy(gameObject, tiempoClavada);
    }
}