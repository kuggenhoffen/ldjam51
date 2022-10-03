using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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
    public bool locked = true;
    public AudioClip lockedSfx;
    public AudioClip openSfx;
    public AudioClip closeSfx;
    private AudioSource audioSource;

    

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                audioSource.PlayOneShot(closeSfx);
            }
        }
        angle = Mathf.Clamp(angle, closedAngle, openedAngle);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z); 
    }

    public override InteractType GetInteractType() 
    {
         return InteractType.Action; 
    }

    public override ActivateResult Activate(Interactable other)
    {
        if (locked) {
            audioSource.PlayOneShot(lockedSfx);
            return ActivateResult.None;
        }
        if (other != null) {
            return ActivateResult.None;
        }

        if (state == State.closed) {
            state = State.opening;
            audioSource.PlayOneShot(openSfx);
        }
        else if (state == State.opened) {
            state = State.closing;
        }
        return ActivateResult.Activate;
    }
}
