using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : Interactable
{

    private bool boltedOn = true;
    private bool missingBattery = true;
    public List<Interactable> toolList = new List<Interactable>();
    public Interactable battery;
    Interactable correctTool;
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        uiManager = gameController.GetComponent<UIManager>();
        if (toolList.Count > 0) {
            correctTool = toolList[Random.Range(0, toolList.Count)];
        }
        GetComponent<Rigidbody>().isKinematic = true;
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

    public override bool Activate(Interactable other)
    {
        Debug.Log("Activate");
        if (!boltedOn && !missingBattery) {
            return true;
        }

        if (missingBattery) {
            if (other == null) {
                uiManager.ShowToastTip("I need a battery for it first.");
                return false;
            }
            else if (other != battery) {
                uiManager.ShowToastTip("That won't work.");
                return false;
            }
            else if (other == battery) {
                uiManager.ShowToastTip("That's it! It's powered by the battery now.");
                missingBattery = false;
                return true;
            }
        }
        else if (boltedOn) {
            if (other == null) {
                uiManager.ShowToastTip("I need to unbolt it first.");
                return false;
            }
            else if (other != correctTool) {
                uiManager.ShowToastTip("That won't work.");
                return false;
            }
            else if (other == correctTool) {
                uiManager.ShowToastTip("That's it! It's unbolted now.");
                boltedOn = false;
                return true;
            }
        }

        return false;
    }
}
