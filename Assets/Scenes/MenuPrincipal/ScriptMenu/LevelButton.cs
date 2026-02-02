using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public string sceneName;
    public int nivelID;
    public GameObject candado;

    private Button boton;

    void Awake()
    {
        boton = GetComponent<Button>();
    }

    void OnEnable() // 👈 CLAVE
    {
        ActualizarEstado();
    }

    void ActualizarEstado()
    {
        int nivelMax = PlayerPrefs.GetInt("NivelMax", 1);
        Debug.Log("NivelMax guardado: " + PlayerPrefs.GetInt("NivelMax", 1));

        bool desbloqueado = nivelID <= nivelMax;

        boton.interactable = desbloqueado;
        candado.SetActive(!desbloqueado);
    }

    public void SeleccionarNivel()
    {
        if (!boton.interactable) return;

        Debug.Log("La escena: " + sceneName);
        GameManager.Instance.SetNivelPendiente(sceneName);
        SceneManager.LoadScene("SelectCharracter");
    }
}