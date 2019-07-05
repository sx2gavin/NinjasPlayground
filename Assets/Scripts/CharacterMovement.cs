using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private const float BIG_EPSILON = 0.00001f;

    [SerializeField] CameraRig m_cameraRig;
    [SerializeField] float m_topSpeed = 10.0f;
    [SerializeField] float acceleration = 1.0f;
    [SerializeField] [Range(0f, 1f)] float playerRotationSmoothness = 0.5f;
    
    Animator m_animator;
    CharacterController m_characterController;
    float m_currentSpeed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (direction.magnitude >= BIG_EPSILON)
        {
            direction.Normalize();
            var cameraRotation = m_cameraRig.transform.eulerAngles;

            direction = Quaternion.Euler(0, cameraRotation.y, 0) * direction;
            var rotateTo = Quaternion.FromToRotation(Vector3.forward, direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, playerRotationSmoothness);
            if (m_currentSpeed < m_topSpeed)
            {
                m_currentSpeed += acceleration * Time.deltaTime;
            }
            var vectorSpeed = direction * m_currentSpeed;
            vectorSpeed.y = m_characterController.velocity.y;
            m_characterController.SimpleMove(vectorSpeed);

            m_animator.SetFloat("speedPercent", m_currentSpeed / m_topSpeed);
        }
        else
        {
            if (m_currentSpeed > 0.0f)
            {
                m_currentSpeed -= 2 * acceleration * Time.deltaTime;
                if (m_currentSpeed < 0.0f)
                {
                    m_currentSpeed = 0.0f;
                }
                var playerDirection = transform.forward * m_currentSpeed;
                m_characterController.SimpleMove(new Vector3(playerDirection.x, m_characterController.velocity.y, playerDirection.z));
            }
            else
            {
                m_currentSpeed = 0.0f;
            }
            m_animator.SetFloat("speedPercent", m_currentSpeed / m_topSpeed);
        }
    }
}
