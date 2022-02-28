using System;
using UnityEngine;
using UnityStandardAssets.Utility;

public class HeadController : MonoBehaviour
{
    public Transform cameraRoot;
    public CurveControlledBob motionBob = new CurveControlledBob();
    public LerpControlledBob jumpAndLandingBob = new LerpControlledBob();
    public FPController rigidbodyFirstPersonController;
    public float StrideInterval;
    [Range(0f, 1f)] public float RunningStrideLengthen;

    // private CameraRefocus m_CameraRefocus;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;


    private void Start()
    {
        motionBob.Setup(cameraRoot, StrideInterval);
        m_OriginalCameraPosition = cameraRoot.localPosition;
        //     m_CameraRefocus = new CameraRefocus(Camera, transform.root.transform, cameraRoot.localPosition);
    }


    private void Update()
    {
        //  m_CameraRefocus.GetFocusPoint();
        Vector3 newCameraPosition;
        if (rigidbodyFirstPersonController.Velocity.magnitude > 0 && rigidbodyFirstPersonController.Grounded)
        {
            cameraRoot.localPosition = motionBob.DoHeadBob(rigidbodyFirstPersonController.Velocity.magnitude * (rigidbodyFirstPersonController.Running ? RunningStrideLengthen : 1f));
            newCameraPosition = cameraRoot.localPosition;
            newCameraPosition.y = cameraRoot.localPosition.y - jumpAndLandingBob.Offset();
        }
        else
        {
            newCameraPosition = cameraRoot.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - jumpAndLandingBob.Offset();
        }
        cameraRoot.localPosition = newCameraPosition;

        if (!m_PreviouslyGrounded && rigidbodyFirstPersonController.Grounded)
        {
            StartCoroutine(jumpAndLandingBob.DoBobCycle());
        }

        m_PreviouslyGrounded = rigidbodyFirstPersonController.Grounded;
        //  m_CameraRefocus.SetFocusPoint();
    }
}

