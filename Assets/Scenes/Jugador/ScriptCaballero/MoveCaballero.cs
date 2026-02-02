using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using static UnityEngine.Rendering.DebugUI;

public class MoveCaballero : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad;
    
    [Header("Saltos")]
    public float saltos;
    public float saltosMax;
    private float saltosFaltantes;
    private float saltosPendientes = 0;
    public float fuerzaGolpe;

    [Header("Vida")]
    public HeartAll heartCaballero;
    private float vidaMax = 16;
    private float vidaActual;

    [Header("Sonido + Detencion")]
    public LayerMask capaSuelo;
    public AudioClip audioClip;
    public LayerMask enemiesLayer;

    [Header("HitBox - Attack")]
    public GameObject hitBoxSuelo;
    public GameObject hitBoxAire;

    private bool see = true;
    private Rigidbody2D rigBody2D;
    private BoxCollider2D boxCollider2D;
    private float saltoFaltantes;
    private bool estaEnElSuelo;
    private Animator animator;
    private bool puedeMoverse = true;

    public int characterID = 1; // ID de Caballerito


    void Start()
    {
        vidaActual = vidaMax;
        rigBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        if (!CharacterUnlocker.EstaDesbloqueado(characterID))
        {
            Debug.LogWarning("Personaje destruido por desbloqueo: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        DisableHitboxAire();
        DisableHitboxSuelo();
    }

    private void Update()
    {
        ProcesarMove();
        ProcesarSalto();

        animator.SetBool("isAire", estaEnElSuelo);

        if (Input.GetKeyDown(KeyCode.J) || ApplicationMovile.attack)
        {
            if (estaEnElSuelo)
            {
                ApplicationMovile.attack = false;
                animator.SetTrigger("isAttackSuelo");
            }
            else
            {
                ApplicationMovile.attack = false;
                animator.SetTrigger("isAttackAire");
            }
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
            animator.SetBool("isRunning", true);
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
        if ((see == true && inputMove < 0 || (see == false && inputMove > 0)))
        {
            //ejecuta codigo de volteado
            see = !see;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);

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

        bool acabaDeAterrizar = (enElSuelo && !estaEnElSuelo);

        // 🔹 Resetea saltos al aterrizar
        if (acabaDeAterrizar)
        {
            saltoFaltantes = saltosMax;
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
            rigBody2D.AddForce(Vector2.up * saltos, ForceMode2D.Impulse);

            saltoFaltantes--;
            saltosPendientes--;

            Audiomanager.Instance.reproducirMusic(audioClip);
        }

        // Guardar estado anterior
        estaEnElSuelo = enElSuelo;
    }

    public void InicializarBarra()
    {
        if (heartCaballero != null)
        {
            heartCaballero.Actualizar(vidaActual, vidaMax);
        }
    }

    public void AsignarHUD(HeartAll hudVida)
    {
        heartCaballero = hudVida;
        InicializarBarra();
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

    public void RecibirDano(float dano)
    {
        vidaActual -= dano;

        if (vidaActual < 0)
        {
            vidaActual = 0;
        }

        heartCaballero.Actualizar(vidaActual, vidaMax);

        Debug.Log("caballero recibio dano" + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Morir()
    {
        Debug.Log("Caballero murió — animación activada");

        puedeMoverse = false;
        rigBody2D.linearVelocity = Vector2.zero;

        animator.SetTrigger("deatch");

        GameManager.Instance.PlayerMurio();
    }

    public void EnableHitboxSuelo()
    {
        hitBoxSuelo.SetActive(true);
    }

    public void DisableHitboxSuelo()
    {
        hitBoxSuelo.SetActive(false);
    }

    public void EnableHitboxAire()
    {
        hitBoxAire.SetActive(true);
    }

    public void DisableHitboxAire()
    {
        hitBoxAire.SetActive(false);
    }
}
