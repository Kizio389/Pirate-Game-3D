using UnityEngine;

public static class PlayerPrefsManager
{
    // Lưu và lấy thông tin máu
    public static void SetHealth(float value) => PlayerPrefs.SetFloat("Health", value);
    public static float GetHealth() => PlayerPrefs.GetFloat("Health", 100f);

    public static void SetMaxHealth(float value) => PlayerPrefs.SetFloat("MaxHealth", value);
    public static float GetMaxHealth() => PlayerPrefs.GetFloat("MaxHealth", 100f);

    // Lưu và lấy thông tin stamina
    public static void SetStamina(float value) => PlayerPrefs.SetFloat("Stamina", value);
    public static float GetStamina() => PlayerPrefs.GetFloat("Stamina", 50f);

    public static void SetMaxStamina(float value) => PlayerPrefs.SetFloat("MaxStamina", value);
    public static float GetMaxStamina() => PlayerPrefs.GetFloat("MaxStamina", 50f);

    public static void ResetData()
    {
        SetHealth(100f);
        SetMaxHealth(100f);
        SetStamina(50f);
        SetMaxStamina(50f);
    }
}
