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
    }

    public void Disparar(Vector2 dir)
    {
        rb.linearVelocity = dir.normalized * velocidad;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<MoveCharacter>()?.RecibirDano(dmg);
            col.GetComponent<MoveCharacter>()?.Paralizar(stun);
        }

        Destroy(gameObject);
    }
}