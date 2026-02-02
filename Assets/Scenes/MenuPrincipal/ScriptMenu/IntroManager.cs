using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public GameObject[] personajes;
    public WorldsPanelUI iuManger;
    public TituloIntro titulo;

    private bool tituloMostrado = false;

    void Start()
    {
        // ❌ Ya se mostró en esta sesión → NO repetir
        if (SessionManager.introYaMostrada)
        {
            SaltarIntro();
        }
    }

    void Update()
    {
        if (tituloMostrado) return;

        bool todosFuera = true;

        foreach (GameObject p in personajes)
        {
            if (p.activeSelf)
            {
                todosFuera = false;
                break;
            }
        }

        if (todosFuera)
        {
            titulo.Mostrar();
            iuManger.MostrarUI();
            tituloMostrado = true;

            // ✅ Marcar como ya mostrada EN ESTA SESIÓN
            SessionManager.introYaMostrada = true;
        }
    }

    void SaltarIntro()
    {
        foreach (GameObject p in personajes)
        {
            p.SetActive(false);
        }

        titulo.Mostrar(); // o Mostrar()
        iuManger.MostrarUI();
        tituloMostrado = true;
    }
}
