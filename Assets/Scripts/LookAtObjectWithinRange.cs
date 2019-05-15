using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObjectWithinRange : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float verticalRange = 90f;
    [SerializeField] float horizontalRange = 90f;

    Vector3 initialForwardVector;
    Vector3 initialRightVector;
    Matrix4x4 initialWorldToLocalMatrix;

    // Start is called before the first frame update
    void Start()
    {
        initialForwardVector = transform.forward;
        initialRightVector = transform.right;
        initialWorldToLocalMatrix = transform.worldToLocalMatrix;
    }

    // Update is called once per frame
    void Update()
    {
        var vecSelfToTarget = target.transform.position - transform.position;

        var localVector = initialWorldToLocalMatrix * vecSelfToTarget;

        var verticalAngle = Mathf.Atan2(localVector.y, localVector.z) * Mathf.Rad2Deg;
        var horizontalAngle = Mathf.Atan2(localVector.x, localVector.z) * Mathf.Rad2Deg;

        if (verticalAngle >= - verticalRange / 2 && 
            verticalAngle <= verticalRange / 2 &&
            horizontalAngle >= - horizontalRange / 2 &&
            horizontalAngle <= horizontalRange / 2)
        {
            transform.forward = Vector3.Lerp(transform.forward, vecSelfToTarget, 0.05f);
        }
        else
        {
            transform.forward = Vector3.Lerp(transform.forward, initialForwardVector, 0.3f);
        }
    }


}
