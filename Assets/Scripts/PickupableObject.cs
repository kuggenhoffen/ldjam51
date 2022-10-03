using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PickupableObject : Interactable
{

    public AudioClip dropSfx;

    public AudioSource audioSource {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameManager>();
        dropSfx = gameManager.dropSfx;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (audioSource && !audioSource.isPlaying && collision.relativeVelocity.magnitude > 1f) {
            audioSource.PlayOneShot(dropSfx);
        }
    }

    public override InteractType GetInteractType()
    {
        return InteractType.PickupableObject;
    }

    public override ActivateResult Activate(Interactable other)
    {
        return other == null ? ActivateResult.Activate : ActivateResult.None;
    }

}
