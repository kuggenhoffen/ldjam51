using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterProgrammer : PickupableObject
{

    private bool poweredOn = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool PoweredOn()
    {
        return poweredOn;
    }

    public void SetPoweredOn(bool powered)
    {
        poweredOn = powered;
    }
}
