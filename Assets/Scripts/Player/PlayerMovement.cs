using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;

    public InputAction MoveAction;
    public InputAction SprintAction;
    public InputAction JumpAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public float sprintMultiplier = 1.8f;
    public float jumpImpulse = 5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask = ~0;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    bool m_IsGrounded;
    bool m_QueuedJump;
    Transform m_CameraTransform;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        if (SprintAction != null) SprintAction.Enable();
        if (JumpAction != null) JumpAction.Enable();
        if (Camera.main != null) m_CameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (JumpAction != null && JumpAction.triggered)
        {
            m_QueuedJump = true;
        }
        if (m_CameraTransform == null && Camera.main != null)
        {
            m_CameraTransform = Camera.main.transform;
        }
    }

    void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;
        if (m_CameraTransform != null)
        {
            Vector3 camForward = m_CameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();
            Vector3 camRight = m_CameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();
            m_Movement = camRight * horizontal + camForward * vertical;
            if (m_Movement.sqrMagnitude > 1f) m_Movement.Normalize();
        }
        else
        {
            m_Movement.Set(horizontal, 0f, vertical);
            m_Movement.Normalize();
        }

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        // Make character face the camera's yaw so it "rotates with the camera"
        if (m_CameraTransform != null)
        {
            Vector3 cameraYawForward = m_CameraTransform.forward;
            cameraYawForward.y = 0f;
            if (cameraYawForward.sqrMagnitude > 0.0001f)
            {
                cameraYawForward.Normalize();
                Vector3 desiredForward = Vector3.RotateTowards(transform.forward, cameraYawForward, turnSpeed * Time.deltaTime, 0f);
                m_Rotation = Quaternion.LookRotation(desiredForward);
            }
        }
        else if (m_Movement.sqrMagnitude > 0.0001f)
        {
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
        }

        // Ground check
        m_IsGrounded = Physics.Raycast(m_Rigidbody.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f, groundMask, QueryTriggerInteraction.Ignore);

        // Jump
        if (m_QueuedJump && m_IsGrounded)
        {
            m_QueuedJump = false;
            m_Rigidbody.AddForce(Vector3.up * jumpImpulse, ForceMode.VelocityChange);
        }
        else
        {
            // Clear queued flag if we couldn't jump this frame (prevents sticky buffering when airborne for too long)
            m_QueuedJump = false;
        }

        // Sprinting affects planar speed
        float speed = walkSpeed;
        if (SprintAction != null && SprintAction.IsPressed())
        {
            speed *= sprintMultiplier;
        }

        m_Rigidbody.MoveRotation(m_Rotation);
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * speed * Time.deltaTime);
    }
}