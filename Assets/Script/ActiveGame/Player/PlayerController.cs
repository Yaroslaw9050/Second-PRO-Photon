using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private CameraFlow myCamera;
    private PhotonView pv;
    private PlayerInput inputActions;
    private CharacterController controller;
    private Animator animator;
    private Vector2 movementInput;
    private Vector3 currentMovement;
    private Quaternion rotateDir;
    private bool isRun;
    private bool isWalk;
    private const string animatorIsRun = "isRun";
    private const string animatorIsWalk = "isWalk";

    private void Awake()
    {
        pv = gameObject.GetComponentInParent<PhotonView>();
        controller = gameObject.GetComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();
        inputActions = new PlayerInput();

        inputActions.PlayerController.Movement.started += OnMovementActions;
        inputActions.PlayerController.Movement.performed += OnMovementActions;
        inputActions.PlayerController.Movement.canceled += OnMovementActions;

        inputActions.PlayerController.Run.started += OnRunActions;
        inputActions.PlayerController.Run.canceled += OnRunActions;

        inputActions.PlayerController.Movement.started += OnCameraMovement;
        inputActions.PlayerController.Movement.performed += OnCameraMovement;
        inputActions.PlayerController.Movement.canceled += OnCameraMovement;

        if(!pv.IsMine)
        {
            Destroy(myCamera.gameObject);
        }
    }
    private void OnEnable()
    {
        inputActions.PlayerController.Enable();
    }
    private void OnDisable()
    {
        inputActions.PlayerController.Disable();
    }
    private void OnMovementActions(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        
        currentMovement.x = movementInput.x;
        currentMovement.z = movementInput.y;

        isWalk = movementInput.x != 0 || movementInput.y != 0;
    }
    private void OnCameraMovement(InputAction.CallbackContext context)
    {
        myCamera.SetOffset(currentMovement);
    }
    private void OnRunActions(InputAction.CallbackContext context)
    {
        isRun = context.ReadValueAsButton();
    }

    private void PlayerRotate()
    {
        if(!isWalk) return;

        rotateDir = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentMovement), Time.deltaTime * rotateSpeed);

        transform.rotation = rotateDir;
    }

    private void AnimationController()
    {
        animator.SetBool(animatorIsWalk, isWalk);
        animator.SetBool(animatorIsRun, isRun);
    }
    private void Update()
    {
        if(!pv.IsMine) return;

        AnimationController();
        PlayerRotate();
    }
    private void FixedUpdate()
    {
        if(!pv.IsMine) return;

        controller.Move(currentMovement * Time.fixedDeltaTime);
    }

    public void Respawn()
    {
        controller.enabled = false;
        transform.position = Vector3.up;
        controller.enabled = true;
    }
}
