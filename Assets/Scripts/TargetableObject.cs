using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetableObject : MonoBehaviour
{
    [SerializeField] Image m_targetMarker;
    private bool _isTargetMarkerVisible;
    public bool IsTargetMarkerVisible
    {
        get { return _isTargetMarkerVisible; }
        set
        {
            _isTargetMarkerVisible = value;
            if (m_targetMarker != null)
            {
                m_targetMarker.enabled = value;
            }
        }
    }
}
