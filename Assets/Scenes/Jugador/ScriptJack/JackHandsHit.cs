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
            Bat bat = collision.gameObject.GetComponent<Bat>();

            if (enemies == null && bat == null) return;

            if(enemies != null)
            {
                if (tipomano == Tipomano.Paraliza)
                {
                    enemies.Paralizar(tiempoStun);
                }
                else if (tipomano == Tipomano.Lanza)
                {
                    enemies.Lanzar(transform.position, fuerzaLanzar, dmg);
                }
            } 

            if(bat != null)
            {
                if (tipomano == Tipomano.Paraliza)
                {
                    bat.RecibirDano(1.5f);
                }
                else if (tipomano == Tipomano.Lanza)
                {
                    bat.RecibirDano(1.5f);
                }
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

        if (collision.gameObject.CompareTag("Boxes"))
        {
            BoxesClaim boxes = collision.gameObject.GetComponent<BoxesClaim>();

            if (boxes == null) return;

            if (tipomano == Tipomano.Paraliza)
            {
                boxes.CajaAbierta(1);
            }
            else if (tipomano == Tipomano.Lanza)
            {
                boxes.CajaAbierta(1);
            }
        }
    }
}
