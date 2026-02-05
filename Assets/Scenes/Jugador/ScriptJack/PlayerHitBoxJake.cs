using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBoxJake : MonoBehaviour
{
    public float dmg = 2f;
    public float tickRate = 0.4f;

    private Dictionary<MonoBehaviour, float> lastHit =
        new Dictionary<MonoBehaviour, float>();

    private void OnDisable()
    {
        lastHit.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        float time = Time.time;

        var enemigo = collision.GetComponentInParent<Enemigo>();
        if (enemigo != null)
        {
            TryDamage(enemigo, time, () => enemigo.RecibirDano(dmg));
            return;
        }

        var boss = collision.GetComponentInParent<BossStatus>();
        if (boss != null)
        {
            TryDamage(boss, time, () => boss.PerderVida(dmg));
        }

        var boxes = collision.GetComponent<BoxesClaim>();
        if (boxes != null)
        {
            TryDamage(boxes, time, () => boxes.CajaAbierta(1));
        }

        var bat = collision.GetComponent<Bat>();
        if (bat != null)
        {
            TryDamage(bat, time, () => bat.RecibirDano(dmg));
        }
    }

    void TryDamage(MonoBehaviour target, float time, System.Action action)
    {
        if (lastHit.TryGetValue(target, out float lastTime))
        {
            if (time - lastTime < tickRate) return;
        }

        action.Invoke();
        lastHit[target] = time;
    }
}

