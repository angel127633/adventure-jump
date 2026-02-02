using UnityEngine;

public class TituloIntro : MonoBehaviour
{
    public float velocidad = 2.5f;
    public Vector3 escalaFinal = Vector3.one;

    private bool mostrar = false;

    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (!mostrar) return;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            escalaFinal,
            Time.deltaTime * velocidad
        );
    }

    public void Mostrar()
    {
        mostrar = true;
    }
}

