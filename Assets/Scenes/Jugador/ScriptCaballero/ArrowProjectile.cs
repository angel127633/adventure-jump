using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Daño")]
    public float dmg = 5f; // daño que hace la flecha
    public float stum = 5f; // stum que hace la flecha

    public float gravedad = 9.8f;
    public LayerMask capasClavables;
    public float duracionVuelo = 1.2f;

    private Vector3 inicio;
    private Vector3 velocidadInicial;
    private float tiempo;
    private bool clavada;
    public bool esFlechaDeLluvia;

    private Vector3 posicionAnterior;
    private Collider2D col;

    [Header("Desaparición")]
    public float tiempoAntesDeDesaparecer = 1.5f;
    private ArrowRainController lluvia;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        lluvia = GetComponent<ArrowRainController>();
    }

    public void Disparar(Vector3 inicio, Vector3 direccion, float fuerza)
    {
        this.inicio = inicio;

        velocidadInicial =
            direccion.normalized * fuerza +
            Vector3.up * fuerza * 0.1f;

        tiempo = 0f;
        clavada = false;

        posicionAnterior = inicio;
        transform.position = inicio;

        col.enabled = true;
    }

    void Update()
    {
        if (clavada) return;

        tiempo += Time.deltaTime;

        Vector3 nuevaPosicion =
            inicio +
            velocidadInicial * tiempo +
            0.5f * Vector3.down * gravedad * tiempo * tiempo;

        Vector3 movimiento = nuevaPosicion - posicionAnterior;
        float distancia = movimiento.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(
            posicionAnterior,
            movimiento.normalized,
            distancia,
            capasClavables
        );

        if (hit.collider != null)
        {
            Clavar(hit);
            return;
        }

        transform.position = nuevaPosicion;
        posicionAnterior = nuevaPosicion;

        // 🔄 Rotación realista
        Vector3 velocidadActual =
            velocidadInicial + Vector3.down * gravedad * tiempo;

        float angulo = Mathf.Atan2(velocidadActual.y, velocidadActual.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    void Clavar(RaycastHit2D hit)
    {
        clavada = true;

        // 🔒 Fijar posición exacta
        transform.position = hit.point;

        // 🔥 DESACTIVAR COLISIÓN
        col.enabled = false;

        // 📌 Anclar al objeto
        transform.parent = hit.collider.transform;

        // ✅ Aplicar daño si el objeto tiene un componente con método RecibirDano
        var enemigo = hit.collider.GetComponent<Enemigo>();
        var boss = hit.collider.GetComponent<BossStatus>();
        var bat = hit.collider.GetComponent<Bat>();

        if (enemigo != null)
        {
            enemigo.RecibirDano(dmg);
            enemigo.Paralizar(stum);
        }
        if (boss != null)
        {
            boss.PerderVida(dmg);
            boss.ParalizarBoss(stum);
        }
        if (bat != null)
        {
            bat.RecibirDano(dmg);
        }

        if (!esFlechaDeLluvia && lluvia != null)
        {
            lluvia.ActivarLluvia();
        }

        Destroy(gameObject, tiempoAntesDeDesaparecer);
    }
}
