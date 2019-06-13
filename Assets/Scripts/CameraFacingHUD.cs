using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacingHUD : MonoBehaviour
{
    Camera m_mainCamera;

    private void Start()
    {
        m_mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(m_mainCamera.transform.position - transform.position);
        }
    }
}
