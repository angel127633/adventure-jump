using UnityEngine;
using UnityEngine.SceneManagement;

public class KiProyectile : MonoBehaviour
{
    public float velocidadKi;
    public float tiempoVida;
    private float dmg = 5f;

    private float dirrecion;
    private Vector3 scaleInicial;
    private float scaleEscena = 0.5f;

    void Awake()
    {
        // Guarda la escala REAL del prefab (ajústala allí en Unity)
        scaleInicial = transform.localScale;
    }

    public void SetDireccion(float dir)
    {
        dirrecion = Mathf.Sign(dir);
    }

    void Start()
    {
        // ❌ YA NO TOQUES EL SCALE AQUÍ — SOLO destruyó tu flip
        VerificarScene();

        transform.localScale = new Vector3(
            scaleInicial.x * scaleEscena * dirrecion,
            scaleInicial.y * scaleEscena,
            scaleInicial.z
        );

        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.position += Vector3.right * dirrecion * velocidadKi * Time.deltaTime;
    }


    void VerificarScene()
    {
        string escena = SceneManager.GetActiveScene().name;

        if (escena == "BossFirts")   // ← nombre exacto de tu escena
        {
            // Cambiar valores automáticamente
            velocidadKi = 15f;
            scaleEscena = 1.5f;
        }
        else
        {
            // Volver al modo normal
            scaleEscena = 1f;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies"))
        {
            var enemies = collision.GetComponent<Enemigo>();
            if (enemies != null)
            {
                enemies.RecibirDano(dmg);
                Destroy(this.gameObject);
            }

            var bat = collision.GetComponent<Bat>();
            if (bat != null)
            {
                bat.RecibirDano(dmg);
                Destroy(this.gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            collision.GetComponent<BossStatus>()?.PerderVida(dmg);
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Boxes"))
        {
            var boxes = collision.GetComponent<BoxesClaim>();
            if (boxes != null)
            {
                boxes.CajaAbierta(1);
                Destroy(this.gameObject);
            }
        }
    }
}


