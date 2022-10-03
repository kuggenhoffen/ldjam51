using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackout : MonoBehaviour
{

    float duration;
    bool isBlackout = true;
    float timer = 0f;
    Image image;
    public TMPro.TMP_Text endText;
    bool showEndscreen = false;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        showEndscreen = false;
        endText.gameObject.SetActive(false);
        SetInitial(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0f) {
            timer -= Time.deltaTime;
            if (isBlackout) {
                color.a =  Mathf.Lerp(1f, 0f, timer / duration);
            }
            else {
                color.a =  Mathf.Lerp(0f, 1f, timer / duration);
            }
        }
        else {
            if (!isBlackout) {
                gameObject.SetActive(false);
            }
            else {
                if (showEndscreen) {
                    endText.gameObject.SetActive(true);
                }
            }
            SetInitial(isBlackout);
        }
        image.color = color;
    }

    public void SetInitial(bool blackout)
    {
        color = Color.black;
        color.a = blackout ? 1f : 0f;
    }

    public void SetBlackout(bool blackout, float changeDuration)
    {
        SetInitial(!blackout);
        isBlackout = blackout;
        duration = changeDuration;
        timer = duration;
        gameObject.SetActive(true);
    }

    public void ShowEndscreen(int loopCount, bool success)
    {
        if (success) {
            endText.text = string.Format("You managed to fix the teleporter in {0} loops.<br><br><br>Press e to continue.", loopCount);
        }
        else {
            endText.text = string.Format("You couldn't manage to fix the teleporter, you lasted for {0} loops.<br><br><br>Press e to continue.", loopCount);
        }
        showEndscreen = true;
    }
}
