using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MoveGoku : MonoBehaviour
{
    //variables miembros
    public float velocidad;
    public LayerMask capaSuelo;
    public LayerMask capaPlataforms;
    public AudioClip audioClip;
    public float fuerzaGolpe;
    public LayerMask enemiesLayer;
    public GameObject kamehameha;
    public Transform puntoKamehameha;

    [Header("Vida")]
    private float vidaMax = 12;
    private float vidaActual;

    public HeartAll heartGoku;

    //variables privates
    private bool see = true;
    private Rigidbody2D rigBody2D;
    float gravedadOriginal;
    private BoxCollider2D boxCollider2D;

    [Header("Saltos")]
    private float saltoFaltantes;
    public float salto;
    public float saltoMax;

    [Header("Estado de salto")]
    private bool estaEnElSuelo;
    private bool estaEnPlataforms;

    [Header("Ataque Especial - Goku")]
    public float cooldownGoku = 60f;
    private bool puedeUsarSpecial = true;
    public bool invulnerable = false;

    private Animator animator;
    private bool puedeMoverse = true;
    private bool transformado = false;
    public bool heartGokuIniciado = false;
    private bool estaParalizado = false;
    private float velocidadAnimOriginal;
    PlataformsMove plataformaActual;

    [Header("En el aire")]
    private int saltosPendientes = 0;

    [Header("Time Stop Kame")]
    public GameObject teleportPointPrefab;
    public float radioTeleport = 4f;

    [Header("Teleport Validación")]
    public LayerMask capasBloqueadas;
    [SerializeField] float distanciaMinimaEntreTP = 2.5f;
    public float radioLibre = 0.5f; // tamaño del cuerpo de Goku

    private bool esperandoSeleccionTP = false;
    private bool seleccionRealizada = false;
    private Vector2 posicionFinal;
    private Coroutine cooldownCoroutine;

    private GameObject[] teleportPoints;

    public int characterID = 2; // ID de Goku


    void Start()
    {
        vidaActual = vidaMax;
        rigBody2D = GetComponent<Rigidbody2D>();
        gravedadOriginal = rigBody2D.gravityScale;
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        velocidadAnimOriginal = animator.speed;

        // Si ya es tuyo, NO aparece
        if (!CharacterUnlocker.EstaDesbloqueado(characterID))
        {
            Destroy(gameObject);
        }

        VerificarScene();
    }

    void Update()
    {
        if (TimeStopManager.Instance != null &&
        TimeStopManager.Instance.tiempoDetenido)
        {
            return; // bloquear TODO input
        }

        if (estaParalizado)
        {
            rigBody2D.linearVelocity = Vector2.zero;
            return;
        }

        ProcesarMove();
        ProcesarSalto();

        // RECARGA INSTANTÁNEA POR KILLS
        if(!puedeUsarSpecial && GameManager.Instance.enemigoskill >= 3)
{
            puedeUsarSpecial = true;

            //CANCELAR cooldown por tiempo
            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
                cooldownCoroutine = null;
            }

            if (SkillCooldownUI.Instance != null)
            {
                SkillCooldownUI.Instance.ForzarFinalizarCooldown();
            }

            Debug.Log("⚡ Goku recargó la especial por kills");
        }

        if ((Input.GetKeyDown(KeyCode.L) || ApplicationMovile.special) && !transformado && puedeUsarSpecial)
        {
            ApplicationMovile.special = false;

            puedeUsarSpecial = false;

            GameManager.Instance.enemigoskill = 0;

            if (SkillCooldownUI.Instance != null)
                SkillCooldownUI.Instance.ActivarCooldown();

            StartCoroutine(Tranformation());
            cooldownCoroutine = StartCoroutine(CooldownGoku());
        }

    }

    public void InicializarBarra()
    {
        if (heartGoku != null)
        {
            heartGoku.Actualizar(vidaActual, vidaMax);
        }
    }

    public void AsignarHUD(HeartAll hudVida)
    {
        heartGoku = hudVida;
        InicializarBarra();
    }

    void VerificarScene()
    {
        string escena = SceneManager.GetActiveScene().name;

        if (escena == "BossFirts")   // ← nombre exacto de tu escena
        {
            // Cambiar valores automáticamente
            velocidad = 16f;
            salto = 16f;
            rigBody2D.gravityScale = 3.5f;
            distanciaMinimaEntreTP = 8f;
            radioTeleport = 12f;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            // Volver al modo normal
            velocidad = 8f;
            salto = 10f;
        }
    }

    IEnumerator CooldownGoku()
    {
        yield return new WaitForSeconds(cooldownGoku);
        puedeUsarSpecial = true;
    }


    bool EstaEnElPlataforms()
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
            capaPlataforms
        );

        return hit.collider != null;
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
        bool enLaPlataforms = EstaEnElPlataforms();

        bool acabaDeAterrizar =
            (enElSuelo && !estaEnElSuelo) ||
            (enLaPlataforms && !estaEnPlataforms);

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
        estaEnPlataforms = enLaPlataforms;
    }

    void ProcesarMove()
    {
        if (!puedeMoverse)
            return;

        float inputMove = Input.GetAxis("Horizontal") + ApplicationMovile.horizontal;
        inputMove = Mathf.Clamp(inputMove, -1f, 1);
        bool corriendo = Mathf.Abs(inputMove) > 0.01f;

        // Control de animaciones
        animator.SetBool("isRunning", corriendo);
        animator.SetBool("isIdlePausa", !corriendo);

        // Movimiento físico
        rigBody2D.linearVelocity = new Vector2(inputMove * velocidad, rigBody2D.linearVelocity.y);

        gestionarOrientacion(inputMove);

    }

    public void RecibirDano(float dano)
    {

        if (invulnerable) return; // 🔥 CLAVE

        vidaActual -= dano;

        if(vidaActual < 0)
        {
            vidaActual = 0;
        }

        if (heartGoku != null)
        {
            heartGoku.Actualizar(vidaActual, vidaMax);
        }
        else
        {
            Debug.LogWarning("⚠️ Recibió daño pero HUD aún no está asignado");
        }

        Debug.Log("Goku recibió daño: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
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

        while (!estaEnElSuelo && !estaEnPlataforms)
        {
            //Que pare si no es asi
            yield return null;
        }

        puedeMoverse = true;
    }

    IEnumerator Tranformation()
    {
        transformado = true;
        rigBody2D.linearVelocity = Vector2.zero;
        puedeMoverse = false;

        animator.SetTrigger("Transformation");
        animator.SetBool("isKamehameha", true);

        Audiomanager.Instance.reproducirMusic(audioClip);

        yield return new WaitForSeconds(2f);

        puedeMoverse = true;

        Debug.Log("Goku se a transfomado");

    }

    public void Morir()
    {
        Debug.Log("🔥 Goku murió — animación activada");

        puedeMoverse = false;
        rigBody2D.linearVelocity = Vector2.zero;

        animator.SetTrigger("deatch");

        GameManager.Instance.PlayerMurio();
    }

    Vector2 ObtenerPosicionVacia(List<Vector2> posicionesExistentes)
    {
        for (int i = 0; i < 40; i++)
        {
            Vector2 randomPos =
                (Vector2)transform.position +
                Random.insideUnitCircle * radioTeleport;

            // 1️⃣ No colisiones
            if (Physics2D.OverlapCircle(randomPos, radioLibre, capasBloqueadas))
                continue;

            // 2️⃣ Distancia entre TP
            bool muyCerca = false;
            foreach (Vector2 pos in posicionesExistentes)
            {
                if (Vector2.Distance(randomPos, pos) < distanciaMinimaEntreTP)
                {
                    muyCerca = true;
                    break;
                }
            }

            if (!muyCerca)
                return randomPos;
        }

        // fallback seguro
        return transform.position;
    }

    public void IniciarKamehamehaConTiempo()
    {
        puedeMoverse = false;
        esperandoSeleccionTP = true;

        rigBody2D.linearVelocity = Vector2.zero;       // Detiene caída
        rigBody2D.gravityScale = 0f;

        // 1️⃣ Detener tiempo
        TimeStopManager.Instance.DetenerTiempo();

        // 2️⃣ Crear puntos TP
        teleportPoints = new GameObject[3];

        List<Vector2> posicionesTP = new List<Vector2>();

        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = ObtenerPosicionVacia(posicionesTP);

            posicionesTP.Add(pos);

            GameObject tp = Instantiate(
                teleportPointPrefab,
                pos,
                Quaternion.identity
            );

            tp.GetComponent<TeleportsPoints>().goku = this;
            teleportPoints[i] = tp;
        }

        // 3️⃣ Seguridad: si no elige, se reanuda en 2s
        StartCoroutine(TimeOutSeleccion());
    }

    public void Teletransportar(Vector2 posicion)
    {
        if (!esperandoSeleccionTP) return;

        esperandoSeleccionTP = false;

        // Mover Goku
        transform.position = posicion;

        // Limpiar puntos
        foreach (var tp in teleportPoints)
            Destroy(tp);

        // Reanudar tiempo
        TimeStopManager.Instance.ReanudarTiempo();

        // Lanzar Kamehameha ahora sí
        LanzarKamehameha();
    }

    IEnumerator TimeOutSeleccion()
    {

        yield return new WaitForSecondsRealtime(10f);

        if (!esperandoSeleccionTP) yield break;

        // ⏱️ No eligió nada → volver normal
        esperandoSeleccionTP = false;

        // Limpiar puntos
        foreach (var tp in teleportPoints)
            Destroy(tp);

        // Reanudar tiempo
        TimeStopManager.Instance.ReanudarTiempo();

        // Lanzar desde la MISMA posición
        LanzarKamehameha();
    }

    public void LanzarKamehameha()
    {
        StartSpecial();
        puedeMoverse = false;
        animator.SetBool("isKamehameha", true);
        // Crear el rayo
        var k = Instantiate(kamehameha, puntoKamehameha.position, Quaternion.identity);

        // Detectar hacia dónde mira Goku
        float dir = transform.localScale.x > 0 ? 1f : -1f;

        // Voltea el prefab según la dirección
        k.transform.localScale = new Vector3(dir * Mathf.Abs(k.transform.localScale.x),
                                             k.transform.localScale.y,
                                             k.transform.localScale.z);

        Kamehameha script = k.GetComponent<Kamehameha>();
        script.SetDireccion(see ? Vector2.right : Vector2.left);

        // 👉 Cuando el proyectil muera, volver a idle
        StartCoroutine(EsperarFinDelAtaque(k));
    }

    void RestaurarGravedad()
    {
        rigBody2D.gravityScale = gravedadOriginal;
    }
    IEnumerator EsperarFinDelAtaque(GameObject kame)
    {
        // Espera hasta que el prefab desaparezca
        while (kame != null)
            yield return null;

        EndSpecial();

        GameManager.Instance.enemigoskill = 0;

        animator.SetBool("isKamehameha",false);
        puedeMoverse = true;

        // ✔️ Permitir otra transformación
        transformado = false;
        RestaurarGravedad();
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

    public void StartSpecial()
    {
        invulnerable = true;

        // 🔥 frena cualquier movimiento previo
        rigBody2D.linearVelocity = Vector2.zero;
        rigBody2D.angularVelocity = 0f;

    }

    public void EndSpecial()
    {
        invulnerable = false;

        // frenar al salir
        rigBody2D.linearVelocity = Vector2.zero;

    }

    public void Paralizar(float duracion)
    {
        if (estaParalizado || vidaActual <= 0) return;

        StartCoroutine(ParalizarCoroutine(duracion));
    }

    IEnumerator ParalizarCoroutine(float duracion)
    {
        estaParalizado = true;

        // 🔒 Bloquear estados
        puedeMoverse = false;
        invulnerable = false;      // ⚠️ recibe daño igual
        transformado = false;

        // 🧊 Congelar animaciones
        animator.speed = 0f;

        // 🛑 Detener físicas
        rigBody2D.linearVelocity = Vector2.zero;
        rigBody2D.angularVelocity = 0f;

        yield return new WaitForSeconds(duracion);

        // 🔓 Restaurar
        animator.speed = velocidadAnimOriginal;
        puedeMoverse = true;

        estaParalizado = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioTeleport);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaMinimaEntreTP);
    }

}