using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using static UnityEngine.Rendering.DebugUI;

public class MoveCharacter : MonoBehaviour
{
    //variables miembros
    public float velocidad;
    public float salto;
    public float saltoMax;
    public LayerMask capaSuelo;
    public AudioClip audioClip;
    public float fuerzaGolpe;
    public Transform puntoAtaque;
    public float rangoAttack = 1f;
    public LayerMask enemiesLayer;
    public bool isAttack = false;

    [Header("Vida")]
    private float vidaMax = 20;
    private float vidaActual;
    public HeartAll heartFins;

    [Header("Ataque Especial - Fins")]
    public GameObject jakeInvocadoPrefab;
    public Transform puntoInvocacion;
    public float cooldownFins = 8f;

    [Header("Estado - Paralizado")]
    public bool estaParalizado = false;

    private bool puedeInvocarJake = true;

    //variables miembros privadas
    private bool see = true;
    private Rigidbody2D rigBody2D;
    private BoxCollider2D boxCollider2D;
    private float saltoFaltantes;
    private bool estaEnElSuelo;
    private Animator animator;
    private bool puedeMoverse = true;
    public GameObject hitBox; // arrastra el objeto PlayerAttackHitbox
    public bool estaMuerto { private set; get; } = false;

    PlataformsMove plataformaActual;

    [Header("Saltos")]
    private int saltosPendientes = 0;

    public int characterID = 1; // ID de Fins


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vidaActual = vidaMax;
        rigBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        Debug.Log($"[DEBUG] Iniciando personaje: {gameObject.name}, ID: {characterID}, Desbloqueado: {CharacterUnlocker.EstaDesbloqueado(characterID)}");

        if (characterID != 1 && !CharacterUnlocker.EstaDesbloqueado(characterID))
        {
            Debug.LogWarning("Personaje destruido por desbloqueo: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        VerificarScene();
        DisableHitbox();
    }

    // Update is called once per frame
    void Update()
    {
        ProcesarMove();
        ProcesarSalto();

        if (Input.GetKeyDown(KeyCode.J) || ApplicationMovile.attack)
        {
            ApplicationMovile.attack = false;
            animator.SetTrigger("Attack");
        }

        if ((Input.GetKeyDown(KeyCode.L) || ApplicationMovile.special) && puedeInvocarJake)
        {
            ApplicationMovile.special = false;
            InvocarJake();
        }

    }

    void InvocarJake()
    {
        puedeInvocarJake = false;
        puedeMoverse = false;

        animator.SetTrigger("InvocarJake");

        SkillCooldownUI.Instance.ActivarCooldown();

        StartCoroutine(CooldownJake());
    }

    IEnumerator CooldownJake()
    {
        yield return new WaitForSeconds(cooldownFins);
        puedeInvocarJake = true;
    }

    public void CrearJakeInvocado()
    {
        Instantiate(
            jakeInvocadoPrefab,
            puntoInvocacion.position,
            Quaternion.identity
        );
    }

    public void FinInvocacion()
    {
        puedeMoverse = true;
    }


    public void AsignarHUD(HeartAll hudVida)
    {
        heartFins = hudVida;
        InicializarBarra();
    }

    void VerificarScene()
    {
        string escena = SceneManager.GetActiveScene().name;

        if (escena == "BossFirts")   // ← nombre exacto de tu escena
        {
            // Cambiar valores automáticamente
            velocidad = 15f;
            salto = 16f; 
            rigBody2D.gravityScale = 3.5f;
            transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        }
        else
        {
            // Volver al modo normal
            velocidad = 8f;
            salto = 10f;
        }
    }

    public void Paralizar(float duracion)
    {
        if (estaMuerto || estaParalizado) return;

        StartCoroutine(ParalizadoCoroutine(duracion));
    }

    IEnumerator ParalizadoCoroutine(float duracion)
    {
        estaParalizado = true;
        puedeMoverse = false;

        rigBody2D.linearVelocity = Vector2.zero;

        animator.SetBool("isRunning", false);

        yield return new WaitForSeconds(duracion);

        estaParalizado = false;
        puedeMoverse = true;
    }

    public void InicializarBarra()
    {
        if (heartFins != null)
        {
            heartFins.Actualizar(vidaActual, vidaMax);
        }
    }

    bool EstaEnElSuelo()
    {
        Vector2 origen = new Vector2(
            boxCollider2D.bounds.center.x,
            boxCollider2D.bounds.min.y
        );

        Vector2 tamaño = new Vector2(
            boxCollider2D.bounds.size.x * 0.6f,
            0.08f
        );

        RaycastHit2D hit = Physics2D.BoxCast(
            origen,
            tamaño,
            0f,
            Vector2.down,
            0.05f,
            capaSuelo
        );

        return hit.collider != null;
    }

    void ProcesarSalto()
    {
        bool enElSuelo = EstaEnElSuelo();

        bool acabaDeAterrizar =(enElSuelo && !estaEnElSuelo);

        // 🔹 Resetea saltos al aterrizar
        if (acabaDeAterrizar)
        {
            saltoFaltantes = saltoMax;
            saltosPendientes = 0; // limpiar inputs antiguos
        }

        // Detectar input de salto
        bool inputSalto = Input.GetKeyDown(KeyCode.Space) || ApplicationMovile.jump;

        if (inputSalto)
        {
            ApplicationMovile.jump = false; // resetear botón móvil

            // Solo guardar si no excede los saltos faltantes
            if (saltosPendientes < saltoFaltantes)
            {
                saltosPendientes++;
            }
            // Los inputs extra se ignoran automáticamente
        }

        // Procesar saltos pendientes
        if (saltosPendientes > 0)
        {
            rigBody2D.linearVelocity = new Vector2(rigBody2D.linearVelocity.x, 0);
            rigBody2D.AddForce(Vector2.up * salto, ForceMode2D.Impulse);

            saltoFaltantes--;
            saltosPendientes--;

            Audiomanager.Instance.reproducirMusic(audioClip);
        }

        // Guardar estado anterior
        estaEnElSuelo = enElSuelo;
    }


    public void DetenerMovimiento()
    {
        puedeMoverse = false;
        rigBody2D.linearVelocity = Vector2.zero;
    }

    public void RecibirDano(float dano)
    {
        vidaActual -= dano;

        if (vidaActual < 0)
        {
            vidaActual = 0;
        }

        heartFins.Actualizar(vidaActual, vidaMax);

        Debug.Log("fins recibio dano" + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void ProcesarMove()
    {
        if (!puedeMoverse)
        {
            return;
        }

        //logica de movimiento
        float inputMove = Input.GetAxis("Horizontal") + ApplicationMovile.horizontal;
        inputMove = Mathf.Clamp(inputMove, -1f, 1);

        if (inputMove != 0f)
        {
            animator.SetBool("isRunning",true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

            rigBody2D.linearVelocity = new Vector2(inputMove * velocidad, rigBody2D.linearVelocity.y);

        gestionarOrientacion(inputMove);
    }

    void gestionarOrientacion(float inputMove)
    {
        //si se cumple ejeute el codigo
        if ( (see == true && inputMove < 0 || (see == false && inputMove > 0)) )
        {
            //ejecuta codigo de volteado
            see = !see;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);

        }
    }

    public void AplicarGolpe()
    {
        puedeMoverse = false;
        Vector2 dirrecionGolpe;
        if (rigBody2D.linearVelocity.x > 0)
        {
            dirrecionGolpe = new Vector2(-1, 1);
        }
        else
        {
            dirrecionGolpe = new Vector2(1, 1);
        }
            rigBody2D.AddForce(dirrecionGolpe * fuerzaGolpe);

        StartCoroutine(EsperarParaPoderMoverse());
    }

    IEnumerator EsperarParaPoderMoverse()
    {
        //esperemos haber si esta en el suelo
        yield return new WaitForSeconds(0.1f);

        while (!estaEnElSuelo)
        {
            //Que pare si no es asi
            yield return null;
        }

        puedeMoverse = true;
    }
    public void EnableHitbox()
    {
        hitBox.SetActive(true);
    }

    public void DisableHitbox()
    {
        hitBox.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        if (puntoAtaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(puntoAtaque.position, rangoAttack);
    }

    public void Morir()
    {
        Debug.Log("Fins murió — animación activada");

        puedeMoverse = false;
        rigBody2D.linearVelocity = Vector2.zero;

        animator.SetTrigger("deatch");

        GameManager.Instance.PlayerMurio();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("PlataformaMove"))
        {
            plataformaActual = col.gameObject.GetComponentInParent<PlataformsMove>();
            transform.SetParent(plataformaActual.transform);

            // 🔒 DESACTIVAR SALTO MÓVIL
            ApplicationMovile.SetJumpEnabled(false);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("PlataformaMove"))
        {
            plataformaActual = null;

            // 🔓 Activar salto
            ApplicationMovile.SetJumpEnabled(true);

            // 🔥 Desparentar con seguridad
            StartCoroutine(DesparentarSeguro());
        }
    }

    IEnumerator DesparentarSeguro()
    {
        // Esperar un frame
        yield return null;

        if (transform != null)
        {
            transform.SetParent(null);
        }
    }

}