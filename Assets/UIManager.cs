using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public TMPro.TMP_Text tooltipText;
    public TMPro.TMP_Text toastText;
    public TMPro.TMP_Text timerText;
    private const float toastTime = 5f;
    private float toastTimer = 0f;
    private float teleportTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        HideTooltip();
    }

    // Update is called once per frame
    void Update()
    {
        if (toastTimer > 0f) {
            toastTimer -= Time.deltaTime;
        }
        else {
            toastText.gameObject.SetActive(false);
            toastText.SetText("");
        }

        timerText.SetText(teleportTimer.ToString("F2"));
    }

    public void ShowTooltip(string text)
    {
        //Debug.Log("Tooltip " + text);
        tooltipText.SetText(text);
        tooltipText.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipText.gameObject.SetActive(false);
    }

    public void ShowToastTip(string text)
    {
        Debug.Log("toast " + text);
        if (text != toastText.text) {
            toastText.SetText(text);
            toastText.gameObject.SetActive(true);
            toastTimer = toastTime;
        }
    }

    public void SetTeleportTimer(float teleTimer)
    {
        teleportTimer = teleTimer;
    }
}
