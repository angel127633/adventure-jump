using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    public static FadeController instance;

    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // mantiene el Canvas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetAlpha(0f); // empieza transparente
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutIn(sceneName));
    }

    IEnumerator FadeOutIn(string sceneName)
    {
        // FADE A NEGRO
        yield return StartCoroutine(Fade(1f));

        // CARGAR ESCENA
        SceneManager.LoadScene(sceneName);

        yield return null; // espera 1 frame

        // FADE DESDE NEGRO
        yield return StartCoroutine(Fade(0f));
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
