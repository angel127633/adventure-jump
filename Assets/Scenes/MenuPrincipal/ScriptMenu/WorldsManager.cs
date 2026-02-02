using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class WorldsManager : MonoBehaviour
{
    public GameObject worldsPanel;
    public GameObject levelsPanel;
    public TMP_Text tituloMundo;

    public void AbrirMundo(string nombreMundo)
    {
        worldsPanel.SetActive(false);
        levelsPanel.SetActive(true);

        tituloMundo.text = nombreMundo;
    }

    public void VolverAMundos()
    {
        levelsPanel.SetActive(false);
        worldsPanel.SetActive(true);
    }

    public void IrAlaTienda()
    {
        SceneManager.LoadScene("Tienda");
    }
}

