using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class MoveJack : MonoBehaviour
{
    //variables miembros
    public float velocidad;
    public float salto;
    public float saltoMax;
    public LayerMask capaSuelo;
    public AudioClip audioClip;
    public float fuerzaGolpe;
    public float rangoAttack = 1f;
    public LayerMask enemiesLayer;
    public bool isAttack = false;

    [Header("Vida")]
    private float vidaMax = 16;
    private float vidaActual;
    public HeartAll heartJack;

    //variables miembros privadas
    private bool see = true;
    private Rigidbody2D rigBody2D;
    private BoxCollider2D boxCollider2D;
    private float saltoFaltantes;
    private bool estaEnElSuelo;
    private Animator animator;
    private bool puedeMoverse = true;
    public bool estaMuerto { private set; get; } = false;

    [Header("Ataque Especial - Jake")]
    public float cooldownJake = 8f;
    private bool puedeUsarSpecial = true;
    public bool invulnerable = false;
    public bool enSpecial = false;
    public float velocidadSpecial;

    [Header("Idle Pause")]
    public float tiempoParaIdlePause = 3f;
    private float contadorIdle = 0f;

    [Header("Hit Box")]
    public GameObject hitBox;

    [Header("Plataforms")]
    PlataformsMove plataformaActual;
    
    [Header("Plataforms")]
    private int saltosPendientes = 0;

    public int characterID = 3; // ID de Jake


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vidaActual = vidaMax;
        rigBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        // Si ya es tuyo, NO aparece
        if (!CharacterUnlocker.EstaDesbloqueado(characterID))
        {
            Destroy(gameObject);
        }

        VerificarScene();
        DisableHitbox();
    }

    // Update is called once per frame
    void Update()
    {
        if (enSpecial)
        {
            ProcesarMoveSpecial();
        }
        else
        {
            ProcesarMove();
        }
        GestionarIdlePause();
        ProcesarSalto();

        if (Input.GetKeyDown(KeyCode.J) || ApplicationMovile.attack)
        {
            ApplicationMovile.attack = false;
            ResetearIdleEspecial();
            isAttack = true;
            animator.SetTrigger("Attack");
        }

        if ((Input.GetKeyDown(KeyCode.L) || ApplicationMovile.special) && puedeUsarSpecial)
        {
            ApplicationMovile.special = false;
            ResetearIdleEspecial();
            isAttack = true;

            puedeUsarSpecial = false;

            // 🔥 ACTIVAR UI COOLDOWN
            SkillCooldownUI.Instance.ActivarCooldown();

            animator.SetTrigger("Special");
            StartCoroutine(CooldownJake());
        }
    }

    public void AsignarHUD(HeartAll hudVida)
    {
        heartJack = hudVida;
        InicializarBarra();
    }

    void GestionarIdlePause()
    {
        float inputMove = Input.GetAxis("Horizontal");

        // Si se mueve, se reinicia todo
        if (inputMove != 0f || !estaEnElSuelo)
        {
            ResetearIdleEspecial();
            return;
        }

        // Si está quieto
        if (!isAttack)
        {
            contadorIdle += Time.deltaTime;
        }
        else
        {
            contadorIdle = 0f;
            isAttack = false;
        }

        if (contadorIdle >= tiempoParaIdlePause && !isAttack)
        {
            int idleElegido = ElegirIdleConProbabilidad();

            animator.SetBool("IdlePausa", true);
            animator.SetInteger("IdleIndex", idleElegido);

            contadorIdle = 0f;
        }
    }

    void ResetearIdleEspecial()
    {
        animator.SetBool("IdlePausa", false);
        animator.SetInteger("IdleIndex", 0);
        contadorIdle = 0f;
    }

    int ElegirIdleConProbabilidad()
    {
        float random = Random.Range(0, 100); // 0–99

        if (random < 2)
            return 1; // Idle raro
        else if (random < 50)
            return 2; // Idle medio
        else
            return 3; // Idle comun
    }

    public void FinIdlePausa()
    {
        animator.SetBool("IdlePausa", false);
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
            velocidadSpecial = 10f;
            transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        }
        else
        {
            // Volver al modo normal
            velocidad = 8f;
            salto = 10f;
        }
    }


    public void InicializarBarra()
    {
        if (heartJack != null)
        {
            heartJack.Actualizar(vidaActual, vidaMax);
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

        bool acabaDeAterrizar =
            (enElSuelo && !estaEnElSuelo);

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
        if (invulnerable) return; // 🔥 CLAVE

        vidaActual -= dano;

        if (vidaActual < 0)
        {
            vidaActual = 0;
        }

        heartJack.Actualizar(vidaActual, vidaMax);

        Debug.Log("fins recibio dano" + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void StartSpecial()
    {
        enSpecial = true;
        invulnerable = true;

        // 🔥 frena cualquier movimiento previo
        rigBody2D.linearVelocity = Vector2.zero;
        rigBody2D.angularVelocity = 0f;

        EnableHitbox();
    }

    public void EndSpecial()
    {
        invulnerable = false;
        enSpecial = false;

        // frenar al salir
        rigBody2D.linearVelocity = Vector2.zero;

        DisableHitbox();
    }

    IEnumerator CooldownJake()
    {
        yield return new WaitForSeconds(cooldownJake);
        puedeUsarSpecial = true;
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
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        rigBody2D.linearVelocity = new Vector2(inputMove * velocidad, rigBody2D.linearVelocity.y);

        gestionarOrientacion(inputMove);
    }

    void ProcesarMoveSpecial()
    {
        // 🔥 durante la special SI se puede mover
        float inputMove = Input.GetAxisRaw("Horizontal") + ApplicationMovile.horizontal;

        // NO animación de correr
        animator.SetBool("isRunning", false);

        // control manual sin inercia
        rigBody2D.linearVelocity = new Vector2(
            inputMove * velocidadSpecial,
            0f
        );

        gestionarOrientacion(inputMove);
    }

    void gestionarOrientacion(float inputMove)
    {
        //si se cumple ejeute el codigo
        if ((see == true && inputMove < 0 || (see == false && inputMove > 0)))
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

    public void Morir()
    {
        Debug.Log("🔥 Jack murió — animación activada");

        puedeMoverse = false;
        rigBody2D.linearVelocity = Vector2.zero;

        animator.SetTrigger("deatch");

        GameManager.Instance.PlayerMurio();
    }

    public void EnableHitbox()
    {
        hitBox.SetActive(true);
        Debug.Log("inicio");
    }
    public void DisableHitbox()
    {
        hitBox.SetActive(false);
        Debug.Log("ya se desactivo");
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
