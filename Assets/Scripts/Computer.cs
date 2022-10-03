using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Interactable
{
    private bool missingDisk = true;
    public Interactable requiredTool;
    private UIManager uiManager;
    public TeleporterProgrammer programmer;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        uiManager = gameController.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override InteractType GetInteractType()
    {
        return InteractType.Action;
    }

    public override ActivateResult Activate(Interactable other)
    {
        if (missingDisk) {
            if (other != requiredTool) {
                uiManager.ShowToastTip("That's useless unless I have the programming disk.");
                return ActivateResult.None;
            }
            else if (other == requiredTool) {
                uiManager.ShowToastTip("That worked.");
                missingDisk = false;
                programmer.SetPoweredOn(true);
                return ActivateResult.Consume;
            }
        }
        
        return ActivateResult.None;
    }
}
