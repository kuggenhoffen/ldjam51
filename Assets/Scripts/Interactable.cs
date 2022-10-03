using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    public enum InteractType {
        Action,
        PickupableObject
    };

    public enum ActivateResult {
        None,
        Activate,
        Consume
    }

    public abstract InteractType GetInteractType();

    public abstract ActivateResult Activate(Interactable other);
}
