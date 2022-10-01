using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    public enum InteractType {
        Action,
        PickupableObject
    };

    public abstract InteractType GetInteractType();

    public abstract bool Activate(Interactable other);
}
