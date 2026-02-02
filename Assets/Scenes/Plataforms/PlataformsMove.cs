using UnityEngine;

public class PlataformsMove : MonoBehaviour
{
    public Vector2 direccion = Vector2.right; // derecha, arriba, etc
    public float distancia = 3f;
    public float velocidad = 2f;

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;
    private bool yendo = true;

    void Start()
    {
        posicionInicial = transform.position;
        posicionFinal = posicionInicial + (Vector3)(direccion.normalized * distancia);
    }

    void FixedUpdate()
    {
        Vector3 destino = yendo ? posicionFinal : posicionInicial;

        transform.position = Vector3.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.fixedDeltaTime
        );

        if (Vector3.Distance(transform.position, destino) < 0.02f)
        {
            yendo = !yendo; // 🔁 se devuelve
        }
    }

}
