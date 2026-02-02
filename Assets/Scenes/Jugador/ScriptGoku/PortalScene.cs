using UnityEngine;

public class PortalScene : MonoBehaviour
{
    public int nivelActual;      // ej: 1
    public int siguienteNivel;   // ej: 2
    public string escenaACargar; // ej: "BossFirts"

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameManager.Instance.ConvertirGemasEnMonedas();
            GameManager.Instance.CompletarNivel(nivelActual);
            FadeController.instance.FadeAndLoadScene(escenaACargar);
        }
    }
}

