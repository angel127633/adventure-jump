using UnityEngine;

public class SpawnerCharacter : MonoBehaviour
{

    public GameObject[] characters; // todos los prefabs de personajes
    public Transform spawnPoint;    // lugar donde aparecerá

    void Start()
    {
        // 1. Obtener el índice, asumiendo que GameManager.Instance ya existe.
        int index = GameManager.Instance.characterSelect;


        // 2. Verificar que el índice es válido para el array de prefabs.
        if (index >= 0 && index < characters.Length)
        {

            // 3. Instanciar el personaje en la posición del SpawnPoint.
            GameObject character = Instantiate(characters[index], spawnPoint.position, Quaternion.identity);
            Debug.Log("✅ Personaje instanciado: " + character.name);

            // 👇 Aquí es donde debes apuntar la cámara al nuevo personaje
            CameraTargetSetter.SetCameraTarget(character.transform);

            BossStatus boss = FindObjectOfType<BossStatus>();
            if (boss != null)
            {
                boss.SetJugador(character.transform);
            }
        }
        else
        {
            // 4. Si el índice no es válido (por ejemplo, 0 si no se seleccionó nada),
            // usar un personaje de respaldo o mostrar un error.
            Debug.LogWarning("Índice de personaje inválido o array vacío. Usando personaje 0 (por defecto).");
            // Opcional: Instanciar un personaje por defecto (ej. characters[0]) si el array no está vacío.
            if (characters.Length > 0)
            {
                Instantiate(characters[0], spawnPoint.position, Quaternion.identity);
            }
        }
    }

}
