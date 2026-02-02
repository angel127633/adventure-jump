using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    public float dmg;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hitbox tocó a: " + collision.name);

        if (collision.CompareTag("Enemies"))
        {
            var enemigo = collision.GetComponent<Enemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDano(dmg);
            }
        }

        if (collision.CompareTag("Boss"))
        {
            var boss = collision.GetComponent<BossStatus>();
            if (boss != null)
            {
                boss.PerderVida(dmg);
            }
        }
    }
}
