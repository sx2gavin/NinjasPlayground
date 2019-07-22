using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private const float BIG_EPSILON = 0.00001f;

    [SerializeField] CameraRig m_cameraRig;
    [SerializeField] float m_topSpeed = 5.0f;
    [SerializeField] float m_acceleration = 10.0f;
    [SerializeField] [Range(0f, 1f)] float playerRotationSmoothness = 0.5f;
    [SerializeField] float m_jumpInitialSpeed = 20.0f;
    [SerializeField] float m_gravity = -9.8f;

    Animator m_animator;
    CharacterController m_characterController;
    float m_currentSpeed = 0.0f;
    float m_verticalSpeed = 0.0f;
    bool m_isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Jump();
        Move();
        Attack();
    }

    public void BeginAttacking()
    {
        m_isAttacking = true;
    }

    public void EndAttacking()
    {
        m_isAttacking = false;
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_animator.SetTrigger("attack");
        }
    }

    private void Jump()
    {
        if (m_characterController.isGrounded)
        {
            m_verticalSpeed = 0.0f;
            m_animator.SetBool("inAir", false);
        }
        else
        {
            m_verticalSpeed += m_gravity * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && m_characterController.isGrounded)
        {
            m_verticalSpeed = m_jumpInitialSpeed;
            m_animator.SetTrigger("jump");
            m_animator.SetBool("inAir", true);
        }
    }

    private void Move()
    {
        Vector3 vectorSpeed = Vector3.zero;

        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (direction.magnitude >= BIG_EPSILON && !m_isAttacking)
        {
            direction.Normalize();
            var cameraRotation = m_cameraRig.transform.eulerAngles;

            direction = Quaternion.Euler(0, cameraRotation.y, 0) * direction;
            var rotateTo = Quaternion.FromToRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, playerRotationSmoothness);
            if (m_currentSpeed < m_topSpeed)
            {
                m_currentSpeed += m_acceleration * Time.deltaTime;
            }
        }
        else
        {
            if (m_currentSpeed > 0.0f)
            {
                m_currentSpeed -= 2 * m_acceleration * Time.deltaTime;
                if (m_currentSpeed < 0.0f)
                {
                    m_currentSpeed = 0.0f;
                }
            }
            else
            {
                m_currentSpeed = 0.0f;
            }
        }
        vectorSpeed = transform.forward * m_currentSpeed;
        vectorSpeed.y = m_verticalSpeed;
        vectorSpeed *= Time.deltaTime;
        m_characterController.Move(vectorSpeed);
        m_animator.SetFloat("speedPercent", m_currentSpeed / m_topSpeed);
    }
}
