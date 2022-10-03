using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractableWindow : Interactable
{
    public enum State {
        closed,
        opening,
        opened,
        closing
    }
    public State state;
    private UIManager uiManager;
    public bool locked = true;
    public AudioClip lockedSfx;
    public AudioClip openSfx;
    public AudioClip closeSfx;
    private AudioSource audioSource;
    private Vector3 startPosition;
    private const float openAmount = 0.7f;
    private float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        uiManager = gameController.GetComponent<UIManager>();
        state = State.closed;
        audioSource = GetComponentInChildren<AudioSource>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.opening) {
            transform.Translate(Vector3.up * Time.deltaTime * speed);
            if (transform.position.y > startPosition.y + openAmount) {
                transform.position = startPosition + Vector3.up * openAmount;
                state = State.opened;
            }
        }
        else if (state == State.closing) {
            transform.Translate(Vector3.up * Time.deltaTime * -speed);
            if (transform.position.y < startPosition.y) {
                transform.position = startPosition;
                audioSource.PlayOneShot(closeSfx);
                state = State.closed;
            }
        }
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
