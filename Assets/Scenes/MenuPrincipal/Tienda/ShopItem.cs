using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public int characterID;      // 1 Fins, 2 Goku, 3 Jake
    public int precio;

    public Button botonComprar;
    public TextMeshProUGUI textoBoton;
    public TextMeshProUGUI textoPrecio;

    void Start()
    {
        ActualizarUI();
    }

    public void Comprar()
    {
        if (CharacterUnlocker.EstaDesbloqueado(characterID))
            return;

        if (GameManager.Instance.monedasTotales >= precio)
        {
            GameManager.Instance.SumarMonedas(-precio);
            CharacterUnlocker.Desbloquear(characterID);
            ActualizarUI();
        }
        else
        {
            Debug.Log("❌ No tienes monedas suficientes");
        }
    }

    void ActualizarUI()
    {
        if (CharacterUnlocker.EstaDesbloqueado(characterID))
        {
            textoBoton.text = "OBTENIDO";
            botonComprar.interactable = false;
        }
        else
        {
            textoBoton.text = "OBTENER";
            textoPrecio.text = precio.ToString();
            botonComprar.interactable = true;
        }
    }
}
