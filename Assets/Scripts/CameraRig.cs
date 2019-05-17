using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    [SerializeField] GameObject attachObject;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float cameraDistance = 10.0f;
    [SerializeField][Range(0f,1f)] float rotationSmoothness = 0.5f;

    private Vector3 rotation;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = attachObject.transform.position;
        transform.rotation = Quaternion.identity;

        var cameraChildren = GetComponentInChildren<Camera>()?.gameObject;
        if (cameraChildren != null)
        {
            cameraChildren.transform.rotation = Quaternion.identity;
            cameraChildren.transform.position = transform.position + new Vector3(0, 0, -cameraDistance);
        }

        rotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = attachObject.transform.position;

        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        rotation.x -= y * rotationSpeed;

        rotation.x = Mathf.Clamp(rotation.x, -90, 90);
        rotation.y += x * rotationSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), 0.5f);
    }
}
