using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    [SerializeField] GameObject attachObject;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float cameraDistance = 10.0f;
    [SerializeField][Range(0f,1f)] float rotationSmoothness = 0.5f;
    [SerializeField] float closeUpCameraAdjustment = 0.2f;

    private Vector3 rotation;
    private Camera childCamera;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = attachObject.transform.position;
        transform.rotation = Quaternion.identity;

        childCamera = GetComponentInChildren<Camera>();
        if (childCamera != null)
        {
            childCamera.transform.rotation = Quaternion.identity;
            childCamera.transform.position = transform.position + new Vector3(0, 0, -cameraDistance);
        }

        rotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCameraRig();
        CheckForObjectsInFrontOfCamera();
    }

    private void RotateCameraRig()
    {
        transform.position = attachObject.transform.position;

        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        rotation.x -= y * rotationSpeed;

        rotation.x = Mathf.Clamp(rotation.x, -90, 90);
        rotation.y += x * rotationSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), rotationSmoothness);
    }

    private void CheckForObjectsInFrontOfCamera()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, cameraDistance))
        {
            childCamera.transform.localPosition = new Vector3(0, 0, -hit.distance + closeUpCameraAdjustment);
        } 
        else
        {
            var layer = LayerMask.NameToLayer("Structure");
            childCamera.transform.localPosition = new Vector3(0, 0, -cameraDistance);
        }
    }
}
