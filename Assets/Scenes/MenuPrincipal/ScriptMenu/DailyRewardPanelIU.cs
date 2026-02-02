using UnityEngine;

public class DailyRewardPanelUI : MonoBehaviour
{
    void Awake()
    {
        if (PlayerPrefs.GetInt("DailyLoginCompletado", 0) == 1)
        {
            gameObject.SetActive(false); // ❌ NO Destroy
            return;
        }
    }

    void Start()
    {
        ActualizarPanel();
    }

    public void ActualizarPanel()
    {
        if (PlayerPrefs.GetInt("DailyLoginCompletado", 0) == 1)
        {
            gameObject.SetActive(false); // ❌ NO Destroy
            return;
        }

        if (DailyLoginManager.Instance == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(DailyLoginManager.Instance.PuedeReclamar());
    }

    public void OnClickReclamar()
    {
        DailyLoginManager.Instance.Reclamar();
        ActualizarPanel();
    }
}
