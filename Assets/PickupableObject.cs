using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupableObject : Interactable
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override InteractType GetInteractType()
    {
        return InteractType.PickupableObject;
    }

    public override bool Activate(Interactable other)
    {
        return other == null;
    }

}
