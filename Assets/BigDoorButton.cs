using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDoorButton : Interactable
{

    public Transform doorLeft;
    public Transform doorRight;

    private Vector3 doorLeftClosePosition;
    private Vector3 doorRightClosePosition;
    private const float doorOpenSize = 1f;
    private const float doorSpeed = 1f;
    private const float doorOpenTime = 1f;
    private float doorOpenTimer = 0f;

    public enum State {
        closed,
        opening,
        opened,
        closing
    }

    public State state;

    // Start is called before the first frame update
    void Start()
    {
        doorLeftClosePosition = doorLeft.position;
        doorRightClosePosition = doorRight.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.opening) {
            bool ready = true;
            if (doorLeft.position.z < doorLeftClosePosition.z + doorOpenSize) {
                doorLeft.Translate(Vector3.forward * doorSpeed * Time.deltaTime);
                ready &= false;
            }
            if (doorRight.position.z > doorRightClosePosition.z - doorOpenSize) {
                doorRight.Translate(Vector3.back * doorSpeed * Time.deltaTime);
                ready &= false;
            }
            if (ready) {
                state = State.opened;
                doorOpenTimer = doorOpenTime;
            }
        }
        else if (state == State.opened) {
            doorOpenTimer -= Time.deltaTime;
            if (doorOpenTimer <= 0f) {
                state = State.closing;
            }
        }
        else if (state == State.closing) {
            bool ready = true;
            if (doorLeft.position.z > doorLeftClosePosition.z) {
                doorLeft.Translate(Vector3.back * doorSpeed * Time.deltaTime);
                ready &= false;
            }
            if (doorRight.position.z < doorRightClosePosition.z) {
                doorRight.Translate(Vector3.forward * doorSpeed * Time.deltaTime);
                ready &= false;
            }
            if (ready) {
                state = State.closed;
            }
        }
        doorLeft.position = new Vector3(doorLeft.position.x, 
                                        doorLeft.position.y, 
                                        Mathf.Clamp(doorLeft.position.z, doorLeftClosePosition.z, doorLeftClosePosition.z + doorOpenSize));
        doorRight.position = new Vector3(doorRight.position.x, 
                                         doorRight.position.y, 
                                         Mathf.Clamp(doorRight.position.z, doorRightClosePosition.z - doorOpenSize, doorRightClosePosition.z));
    }

    public override InteractType GetInteractType()
    {
        return InteractType.Action;
    }

    public override bool Activate(Interactable other)
    {
        if (state != State.closed) {
            return false;
        }

        state = State.opening;
        return true;
    }
}
