using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDoorInteractable : Interactable
{
    private UIManager uiManager;

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
        if (other == null) {
            uiManager.ShowToastTip("I can't move it by hand, maybe there's a button somewhere.");
        }
        else {
            uiManager.ShowToastTip("That doesn't work, maybe there's a button somewhere.");
        }
        return ActivateResult.None;
    }
}
