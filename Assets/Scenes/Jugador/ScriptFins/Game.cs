using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public void Jugar()
    {
        if (GameManager.Instance.nivelPendiente != "")
        {
            SceneManager.LoadScene(GameManager.Instance.nivelPendiente);
        }
        else
        {
            Debug.Log("No se encontro la escena");
        }
    }

    public void Volver()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
