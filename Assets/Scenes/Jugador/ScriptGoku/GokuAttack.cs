using UnityEngine;

public class GokuAttack : MonoBehaviour
{
    public Transform manoDerecha;
    public Transform manoIzquierda;
    public GameObject prefabKi;

    private Animator animator;
    private bool manoDerechaActiva = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) || ApplicationMovile.attack)
        {
            ApplicationMovile.attack = false;
            animator.SetTrigger("attack");
        }
    }

    // LLAMADO DESDE LA ANIMACIÓN
    public void LanzarKi()
    {
        // 1. Elegimos la mano
        Transform mano = manoDerechaActiva ? manoDerecha : manoIzquierda;

        // 2. Obtenemos la posición EXACTA de la mano
        Vector3 posicion = mano.position;

        // 3. Instanciamos el proyectil en esa posición
        GameObject ki = Instantiate(prefabKi, posicion, Quaternion.identity);

        // 4. Dirección depende de hacia dónde mira el personaje
        float direccion = transform.localScale.x > 0 ? 1 : -1;

        ki.GetComponent<KiProyectile>().SetDireccion(direccion);

        // Alternamos manos para el siguiente disparo
        manoDerechaActiva = !manoDerechaActiva;
    }

}
