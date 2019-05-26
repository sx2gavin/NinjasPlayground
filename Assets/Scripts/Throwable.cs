using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float flyingSpeed;

    private Rigidbody m_rigidbody;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 20, 0));
    }

    public void Fly(Vector3 startPosition, Vector3 direction)
    {
        m_rigidbody = GetComponent<Rigidbody>();
        if (m_rigidbody != null)
        {
            transform.position = startPosition;
            m_rigidbody.AddForce(direction.normalized * flyingSpeed, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 1.0f);
    }
}
