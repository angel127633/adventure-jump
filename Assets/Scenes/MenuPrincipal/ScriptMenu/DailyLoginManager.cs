using UnityEngine;
using System;

public class DailyLoginManager : MonoBehaviour
{
    public static DailyLoginManager Instance;

    public int diaActual = 1; // 1 a 7
    private DateTime ultimaFecha;

    void Awake()
    {
        Debug.Log("🟢 Awake() llamado");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ Instance creada");
        }
        else
        {
            Debug.Log("❌ Instance duplicada, destruyendo");
            Destroy(gameObject);
            return;
        }

        CargarDatos();
    }

    void CargarDatos()
    {
        diaActual = PlayerPrefs.GetInt("LoginDia", 1);

        if (PlayerPrefs.HasKey("UltimaFechaLogin"))
        {
            ultimaFecha = DateTime.Parse(PlayerPrefs.GetString("UltimaFechaLogin"));
        }
        else
        {
            ultimaFecha = DateTime.MinValue;
        }
    }

    public bool PuedeReclamar()
    {
        if (PlayerPrefs.GetInt("DailyLoginCompletado", 0) == 1)
            return false;

        if (ultimaFecha == DateTime.MinValue)
            return true;

        return ultimaFecha.Date < DateTime.Today;
    }

    public void Reclamar()
    {
        if (!PuedeReclamar())
            return;

        DarRecompensa(diaActual);

        // 🔥 SI ES EL ÚLTIMO DÍA
        if (diaActual >= 7)
        {
            ultimaFecha = DateTime.Today;
            PlayerPrefs.SetString("UltimaFechaLogin", ultimaFecha.ToString());

            PlayerPrefs.SetInt("DailyLoginCompletado", 1);
            PlayerPrefs.Save();

            Debug.Log("🏁 Login diario COMPLETADO");
            return;
        }

        ultimaFecha = DateTime.Today;
        PlayerPrefs.SetString("UltimaFechaLogin", ultimaFecha.ToString());

        diaActual++;
        PlayerPrefs.SetInt("LoginDia", diaActual);
        PlayerPrefs.Save();
    }

    void DarRecompensa(int dia)
    {
        Debug.Log("💰 DarRecompensa() Día: " + dia);

        switch (dia)
        {
            case 1:
                GameManager.Instance.SumarMonedas(500);
                Debug.Log("🪙 +500 monedas");
                break;

            case 2:
                GameManager.Instance.SumarMonedas(1000);
                Debug.Log("🪙 +1000 monedas");
                break;

            case 3:
                GameManager.Instance.SumarMonedas(2000);
                Debug.Log("🪙 +2000 monedas");
                break;

            case 4:
                GameManager.Instance.SumarMonedas(4000);
                Debug.Log("🪙 +4000 monedas");
                break;

            case 5:
                GameManager.Instance.SumarMonedas(8000);
                Debug.Log("🪙 +8000 monedas");
                break;

            case 6:
                GameManager.Instance.SumarMonedas(10000);
                Debug.Log("🪙 +10000 monedas");
                break;

            case 7:
                CharacterUnlocker.Desbloquear(3);
                Debug.Log("🧍 Jake desbloqueado");
                break;
        }

        PlayerPrefs.SetInt("Monedas", GameManager.Instance.monedasTotales);
        Debug.Log("💾 Monedas totales: " + GameManager.Instance.monedasTotales);
    }

    [ContextMenu("🧪 Forzar Nuevo Día (TEST)")]
    public void ForzarNuevoDia()
    {
        ultimaFecha = DateTime.Today.AddDays(-1);
        PlayerPrefs.SetString("UltimaFechaLogin", ultimaFecha.ToString());
        PlayerPrefs.Save();

        Debug.Log("🧪 Día forzado: ahora se puede reclamar");
    }
}