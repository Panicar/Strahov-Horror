using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;

    [Header("Gravity & Jump")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Mouse Look")]
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;

    [Tooltip("Assign your Main Camera here")]
    public Transform playerCamera;
    public float maxLookAngle = 90f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        HandleCursorLock();
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        // If no camera is assigned, don't try to rotate it (prevents errors)
        if (playerCamera == null) return;

        float mouseX = Mouse.current.delta.x.ReadValue() * sensitivityX * 0.01f;
        float mouseY = Mouse.current.delta.y.ReadValue() * sensitivityY * 0.01f;

        yRotation += mouseX;

        // Up/Down rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Rotate the PLAYER BODY horizontally (Left/Right)
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // Rotate ONLY the CAMERA vertically (Up/Down)
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        float x = 0f;
        float z = 0f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.wKey.isPressed) z += 1f;
        if (Keyboard.current.sKey.isPressed) z -= 1f;

        bool sprint = Keyboard.current.leftShiftKey.isPressed;
        bool jump = Keyboard.current.spaceKey.wasPressedThisFrame;

        // Flat horizontal movement based on the player body's forward direction
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = (right * x) + (forward * z);
        if (move.magnitude > 1f) move.Normalize();

        float speed = moveSpeed * (sprint ? sprintMultiplier : 1f);

        if (jump && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * speed + new Vector3(0f, velocity.y, 0f);
        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleCursorLock()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnGUI()
    {
        float cx = Screen.width / 2f;
        float cy = Screen.height / 2f;
        float dotSize = 4f;

        GUI.color = new Color(1f, 1f, 1f, 0.5f);
        GUI.DrawTexture(new Rect(cx - (dotSize / 2f), cy - (dotSize / 2f), dotSize, dotSize), Texture2D.whiteTexture);
    }
}