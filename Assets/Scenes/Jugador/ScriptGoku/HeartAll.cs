using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartAll : MonoBehaviour
{
    public Image barraFill;
    public TextMeshProUGUI textoVida;

    public void Actualizar(float vidaActual, float vidaMax)
    {
        barraFill.fillAmount = vidaActual / vidaMax;

        textoVida.text = $"{vidaActual}/{vidaMax}";
        Debug.Log($"Actualizar: fillAmount={barraFill.fillAmount}, vidaActual={vidaActual}");
    }

}
