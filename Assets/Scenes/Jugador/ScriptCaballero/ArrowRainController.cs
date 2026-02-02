using UnityEngine;
using System.Collections;

public class ArrowRainController : MonoBehaviour
{
    [Header("Lluvia")]
    public GameObject flechaPrefab;
    public int cantidad = 10;
    public float radio = 2f;
    public float altura = 8f;
    public float delay = 0.4f;

    [Header("Física")]
    public float fuerza = 18f;
    public float gravedad = 9.8f;

    private bool activada;

    public void ActivarLluvia()
    {
        if (activada) return;
        activada = true;

        StartCoroutine(Lluvia());
    }

    IEnumerator Lluvia()
    {
        yield return new WaitForSeconds(delay);

        Vector3 objetivo = transform.position;

        for (int i = 0; i < cantidad; i++)
        {
            Vector2 offset = Random.insideUnitCircle * radio;
            Vector3 inicio = objetivo + new Vector3(offset.x, altura, 0);

            GameObject flecha = Instantiate(flechaPrefab, inicio, Quaternion.identity);

            ArrowProjectile proj = flecha.GetComponent<ArrowProjectile>();
            proj.esFlechaDeLluvia = true;
            proj.gravedad = gravedad;

            Vector3 direccion = (objetivo - inicio).normalized;

            proj.Disparar(inicio, direccion, fuerza);
        }
    }
}