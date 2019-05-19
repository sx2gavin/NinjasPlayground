using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField] float speed = 500.0f;
    [SerializeField] CameraRig cameraRig;
    [SerializeField] float jumpHeight = 20.0f;
    [SerializeField] [Range(0f, 1f)] float playerRotationSmoothness = 0.5f;

    Rigidbody m_rigidBody;
    bool m_isInAir = false;

    private const float BIG_EPSILON = 0.00001f;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        m_isInAir = CheckInAIr();
        Move();
        Jump();
    }

    private bool CheckInAIr()
    {
        return Math.Abs(m_rigidBody.velocity.y) >= BIG_EPSILON;
    }

    private void Move()
    {
        if (m_isInAir == false)
        {
            float right = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            if (Math.Abs(right) >= BIG_EPSILON || Math.Abs(forward) >= BIG_EPSILON)
            {
                var movingVelocity = new Vector3(right, 0, forward);
                var cameraRotation = cameraRig.transform.eulerAngles;
                movingVelocity = Quaternion.Euler(0, cameraRotation.y, 0) * movingVelocity;
                movingVelocity.y = m_rigidBody.velocity.y;
                m_rigidBody.velocity = movingVelocity;
                var rotateTo = Quaternion.FromToRotation(Vector3.forward, movingVelocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, playerRotationSmoothness);
            }
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && !m_isInAir)
        {
            var currentVelocity = m_rigidBody.velocity;
            currentVelocity.y = jumpHeight;
            m_rigidBody.velocity = currentVelocity;
        }
    }
}
