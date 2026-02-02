using UnityEngine;

public static class CharacterUnlocker
{
    public static bool EstaDesbloqueado(int id)
    {
        // 🟢 Finn siempre disponible
        if (id == 1) return true;

        return PlayerPrefs.GetInt("Character_" + id, 0) == 1;
    }

    public static void Desbloquear(int id)
    {
        PlayerPrefs.SetInt("Character_" + id, 1);
        PlayerPrefs.Save();

        Debug.Log("🔥 Personaje desbloqueado ID: " + id);
    }
}
