using UnityEngine;
using DG.Tweening;

public class WorldsPanelUI : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform panelWorlds;
    public GameObject btnWorlds;
    public GameObject btnLogins;
    public RectTransform btnSettings;
    public RectTransform btnShop;
    public RectTransform btnCoins;

    [Header("Animación")]
    public float tiempoAnim = 0.7f;
    public float settingsMoveX = -140f;

    private Vector2 panelPosInicial;
    private Vector2 panelPosFinal;
    private Vector2 settingsPosOriginalSetting;
    private Vector2 settingsPosOriginalShop;

    void Awake()
    {
        // 🛡️ Protección total
        if (panelWorlds == null || btnWorlds == null || btnSettings == null)
        {
            Debug.LogError("❌ Falta asignar referencias en WorldsPanelUI");
            enabled = false;
            return;
        }

        // Guardar posiciones
        panelPosFinal = panelWorlds.anchoredPosition;
        panelPosInicial = panelPosFinal + new Vector2(600f, 0);

        settingsPosOriginalSetting = btnSettings.anchoredPosition;
        settingsPosOriginalShop = btnShop.anchoredPosition;
    }

    void Start()
    {
        btnWorlds.SetActive(false);
        btnLogins.SetActive(false);
        btnSettings.gameObject.SetActive(false);
        btnCoins.gameObject.SetActive(false);
        btnShop.gameObject.SetActive(false);
        // Estado inicial
        panelWorlds.gameObject.SetActive(false);
        panelWorlds.anchoredPosition = panelPosInicial;
    }

    // 👉 ABRIR
    public void AbrirWorlds()
    {
        panelWorlds.gameObject.SetActive(true);
        btnWorlds.SetActive(false);

        panelWorlds.anchoredPosition = panelPosInicial;
        panelWorlds
            .DOAnchorPos(panelPosFinal, tiempoAnim)
            .SetEase(Ease.OutBack);

        btnSettings
            .DOAnchorPosX(settingsPosOriginalSetting.x + settingsMoveX, tiempoAnim)
            .SetEase(Ease.OutQuad);
        btnShop
            .DOAnchorPosX(settingsPosOriginalShop.x + settingsMoveX, tiempoAnim)
            .SetEase(Ease.OutQuad);
    }

    // 👉 CERRAR
    public void CerrarWorlds()
    {
        panelWorlds
            .DOAnchorPos(panelPosInicial, tiempoAnim)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                panelWorlds.gameObject.SetActive(false);
                btnWorlds.SetActive(true);
            });

        btnSettings
            .DOAnchorPos(settingsPosOriginalSetting, tiempoAnim)
            .SetEase(Ease.OutQuad);
        btnShop
            .DOAnchorPos(settingsPosOriginalShop, tiempoAnim)
            .SetEase(Ease.OutQuad);
    }

    public void MostrarUI()
    {
        btnWorlds.SetActive(true);
        btnLogins.SetActive(true);
        btnSettings.gameObject.SetActive(true);
        btnCoins.gameObject.SetActive(true);
        btnShop.gameObject.SetActive(true);
    }
}