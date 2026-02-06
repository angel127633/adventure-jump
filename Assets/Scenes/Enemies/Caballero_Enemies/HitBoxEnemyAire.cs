using UnityEngine;

public class HitBoxEnemyAire : MonoBehaviour
{

    public float dmg;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            MoveCharacter fins = collision.gameObject.GetComponent<MoveCharacter>();
            MoveJack jake = collision.gameObject.GetComponent<MoveJack>();
            MoveGoku goku = collision.gameObject.GetComponent<MoveGoku>();

            if (fins != null)
            {
                fins.RecibirDano(dmg);
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
    }
}
