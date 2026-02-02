
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportsPoints : MonoBehaviour
{
    public MoveGoku goku;
    public CircleCollider2D circleCollider;
    public ParticleSystem particles;

    // Valores para el nivel normal
    public float normalColliderRadius = 0.4f;
    public float normalParticleSize = 0.5f;

    // Valores para el nivel del boss
    public float bossColliderRadius = 3f;
    public float bossParticleSize = 2f;

    void Start()
    {
        // Si no tienes referencias, las buscamos automáticamente
        if (circleCollider == null)
            circleCollider = GetComponent<CircleCollider2D>();

        if (particles == null)
            particles = GetComponent<ParticleSystem>();

        // Obtenemos la escena actual
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "BossFirts") // Cambia esto al nombre de tu escena del boss
        {
            // Ajustamos el collider
            if (circleCollider != null)
                circleCollider.radius = bossColliderRadius;

            // Ajustamos el tamaño de las partículas
            if (particles != null)
            {
                var main = particles.main;
                main.startSize = bossParticleSize;
            }
        }
        else
        {
            // Ajustamos valores normales
            if (circleCollider != null)
                circleCollider.radius = normalColliderRadius;

            if (particles != null)
            {
                var main = particles.main;
                main.startSize = normalParticleSize;
            }
        }
    }

    void OnMouseDown()
    {
        Debug.Log("ya se movfio" + TimeStopManager.Instance);
        Debug.Log("ya se movfio" + !TimeStopManager.Instance.tiempoDetenido);
        Debug.Log("ya se movfio" + goku);
        if (TimeStopManager.Instance == null) return;
        if (!TimeStopManager.Instance.tiempoDetenido) return;
        if (goku == null) return;

        goku.Teletransportar(transform.position);
        Debug.Log("ya se movfio");
    }
}



