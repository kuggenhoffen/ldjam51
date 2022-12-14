using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryObject : Interactable
{
    private bool screwedOn = true;
    public Interactable requiredTool;
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        uiManager = gameController.GetComponent<UIManager>();
        if (requiredTool == null) {
            screwedOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override InteractType GetInteractType()
    {
        if (screwedOn) {
            return InteractType.Action;
        }
        return InteractType.PickupableObject;
    }

    public override ActivateResult Activate(Interactable other)
    {
        if (screwedOn) {
            if (other != requiredTool) {
                uiManager.ShowToastTip("That won't work, I need a screwdriver.");
                return ActivateResult.None;
            }
            uiManager.ShowToastTip("That worked, I can pick it up now.");
            screwedOn = false;
        }
        
        return ActivateResult.Activate;
    }
}
