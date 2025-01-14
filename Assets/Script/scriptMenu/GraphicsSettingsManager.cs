using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicsSettingsManager : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Toggle vsyncToggle;
    public Toggle fogToggle;
    public Slider globalVolumeSlider;
    public Slider musicVolumeSlider;

    private Resolution[] resolutions = new Resolution[]
    {
        new Resolution { width = 640, height = 480 },
        new Resolution { width = 1024, height = 768 },
        new Resolution { width = 1920, height = 1080 }
    };

    void Start()
    {
        // Lấy độ phân giải hiện tại của màn hình
        Resolution currentResolution = Screen.currentResolution;

        // Đặt độ phân giải và chế độ toàn màn hình
        Screen.SetResolution(currentResolution.width, currentResolution.height, FullScreenMode.FullScreenWindow);

        // Khởi tạo danh sách độ phân giải
        resolutionDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        int defaultResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionText = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(new TMP_Dropdown.OptionData(resolutionText));

            // Đặt độ phân giải mặc định là độ phân giải hiện tại của màn hình
            if (resolutions[i].width == currentResolution.width && resolutions[i].height == currentResolution.height)
            {
                defaultResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = defaultResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Tải cấu hình đã lưu
        LoadSettings();
    }

    public void ApplySettings()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        if (resolutionDropdown.value >= 0 && resolutionDropdown.value < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionDropdown.value];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        RenderSettings.fog = fogToggle.isOn;

        AudioListener.volume = globalVolumeSlider.value;

        Debug.Log("Settings applied!");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySetting", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionSetting", resolutionDropdown.value);
        PlayerPrefs.SetInt("VsyncSetting", vsyncToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("FogSetting", fogToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("GlobalVolume", globalVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);

        PlayerPrefs.Save();
        ApplySettings();
        Debug.Log("Settings saved!");
    }

    public void LoadSettings()
    {
        qualityDropdown.value = PlayerPrefs.GetInt("QualitySetting", 2);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionSetting", 0);
        vsyncToggle.isOn = PlayerPrefs.GetInt("VsyncSetting", 1) == 1;
        fogToggle.isOn = PlayerPrefs.GetInt("FogSetting", 1) == 1;
        globalVolumeSlider.value = PlayerPrefs.GetFloat("GlobalVolume", 1.0f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);

        ApplySettings();
    }

    public void ResetSettings()
    {
        qualityDropdown.value = 3;
        resolutionDropdown.value = 0;
        vsyncToggle.isOn = true;
        fogToggle.isOn = true;
        globalVolumeSlider.value = 1.0f;
        musicVolumeSlider.value = 1.0f;

        SaveSettings();
        Debug.Log("Settings reset to default!");
    }
}
