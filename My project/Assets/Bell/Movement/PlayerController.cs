using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputs playerActions;
    private InputAction move;

    private Rigidbody rb;
    [SerializeField] float movementForce = 1f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float maxSpeed = 5f;
    private Vector3 forceDir = Vector3.zero;

    [SerializeField] Camera cam;

    public bool grounded = false;

    private Animator animator;

    private PauseMenu pauseMenu;

    private void Awake()
    {
        animator = this.GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        playerActions = new PlayerInputs();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
    }

    private void FixedUpdate()
    {
        if (pauseMenu.IsPaused() == false)
        {
            forceDir += move.ReadValue<Vector2>().x * GetCameraRight(cam) * movementForce;
            forceDir += move.ReadValue<Vector2>().y * GetCameraForward(cam) * movementForce;

            rb.AddForce(forceDir, ForceMode.Impulse);
            forceDir = Vector3.zero;

            if (rb.linearVelocity.y < 0f)
            {
                rb.linearVelocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
            }

            Vector3 horizontalVelocity = rb.linearVelocity;
            horizontalVelocity.y = 0;
            if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                rb.linearVelocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.linearVelocity.y;
            }

            LookAt();
        }
    }

    private void Update()
    {
        animator.SetFloat("Speed", rb.linearVelocity.magnitude / maxSpeed);
    }

    private void LookAt()
    {
        Vector3 dir = rb.linearVelocity;
        dir.y = 0;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && dir.sqrMagnitude > 0.1f)
        {
            this.rb.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private Vector3 GetCameraForward(Camera cam)
    {
        Vector3 forward = cam.transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera cam)
    {
        Vector3 right = cam.transform.right;
        right.y = 0f;
        return right.normalized;
    }

    private void OnEnable()
    {
        playerActions.Player.Jump.started += Jump;
        move = playerActions.Player.Move;
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.Jump.started -= Jump;
        playerActions.Player.Disable();
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (grounded && pauseMenu.IsPaused() == false)
        {
            animator.SetTrigger("Jump");
            forceDir += Vector3.up * jumpForce;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Touched " + other.tag);
        if (other.transform.tag == "Ground")
        {
            grounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        grounded = false;
    }
}
