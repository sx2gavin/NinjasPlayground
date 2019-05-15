using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    [SerializeField] GameObject m_attachObject;
    [SerializeField] float m_rotationSpeed = 10.0f;
    [SerializeField] float m_cameraDistance = 10.0f;

    private Vector3 rotation;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = m_attachObject.transform.position;
        transform.rotation = Quaternion.identity;

        var cameraChildren = GetComponentInChildren<Camera>()?.gameObject;
        if (cameraChildren != null)
        {
            cameraChildren.transform.rotation = Quaternion.identity;
            cameraChildren.transform.position = transform.position + new Vector3(0, 0, -m_cameraDistance);
        }

        rotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_attachObject.transform.position;

        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        rotation.x -= y * m_rotationSpeed;

        rotation.x = Mathf.Clamp(rotation.x, -90, 90);
        rotation.y += x * m_rotationSpeed;

        transform.rotation = Quaternion.Euler(rotation);
    }
}
