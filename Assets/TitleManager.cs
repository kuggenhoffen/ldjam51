using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{

    public GameObject helpPanel;
    public TMPro.TMP_Text sensitivityText;
    public Slider sensitivitySlider;
    public Toggle invertToggle;

    // Start is called before the first frame update
    void Start()
    {
        helpPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        float sens = PlayerPrefs.GetFloat("sensitivity", 100f);
        bool invert = PlayerPrefs.GetInt("invert", 0) == 1 ? true : false;
        OnSensitivityChanged(sens);
        OnMouseInvertChanged(invert);
        sensitivitySlider.value = sens;
        invertToggle.isOn = invert;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCloseButton()
    {
        helpPanel.SetActive(false);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnHelpButton()
    {
        helpPanel.SetActive(true);
    }

    public void OnSensitivityChanged(float value)
    {
        sensitivityText.SetText(string.Format("Mouse sensitivity: {0:F1}", value));
        PlayerPrefs.SetFloat("sensitivity", value);
        PlayerPrefs.Save();
    }

    public void OnMouseInvertChanged(bool value)
    {
        PlayerPrefs.SetInt("invert", value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
