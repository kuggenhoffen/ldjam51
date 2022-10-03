using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    public GameObject helpPanel;

    // Start is called before the first frame update
    void Start()
    {
        helpPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
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
}
