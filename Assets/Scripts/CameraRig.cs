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
    [SerializeField] float anchorHeight = 2f;

    private Vector3 rotation;
    private Camera childCamera;

    public Transform Target { get; set; }
    public bool LockingTarget { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        FollowAttachObject();
        transform.rotation = Quaternion.identity;

        childCamera = GetComponentInChildren<Camera>();
        if (childCamera != null)
        {
            childCamera.transform.rotation = Quaternion.identity;
            childCamera.transform.localPosition = new Vector3(0, 0, -cameraDistance);
        }

        rotation = transform.rotation.eulerAngles;
    }

    private void FollowAttachObject()
    {
        var newPosition = attachObject.transform.position + new Vector3(0, anchorHeight, 0);
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.3f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowAttachObject();
        RotateCameraRig();
        CheckForObjectsInFrontOfCamera();
    }

    private void RotateCameraRig()
    {
        if (LockingTarget)
        {
            var direction = Target.position - attachObject.transform.position;
            direction.Normalize();

            var angle = Mathf.Atan2(direction.x, direction.z);
            var yawQuaternion = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0);

            var pitchQuaternion = Quaternion.Euler(30, 0, 0);

            var result = yawQuaternion * pitchQuaternion;

            transform.rotation = Quaternion.Slerp(transform.rotation, result, rotationSmoothness);
        }
        else
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            rotation.x -= y * rotationSpeed;

            rotation.x = Mathf.Clamp(rotation.x, -90, 90);
            rotation.y += x * rotationSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), rotationSmoothness);
        }
    }

    private void CheckForObjectsInFrontOfCamera()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, cameraDistance))
        {
            childCamera.transform.localPosition = new Vector3(0, 0, -hit.distance + closeUpCameraAdjustment);
        } 
        else
        {
            childCamera.transform.localPosition = new Vector3(0, 0, -cameraDistance);
        }
    }
}
