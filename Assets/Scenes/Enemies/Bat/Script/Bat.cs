using UnityEngine;

public class Bat : MonoBehaviour
{
    [Header("Deteccion")]
    public Transform jugador;
    public float distanciaDeteccion;

    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void SetJugador(Transform nuevoJugador)
    {
        jugador = nuevoJugador;
        Debug.Log("mira aqui el jugador" + nuevoJugador);
    }

    public void MoverseHaciaJugador()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= distanciaDeteccion)
        {
            Vector2 direccion = (jugador.position - transform.position).normalized;
            animator.SetBool("isSee",true);
        }
    }

}
