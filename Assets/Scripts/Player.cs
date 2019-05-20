using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField] float speed = 500.0f;
    [SerializeField] float acceleration = 2.0f;
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
    }

    private void FixedUpdate()
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
        float right = Mathf.Clamp(Input.GetAxis("Horizontal") * acceleration, -1f, 1f) * speed * Time.deltaTime;
        float forward = Mathf.Clamp(Input.GetAxis("Vertical") * acceleration, -1f, 1f) * speed * Time.deltaTime;

        if (Math.Abs(right) >= BIG_EPSILON || Math.Abs(forward) >= BIG_EPSILON)
        {
            var movingVelocity = new Vector3(right, 0, forward);

            var cameraRotation = cameraRig.transform.eulerAngles;
            movingVelocity = Quaternion.Euler(0, cameraRotation.y, 0) * movingVelocity;

            var rotateTo = Quaternion.FromToRotation(Vector3.forward, movingVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, playerRotationSmoothness);


            movingVelocity.y = m_rigidBody.velocity.y;
            m_rigidBody.velocity = movingVelocity;
        }
        else
        {
            m_rigidBody.velocity = new Vector3(0, m_rigidBody.velocity.y, 0);
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
