using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : Interactable
{
    private float angle = 0f;
    private const float openAngle = 110f;
    private float closedAngle;
    private float openedAngle;
    public enum State {
        closed,
        opening,
        opened,
        closing
    }
    public State state;
    private float openSpeed = 100f;

    

    // Start is called before the first frame update
    void Start()
    {
        closedAngle = transform.rotation.eulerAngles.y;
        openedAngle = closedAngle + openAngle;
        angle = transform.rotation.eulerAngles.y;
        if (state == State.opened) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, openedAngle, transform.rotation.eulerAngles.z); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.opening) {
            if (angle < openedAngle) {
                angle += openSpeed * Time.deltaTime;
            }
            else {
                state = State.opened;
            }
        }
        else if (state == State.closing) {
            if (angle > closedAngle) {
                angle -= openSpeed * Time.deltaTime;
            }
            else {
                state = State.closed;
            }
        }
        angle = Mathf.Clamp(angle, closedAngle, openedAngle);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z); 
    }

    public override InteractType GetInteractType() 
    {
         return InteractType.Action; 
    }

    public override bool Activate(Interactable other)
    {
        if (other != null) {
            return false;
        }

        if (state == State.closed) {
            state = State.opening;
        }
        else if (state == State.opened) {
            state = State.closing;
        }
        return true;
    }
}
