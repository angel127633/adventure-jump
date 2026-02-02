using UnityEngine;

public class JackHands : MonoBehaviour
{
    public GameObject hand2;
    public GameObject hand3;

    public void MostrarManos2()
    {
        hand2.SetActive(true);
    }

    public void OcultarManos2()
    {
        hand2.SetActive(false);
    }
    public void MostrarManos3()
    {
        hand3.SetActive(true);
    }

    public void OcultarManos3()
    {
        hand3.SetActive(false);
    }
}
