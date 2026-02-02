
using UnityEngine;

public class SelectCharacter : MonoBehaviour
{
    public void SelectCharacterGame(int index)
    {
        GameManager.Instance.SelectedCharacter(index);
    }

}
