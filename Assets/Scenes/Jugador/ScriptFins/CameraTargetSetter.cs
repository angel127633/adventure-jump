using Unity.Cinemachine;
using UnityEngine;

public class CameraTargetSetter : MonoBehaviour
{
    // Esta función se puede llamar desde tu SpawnerCharacter
    public static void SetCameraTarget(Transform newTarget)
    {
        // 1. Encontrar la Virtual Camera
        CinemachineCamera vcam = FindObjectOfType<CinemachineCamera>();

        if (vcam != null)
        {
            // 2. Asignar el nuevo Transform a Follow y LookAt
            vcam.Follow = newTarget;
            vcam.LookAt = newTarget;

            Debug.Log("Cinemachine ahora sigue al nuevo objetivo: " + newTarget.name);
        }
        else
        {
            Debug.LogError("¡Cinemachine Virtual Camera no encontrada en la escena!");
        }
    }
}
