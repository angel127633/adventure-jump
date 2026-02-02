using UnityEngine;
using UnityEngine.UI;

public class DailyRewardItem : MonoBehaviour
{
    public int dia;
    public GameObject candado;
    public GameObject check;

    void OnEnable()
    {
        ActualizarEstado();
    }

    public void ActualizarEstado()
    {
        if (DailyLoginManager.Instance == null)
        {
            Debug.LogWarning("⚠️ DailyLoginManager aún no existe");
            return;
        }

        if (candado == null || check == null)
        {
            Debug.LogError($"❌ Día {dia}: Candado o Check no asignado");
            return;
        }

        int diaActual = DailyLoginManager.Instance.diaActual;

        if (dia < diaActual)
        {
            candado.SetActive(false);
            check.SetActive(true);
        }
        else if (dia == diaActual)
        {
            candado.SetActive(false);
            check.SetActive(false);
        }
        else
        {
            candado.SetActive(true);
            check.SetActive(false);
        }
    }
}

