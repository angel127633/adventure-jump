using UnityEngine;

public class Goku : MonoBehaviour
{
    public float velocityGoku = 5f;
    public bool movingRight = true;

    private Rigidbody2D rb;
    private Camera cam;

    private float limiteFinal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        CalcularLimiteFinal();
    }

    void Update()
    {
        float dir = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * velocityGoku, rb.linearVelocity.y);

        CheckFinalPantalla();
    }

    void CalcularLimiteFinal()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // SOLO el borde al que deben llegar para desaparecer
        limiteFinal = movingRight
            ? cam.transform.position.x + camWidth + 1f
            : cam.transform.position.x - camWidth - 1f;
    }

    void CheckFinalPantalla()
    {
        if (movingRight && transform.position.x > limiteFinal)
        {
            gameObject.SetActive(false);
        }
        else if (!movingRight && transform.position.x < limiteFinal)
        {
            gameObject.SetActive(false);
        }
    }
}



