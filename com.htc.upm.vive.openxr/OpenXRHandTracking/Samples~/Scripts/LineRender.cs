using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

#if USE_INPUT_SYSTEM_POSE_CONTROL
using Pose = UnityEngine.InputSystem.XR.PoseState;
#else
using Pose = UnityEngine.XR.OpenXR.Input.Pose;
#endif

public class LineRender : MonoBehaviour
{
    [Tooltip("Root object of Line mesh")]
    public GameObject Line = null;
    [SerializeField] private LineRenderer GazeRayRenderer;

    [SerializeField]
    private InputActionReference m_ActionReferencePose;
    public InputActionReference actionReferencePose { get => m_ActionReferencePose; set => m_ActionReferencePose = value; }

    // Update is called once per frame
    void Update()
    {
        Vector3 DirectionCombinedLocal;
        if (actionReferencePose != null && actionReferencePose.action != null
            && actionReferencePose.action.enabled && actionReferencePose.action.controls.Count > 0)
        {
            //GazeRayRenderer.SetActive(true);
            Pose poseval = actionReferencePose.action.ReadValue<Pose>();
            Quaternion gazeRotation = poseval.rotation;
            Quaternion orientation = new Quaternion(
                1 * (gazeRotation.x),
                1 * (gazeRotation.y),
                1 * gazeRotation.z,
                1 * gazeRotation.w);
            DirectionCombinedLocal = orientation * Vector3.forward;
            Vector3 DirectionCombined = Camera.main.transform.TransformDirection(DirectionCombinedLocal);
            GazeRayRenderer.SetPosition(0, poseval.position);
            GazeRayRenderer.SetPosition(1, poseval.position + DirectionCombinedLocal * 4);
        }
    }

}
