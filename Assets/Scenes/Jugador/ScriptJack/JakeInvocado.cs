using UnityEngine;

public class JakeInvocado : MonoBehaviour
{
    [Header("Impacto")]
    public float radioImpacto = 4f;
    public float dmg = 4f;
    public float tiempoStun = 2.5f;
    public LayerMask objetivosLayer;

    [Header("Camara")]
    public float duracionShake = 0.35f;
    public float fuerzaShake = 0.25f;

    private bool impactoEjecutado = false;

    // 🔥 EVENTO DE ANIMACIÓN
    public void ImpactoInicial()
    {
        if (impactoEjecutado) return;
        impactoEjecutado = true;

        // 📸 Sacudir cámara
        CameraShake.Instance.Shake(duracionShake, fuerzaShake);

        // ⚡ Paralizar todo en área
        Collider2D[] objetivos = Physics2D.OverlapCircleAll(
            transform.position,
            radioImpacto,
            objetivosLayer
        );

        foreach (Collider2D col in objetivos)
        {
            // Enemigos normales
            Enemigo enemigo = col.GetComponent<Enemigo>();
            if (enemigo != null)
            {
                enemigo.Paralizar(tiempoStun);
                enemigo.RecibirDano(dmg);
                continue;
            }

            // Bosses
            BossStatus boss = col.GetComponent<BossStatus>();
            if (boss != null)
            {
                boss.ParalizarBoss(tiempoStun); // menos tiempo pero sí afecta
                boss.PerderVida(dmg);
            }

            BoxesClaim boxes = col.GetComponent<BoxesClaim>();
            if(boxes != null)
            {
                boxes.CajaAbierta(1);
            }

            Bat bat = col.GetComponent<Bat>();
            if (bat != null)
            {
                bat.Paralizar(tiempoStun);
                bat.RecibirDano(dmg);
            }
        }
    }

    // 🔥 EVENTO FINAL DE ANIMACIÓN
    public void Desaparecer()
    {
        Destroy(gameObject);
    }

    // Solo visual
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioImpacto);
    }
}

