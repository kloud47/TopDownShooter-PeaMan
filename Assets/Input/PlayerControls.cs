using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [Header("Movement Info")] 
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravityScale = 9.81f;
    [Header("Aim Info")] 
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private Transform aim;

    private float speed;
    public bool isRunning;
    private float verticalVelocity;
    private Player controls;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 movementInput;
    private Vector2 aimInput;
    private Vector3 lookingDirection;
    private Animator animator;

    private void Awake()
    {
        controls = new Player();

        // Binding all the inputs to the player controller:
        AssignInputMethod();
    }

    private void AssignInputMethod()
    {
        controls.Character.Movement.performed += context => movementInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => movementInput = Vector2.zero;
        
        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;

        controls.Character.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };
        
        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(moveDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(moveDirection.normalized, transform.forward);
        animator.SetFloat("xVelocity", xVelocity, .1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, .1f, Time.deltaTime);

        bool playRunningAnimation = isRunning && moveDirection.magnitude > 0;
        animator.SetBool("IsRunning", playRunningAnimation);
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardsMouse();
        AnimatorControllers();
    }

    private void AimTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        // Ray from camera main will hit the obstacles or ground and this will cause character to look in that direction
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))// layermask is used to provide collision interaction Info.
        {
            lookingDirection = hitInfo.point - transform.position;
            lookingDirection.y = 0f;
            lookingDirection.Normalize();
            transform.forward = lookingDirection;
            aim.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
        }
    }

    private void ApplyMovement()
    {
        moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        ApplyGravity();

        if (moveDirection.magnitude > 0)
        {
            characterController.Move(speed * moveDirection * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
            moveDirection.y = verticalVelocity;
        }
        else verticalVelocity = -.5f;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
