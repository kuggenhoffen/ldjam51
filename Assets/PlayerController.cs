using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController controller;
    public Transform cameraTransform;
    public Transform carryProxy;
    private UIManager uiManager;

    public float mouseSens = 100f;
    public float moveSpeed = 5f;
    public bool mouseInvert = false;
    private float gravity = 20f;

    private float cameraRotation = 1f;

    private GameObject pickedupObject;
    public GameObject jointPrefab;
    private GameObject joint;
    private Interactable targetInteractable;

    private bool inputActive = true;
    private Vector3 lastCarryPosition;
    private float pickupForce = 100f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        uiManager = gameController.GetComponent<UIManager>();
        Cursor.lockState = CursorLockMode.Locked;
        lastCarryPosition = carryProxy.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVec = Vector3.zero;

        if (Input.GetButtonDown("InputToggle")) {
            inputActive = !inputActive;
        }

        if (inputActive) {
            float inputX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
            float inputY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

            float vertAxis = Input.GetAxis("Vertical");
            float horAxis = Input.GetAxis("Horizontal");
            
            if (mouseInvert) {
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
            Debug.Log("Type of " + targetInteractable.name);
            if (targetInteractable.GetInteractType() == Interactable.InteractType.PickupableObject)
            {
                uiManager.ShowTooltip(string.Format("Pickup '{0}' using '{1}'", targetInteractable.transform.name, "e"));
            }
            else if (targetInteractable.GetInteractType() == Interactable.InteractType.Action)
            {
                uiManager.ShowTooltip(string.Format("Interact with '{0}' using '{1}'", targetInteractable.transform.name, "e"));
            }
        }
        else if (pickedupObject != null && targetInteractable == null) {
            uiManager.ShowTooltip(string.Format("Drop '{0}' using '{1}'", pickedupObject.transform.name, "e"));
        }
        else if (pickedupObject != null && targetInteractable != null) {
            uiManager.ShowTooltip(string.Format("Use '{0}' on '{1}' using '{2}'", pickedupObject.transform.name, targetInteractable.transform.name, "e"));
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
        Debug.Log("Object " + target.transform.name + " interactable type " + target.GetInteractType());
        switch (target.GetInteractType()) {
            case Interactable.InteractType.PickupableObject:
                if (pickedupObject) {
                    break;
                }
                Debug.Log("Try pickup");
                if (target.Activate(pickedupObject?.GetComponent<Interactable>())) {
                    PickupObject(target);
                }
                break;
            case Interactable.InteractType.Action:
                Debug.Log("Try activate");
                target.Activate(pickedupObject?.GetComponent<Interactable>());
                break;
            default:
                break;
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
        
        SetLayerInChildren(pickedupObject, LayerMask.NameToLayer("IgnoreRaycast"));
        targetInteractable = null;
        //other.Activate();
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

    void DropObject()
    {
        if (pickedupObject)
        {
            pickedupObject.GetComponent<Rigidbody>().useGravity = true;
            pickedupObject.GetComponent<Rigidbody>().drag = 1;
            pickedupObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            pickedupObject.GetComponent<Rigidbody>().transform.parent = null;
            SetLayerInChildren(pickedupObject, LayerMask.NameToLayer("Default"));
            pickedupObject = null;
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
    }
}
