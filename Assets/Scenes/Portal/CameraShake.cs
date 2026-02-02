using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 posicionOriginal;

    private void Awake()
    {
        Instance = this;
        posicionOriginal = transform.localPosition;
    }

    public void Shake(float duracion, float magnitud)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duracion, magnitud));
    }

    IEnumerator ShakeRoutine(float duracion, float magnitud)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            Vector2 offset = Random.insideUnitCircle * magnitud;
            transform.localPosition = posicionOriginal + (Vector3)offset;

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = posicionOriginal;
    }
}

