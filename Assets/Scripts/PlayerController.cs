using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController controller;
    public Transform cameraTransform;
    public Transform carryProxy;
    private UIManager uiManager;
    private GameManager gameManager;

    public float mouseSens;
    public float moveSpeed = 5f;
    public bool mouseInvert;
    private float gravity = 20f;

    private float cameraRotation = 0f;

    private GameObject pickedupObject;
    public GameObject jointPrefab;
    private GameObject joint;
    private Interactable targetInteractable;

    public bool inputActive = true;
    private Vector3 lastCarryPosition;
    private float pickupForce = 100f;
    private Vector3 carryOffset;

    private AudioClip pickupSfx;
    public List<AudioClip> huhSfxList;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        uiManager = gameController.GetComponent<UIManager>();
        gameManager = gameController.GetComponent<GameManager>();
        Cursor.lockState = CursorLockMode.Locked;
        lastCarryPosition = carryProxy.transform.position;
        audioSource = GetComponent<AudioSource>(); 
        mouseSens = PlayerPrefs.GetFloat("sensitivity", 100f);
        mouseInvert = PlayerPrefs.GetInt("invert", 0) == 1 ? true : false;
        Debug.Log("Invert mouse " + mouseInvert);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVec = Vector3.zero;

        if (Application.isEditor && Input.GetButtonDown("Test")) {
            gameManager.GameOver(true);
        }

        if (inputActive) {
            float inputX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
            float inputY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

            float vertAxis = Input.GetAxis("Vertical");
            float horAxis = Input.GetAxis("Horizontal");
            
            if (!mouseInvert) {
                inputY = -inputY;
            }
            cameraRotation += inputY;

            cameraRotation = Mathf.Clamp(cameraRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(cameraRotation, 0f, 0f);
            transform.Rotate(Vector3.up * inputX);

            moveVec = transform.right * horAxis + transform.forward * vertAxis;
        }

        if (!controller.isGrounded)
        {
            moveVec.y -= gravity * Time.deltaTime;
        }

        controller.Move(moveVec * moveSpeed * Time.deltaTime);

        if (inputActive && Input.GetButtonDown("Interact"))
        {
            if (targetInteractable != null)
            {
                Interact(targetInteractable);
            }
            else {
                DropObject();
            }
        }

        MoveObject();
    }

    public void SetTransform(Vector3 pos, Quaternion rot)
    {
        controller.enabled = false;
        controller.transform.position = pos;
        controller.transform.rotation = rot;
        controller.enabled = true;
    }

    private AudioClip GetHuhSfx()
    {
        int index = Random.Range(0, huhSfxList.Count + 2);
        if (index < huhSfxList.Count) {
            return huhSfxList[index];
        }
        return null;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        targetInteractable = null;
        int layerMask = LayerMask.GetMask("IgnoreRaycast");
        layerMask = ~layerMask;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 2f, layerMask))
        {
            Interactable other = hit.transform.GetComponentInParent<Interactable>();
            if (other) {
                targetInteractable = other;
            }
        }

        ShowTargetTooltip();
    }

    void ShowTargetTooltip()
    {
        if (pickedupObject == null && targetInteractable != null)
        {
            if (targetInteractable.GetInteractType() == Interactable.InteractType.PickupableObject)
            {
                uiManager.ShowTooltip(string.Format("Pickup {0} using '{1}'", targetInteractable.transform.name, "e"));
            }
            else if (targetInteractable.GetInteractType() == Interactable.InteractType.Action)
            {
                uiManager.ShowTooltip(string.Format("Interact with {0} using '{1}'", targetInteractable.transform.name, "e"));
            }
        }
        else if (pickedupObject != null && targetInteractable == null) {
            uiManager.ShowTooltip(string.Format("Drop {0} using '{1}'", pickedupObject.transform.name, "e"));
        }
        else if (pickedupObject != null && targetInteractable != null) {
            uiManager.ShowTooltip(string.Format("Use {0} on {1} using '{2}'", pickedupObject.transform.name, targetInteractable.transform.name, "e"));
        }
        else {
            uiManager.HideTooltip();
        }
    }

    void LateUpdate()
    {
        /*if (pickedupObject) {
            Vector3 vel = carryProxy.transform.position - lastCarryPosition;
            pickedupObject.GetComponent<Rigidbody>().transform.Translate(vel);
        }
        lastCarryPosition = carryProxy.transform.position;*/
    }

    void Interact(Interactable target)
    {
        Interactable.ActivateResult result;
        if (target.GetInteractType() == Interactable.InteractType.PickupableObject && !pickedupObject)
        {
            result = target.Activate(null);
            if (result == Interactable.ActivateResult.Activate) {
                PickupObject(target);
            }
            else if (result == Interactable.ActivateResult.None) {
                audioSource.PlayOneShot(GetHuhSfx());
            }
        }
        else {
            result = target.Activate(pickedupObject?.GetComponent<Interactable>());
            if (result == Interactable.ActivateResult.Consume) {
                DropObject(true);
            }
            else if (result == Interactable.ActivateResult.None) {
                audioSource.PlayOneShot(GetHuhSfx());
            }
        }
    }

    void PickupObject(Interactable other)
    {
        pickedupObject = other.gameObject;
        pickedupObject.GetComponent<Rigidbody>().isKinematic = false;
        pickedupObject.GetComponent<Rigidbody>().useGravity = false;
        pickedupObject.GetComponent<Rigidbody>().drag = 10;
        pickedupObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        pickedupObject.GetComponent<Rigidbody>().transform.parent = carryProxy;
        carryOffset = carryProxy.position - pickedupObject.transform.position;
        
        SetLayerInChildren(pickedupObject, LayerMask.NameToLayer("IgnoreRaycast"));
        targetInteractable = null;
    }

    void SetLayerInChildren(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform transform in obj.GetComponentsInChildren<Transform>())
        {
            transform.gameObject.layer = newLayer;
        }
    }

    void MoveObject()
    {
        if (pickedupObject == null)
        {
            return;
        }

        Vector3 positionVector = carryProxy.position - pickedupObject.transform.position;
        if (positionVector.magnitude > 0.1f)
        {
            pickedupObject.GetComponent<Rigidbody>().AddForce(positionVector.normalized * pickupForce);
        }
    }

    void DropObject(bool consume = false)
    {
        if (pickedupObject)
        {
            GameObject go = pickedupObject;
            pickedupObject = null;
            go.GetComponent<Rigidbody>().useGravity = true;
            go.GetComponent<Rigidbody>().drag = 1;
            go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            go.GetComponent<Rigidbody>().transform.parent = null;
            SetLayerInChildren(go, LayerMask.NameToLayer("Default"));
            if (consume) {
                Destroy(go);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * 2f);
    }

    public void PrepareRespawn()
    {
        DropObject();
        inputActive = false;
    }
}
