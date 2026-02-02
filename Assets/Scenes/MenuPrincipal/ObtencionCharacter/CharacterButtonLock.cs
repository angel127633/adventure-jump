using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonLock : MonoBehaviour
{
    public int characterID;
    public GameObject candado;

    private Button boton;

    void Awake()
    {
        boton = GetComponent<Button>();
    }

    void OnEnable()
    {
        ActualizarEstado();
    }

    void ActualizarEstado()
    {
        bool desbloqueado = CharacterUnlocker.EstaDesbloqueado(characterID);

        boton.interactable = desbloqueado;
        candado.SetActive(!desbloqueado);
    }
}