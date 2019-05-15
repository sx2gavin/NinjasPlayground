using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 500.0f;
    [SerializeField] CameraRig cameraRig;
    [SerializeField] float damping = 10f;
    Rigidbody m_rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float right = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        if (Mathf.Abs(right) > 0.00001 || Mathf.Abs(forward) > 0.00001)
        {
            var flyingVector = forward * Vector3.forward + right * Vector3.right;
            flyingVector = cameraRig.transform.rotation * flyingVector;
            var newPosition = transform.position + flyingVector;

            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * damping);
        }
    }
}
