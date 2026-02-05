using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamehameha : MonoBehaviour
{
    public float tiempoDeVida;

    [Header("Daño")]
    public float danoPorTick = 4f;
    public float tiempoEntreTicks = 0.2f;

    private Vector2 direccion;

    private HashSet<Enemigo> enemigosDentro = new HashSet<Enemigo>();
    private HashSet<Bat> batDentro = new HashSet<Bat>();
    private HashSet<BossStatus> bossDentro = new HashSet<BossStatus>();
    private HashSet<BoxesClaim> cajaDentro = new HashSet<BoxesClaim>();

    public void SetDireccion(Vector2 dir)
    {
        direccion = dir.normalized;
    }

    private void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 🔥 ENEMIGOS NORMALES
        if (collision.CompareTag("Enemies"))
        {
            Enemigo enemigo = collision.GetComponent<Enemigo>();
            if (enemigo != null && !enemigosDentro.Contains(enemigo))
            {
                enemigosDentro.Add(enemigo);
                StartCoroutine(DanoTickEnemigo(enemigo));
            }

            Bat bat = collision.GetComponent<Bat>();
            if (bat != null && !batDentro.Contains(bat))
            {
                batDentro.Add(bat);
                StartCoroutine(DanoTickBat(bat));
            }
        }

        // 🔥 BOSS
        if (collision.CompareTag("Boss"))
        {
            BossStatus boss = collision.GetComponent<BossStatus>();
            if (boss != null && !bossDentro.Contains(boss))
            {
                bossDentro.Add(boss);
                StartCoroutine(DanoTickBoss(boss));
            }
        }

        if (collision.CompareTag("Boxes"))
        {
            BoxesClaim boxes = collision.GetComponent<BoxesClaim>();
            if (boxes != null && !cajaDentro.Contains(boxes))
            {
                cajaDentro.Add(boxes);
                StartCoroutine(DanoTickCaja(boxes));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemies"))
        {
            Enemigo enemigo = collision.GetComponent<Enemigo>();
            if (enemigo != null)
                enemigosDentro.Remove(enemigo);

            Bat bat = collision.GetComponent<Bat>();
            if (bat != null)
                batDentro.Remove(bat);
        }

        if (collision.CompareTag("Boss"))
        {
            BossStatus boss = collision.GetComponent<BossStatus>();
            if (boss != null)
                bossDentro.Remove(boss);
        }

        if (collision.CompareTag("Boxes"))
        {
            BoxesClaim boxes = collision.GetComponent<BoxesClaim>();
            if (boxes != null)
                cajaDentro.Remove(boxes);
        }
    }

    IEnumerator DanoTickEnemigo(Enemigo enemigo)
    {
        while (enemigosDentro.Contains(enemigo))
        {
            enemigo.RecibirDano(danoPorTick);
            yield return new WaitForSeconds(tiempoEntreTicks);
        }
    }
    IEnumerator DanoTickBat(Bat bat)
    {
        while (batDentro.Contains(bat))
        {
            bat.RecibirDano(danoPorTick);
            yield return new WaitForSeconds(tiempoEntreTicks);
        }
    }

    IEnumerator DanoTickBoss(BossStatus boss)
    {
        while (bossDentro.Contains(boss))
        {
            boss.PerderVida(danoPorTick);
            yield return new WaitForSeconds(tiempoEntreTicks);
        }
    }
    IEnumerator DanoTickCaja(BoxesClaim boxes)
    {
        while (cajaDentro.Contains(boxes))
        {
            boxes.CajaAbierta(1);
            yield return new WaitForSeconds(tiempoEntreTicks);
        }
    }
}





