using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TeleporterObject : PickupableObject
{

    private bool boltedOn = true;
    private bool missingBattery = true;
    public List<Interactable> toolList = new List<Interactable>();
    public Interactable battery;
    Interactable correctTool;
    public TeleporterProgrammer programmingTool;
    private UIManager uiManager;
    private GameManager gameManager;
    public AudioClip unboltSfx;
    

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        uiManager = gameController.GetComponent<UIManager>();
        gameManager = gameController.GetComponent<GameManager>();
        if (toolList.Count > 0) {
            correctTool = toolList[Random.Range(0, toolList.Count)];
        }
        GetComponent<Rigidbody>().isKinematic = true;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override InteractType GetInteractType()
    {
        if (boltedOn || missingBattery) {
            return InteractType.Action;
        }
        return InteractType.PickupableObject;
    }

    public override ActivateResult Activate(Interactable other)
    {
        Debug.Log("Activate");
        if (!boltedOn && !missingBattery) {
            if (other == programmingTool) {
                if (programmingTool.PoweredOn()) {
                    gameManager.GameOver(true);
                    // Game finished!
                }
                else {
                    uiManager.ShowToastTip("It won't work, I need the programming disk first.");
                }
                return ActivateResult.Activate;
            }
            return ActivateResult.Activate;
        }

        if (missingBattery) {
            if (other == null) {
                uiManager.ShowToastTip("I need a battery for it first.");
                return ActivateResult.None;
            }
            else if (other != battery) {
                uiManager.ShowToastTip("That won't work, I need a battery for it first.");
                return ActivateResult.None;
            }
            else if (other == battery) {
                uiManager.ShowToastTip("That's it! It's powered by the battery now.");
                missingBattery = false;
                return ActivateResult.Consume;
            }
        }
        else if (boltedOn) {
            if (other == null) {
                uiManager.ShowToastTip("I need to unbolt it first.");
                return ActivateResult.None;
            }
            else if (other != correctTool) {
                uiManager.ShowToastTip("That won't work.");
                return ActivateResult.None;
            }
            else if (other == correctTool) {
                uiManager.ShowToastTip("That's it! It's unbolted now.");
                boltedOn = false;
                audioSource.PlayOneShot(unboltSfx);
                return ActivateResult.Activate;
            }
        }

        return ActivateResult.None;
    }
}
