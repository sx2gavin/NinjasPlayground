using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{

    [SerializeField] GameObject attachObject;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float cameraDistance = 10.0f;
    [SerializeField][Range(0f,1f)] float rotationSmoothness = 0.5f;
    [SerializeField] float closeUpCameraAdjustment = 0.5f;
    [SerializeField] float anchorHeight = 2f;

    private Vector3 rotation;
    private Camera childCamera;
    private List<TargetableObject> m_targets = null;
    private TargetableObject m_lastTarget = null;

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
    
    void FixedUpdate()
    {
        FollowAttachObject();
        RotateCameraRig();
        CheckForObjectsInFrontOfCamera();
    }

    private void RotateCameraRig()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        rotation.x -= y * rotationSpeed;

        rotation.x = Mathf.Clamp(rotation.x, -90, 90);
        rotation.y += x * rotationSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), rotationSmoothness);

        var bestTarget = FindBestTargetToFocus();
        if (bestTarget != m_lastTarget)
        {
            if (m_lastTarget != null)
            {
                m_lastTarget.IsTargetMarkerVisible = false;
            }
            m_lastTarget = bestTarget;
        }
        if (m_lastTarget != null)
        {
            m_lastTarget.IsTargetMarkerVisible = true;
            RotateToTargetableObject(m_lastTarget);
        }
    }

    internal void Targeting(List<TargetableObject> targetableObjects)
    {
        m_targets = targetableObjects;
    }

    private TargetableObject FindBestTargetToFocus()
    {
        var smallestAngle = float.MaxValue;
        TargetableObject bestTarget = default;
        if (m_targets != null)
        {
            foreach (var target in m_targets)
            {
                var direction = target.transform.position - transform.position;
                direction.Normalize();

                var cameraOverlookVector = Quaternion.Euler(30, 0, 0) * transform.forward;

                var angle = Vector3.Angle(cameraOverlookVector, direction);
                if (angle < 20f && angle < smallestAngle)
                {
                    smallestAngle = angle;
                    bestTarget = target;
                }
            }
        }
        return bestTarget;
    }

    private void RotateToTargetableObject(TargetableObject target)
    {
        var direction = target.transform.position - attachObject.transform.position;
        var groundAngle = Mathf.Atan2(direction.x, direction.z);
        var yawQuaternion = Quaternion.Euler(0, groundAngle * Mathf.Rad2Deg, 0);
        var pitchQuaternion = Quaternion.Euler(30, 0, 0);
        var result = yawQuaternion;
        transform.rotation = Quaternion.Slerp(transform.rotation, result, rotationSmoothness);
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
