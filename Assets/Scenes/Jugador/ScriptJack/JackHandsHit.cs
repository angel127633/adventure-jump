using UnityEngine;

public class JackHandsHit : MonoBehaviour
{
    public enum Tipomano { Paraliza, Lanza }
    public Tipomano tipomano;

    public float dmg;
    public float fuerzaLanzar;
    public float tiempoStun;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies"))
        {
            Enemigo enemies = collision.gameObject.GetComponent<Enemigo>();

            if (enemies == null) return;

            if (tipomano == Tipomano.Paraliza)
            {
                enemies.Paralizar(tiempoStun);
            }
            else if (tipomano == Tipomano.Lanza)
            {
                enemies.Lanzar(transform.position, fuerzaLanzar,dmg);
            }

        }
        if (collision.gameObject.CompareTag("Boss"))
        {
            BossStatus boss = collision.gameObject.GetComponent<BossStatus>();

            if (boss == null) return;

            if (tipomano == Tipomano.Paraliza)
            {
                boss.ParalizarBoss(tiempoStun);
            }
            else if (tipomano == Tipomano.Lanza)
            {
                boss.Lanzar(transform.position, fuerzaLanzar,dmg);
            }

        }
    }
}
