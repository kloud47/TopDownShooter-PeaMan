using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [Header("Movement Info")]
    [SerializeField] private float walkSpeed;
    
    private Player controls;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 movementInput;
    private Vector2 aimInput; 

    private void Awake()
    {
        controls = new Player();

        // Binding all the inputs to the player controller:
        controls.Character.Movement.performed += context => movementInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => movementInput = Vector2.zero;
        
        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        moveDirection = new Vector3(movementInput.x, 0, movementInput.y);

        if (moveDirection.magnitude > 0)
        {
            characterController.Move(walkSpeed * moveDirection * Time.deltaTime);
        }
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
