using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuShop : MonoBehaviour
{
    public TextMeshProUGUI textoMonedas;

    void Start()
    {
        ActualizarMonedas();
    }

    void OnEnable()
    {
        ActualizarMonedas();
    }

    public void ActualizarMonedas()
    {
        if (textoMonedas == null)
        {
            Debug.LogError("❌ TextoMonedas NO asignado en MenuHUD");
            return;
        }

        textoMonedas.text = GameManager.Instance.monedasTotales.ToString();
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
