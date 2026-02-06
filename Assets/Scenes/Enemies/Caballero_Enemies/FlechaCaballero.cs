using UnityEngine;

public class FlechaCaballero : MonoBehaviour
{
    public float velocidad = 12f;
    public float dmg = 5f;
    public float stun = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;   // 🔒 BLOQUEA GRAVEDAD POR CÓDIGO
    }

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void Disparar(Vector2 dir)
    {
        dir = dir.normalized;

        rb.linearVelocity = dir * velocidad;   // ✅ LÍNEA RECTA PERFECTA

        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MoveCharacter fins = collision.gameObject.GetComponent<MoveCharacter>();
            MoveJack jake = collision.gameObject.GetComponent<MoveJack>();
            MoveGoku goku = collision.gameObject.GetComponent<MoveGoku>();

            if (fins != null)
            {
                fins.RecibirDano(dmg);
                fins.Paralizar(stun);
            }

            if (jake != null)
            {
                jake.RecibirDano(dmg);
            }

            if (goku != null)
            {
                goku.RecibirDano(dmg);
            }

        }

        if (collision.CompareTag("Wall") || collision.CompareTag("Suelo") || collision.CompareTag("Plataformas"))
        {
            Destroy(this.gameObject);
        }
    }
}