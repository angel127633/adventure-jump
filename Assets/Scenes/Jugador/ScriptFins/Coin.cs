using UnityEngine;

public class Coin : MonoBehaviour
{
    public int valor = 1;
    public AudioClip audio;

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag("Player"))
        {
            GameManager.Instance.SumarGema(valor);
            Destroy(this.gameObject);
            Audiomanager.Instance.reproducirMusic(audio);
        }
    }

}
