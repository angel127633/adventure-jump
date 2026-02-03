using UnityEngine;

public class BoxesClaim : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite cajaRota;
    public int golpesMax = 5;

    public void CajaAbierta(int golpes)
    {
        golpesMax -= golpes;

        if (golpesMax <= 0)
        {
            spriteRenderer.sprite = cajaRota;
        }
    }
}
