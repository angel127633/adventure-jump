using UnityEngine;

public class SpikesMortal : MonoBehaviour
{
    [Header("Estadisticas")]
    public float dmg;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {

            MoveCharacter fins = collision.gameObject.GetComponent<MoveCharacter>();
            MoveGoku goku = collision.gameObject.GetComponent<MoveGoku>();
            MoveJack jack = collision.gameObject.GetComponent<MoveJack>();

            // 🔥 Intentar obtener cualquiera de los dos scripts de movimiento

            if (fins != null)
            {
                fins.RecibirDano(dmg);
            }
            else if (goku != null)
            {
                goku.RecibirDano(dmg);
            }
            else if (jack != null)
            {
                jack.RecibirDano(dmg);
            }
            else
            {
                Debug.LogWarning(" El jugador no tiene ni MoveCharacter ni MoveGoku adjunto.");
            }
        }

    }

}
