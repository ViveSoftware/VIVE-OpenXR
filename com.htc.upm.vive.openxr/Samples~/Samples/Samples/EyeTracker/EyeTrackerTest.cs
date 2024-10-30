using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VIVE.OpenXR.EyeTracker;

namespace VIVE.OpenXR.Samples.OpenXRInput
{
    public class EyeTrackerTest : MonoBehaviour
    {
        const string LOG_TAG = "VIVE.OpenXR.Samples.OpenXRInput.EyeTrackerText";
        void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
        public Transform leftGazeTransform = null;
        public Transform rightGazeTransform = null;

        private Text m_Text = null;

        private void Awake()
        {
            m_Text = GetComponent<Text>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            m_Text.text = "[Eye Tracker]\n";
            m_Text.text += "Left Gaze:\n";
            XR_HTC_eye_tracker.Interop.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] out_gazes);
            XrSingleEyeGazeDataHTC leftGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            m_Text.text += "isValid: " + leftGaze.isValid + "\n";
            m_Text.text += "position ( " + leftGaze.gazePose.position.ToUnityVector().x.ToString("F4") + ", "+leftGaze.gazePose.position.ToUnityVector().y.ToString("F4") + ", " + leftGaze.gazePose.position.ToUnityVector().z.ToString("F4") + ")\n";
            m_Text.text += "rotation ( " + leftGaze.gazePose.orientation.ToOpenXRQuaternion().x.ToString("F4") + ", "+leftGaze.gazePose.orientation.ToOpenXRQuaternion().y.ToString("F4") + ", " + leftGaze.gazePose.orientation.ToOpenXRQuaternion().z.ToString("F4") + ", " + leftGaze.gazePose.orientation.ToOpenXRQuaternion().w.ToString("F4") + ")\n";
            leftGazeTransform.position = leftGaze.gazePose.position.ToUnityVector();
            leftGazeTransform.rotation = leftGaze.gazePose.orientation.ToUnityQuaternion();
            m_Text.text += "Right Gaze:\n";
            XrSingleEyeGazeDataHTC rightGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            m_Text.text += "isValid: " + rightGaze.isValid + "\n";
            m_Text.text += "position ( " + rightGaze.gazePose.position.ToUnityVector().x.ToString("F4") + ", " + rightGaze.gazePose.position.ToUnityVector().y.ToString("F4") + ", " + rightGaze.gazePose.position.ToUnityVector().z.ToString("F4") + ")\n";
            m_Text.text += "rotation ( " + rightGaze.gazePose.orientation.ToOpenXRQuaternion().x.ToString("F4") + ", " +rightGaze.gazePose.orientation.ToOpenXRQuaternion().y.ToString("F4") + ", " + rightGaze.gazePose.orientation.ToOpenXRQuaternion().z.ToString("F4") + ", " + rightGaze.gazePose.orientation.ToOpenXRQuaternion().w.ToString("F4") + ")\n\n";
            rightGazeTransform.position = rightGaze.gazePose.position.ToUnityVector();
            rightGazeTransform.rotation = rightGaze.gazePose.orientation.ToUnityQuaternion();

            m_Text.text += "Left Pupil:\n";
            XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] out_pupils);
            XrSingleEyePupilDataHTC leftPupil =  out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            m_Text.text += "isDiameterValid: " + leftPupil.isDiameterValid + "\tisPositionValid: " + leftPupil.isPositionValid+ "\n";
            m_Text.text += "pupilDiameter: " + leftPupil.pupilDiameter + "\n";
            m_Text.text += "position ( " + leftPupil.pupilPosition.x.ToString("F4") + ", " + leftPupil.pupilPosition.y.ToString("F4") + ")\n";
            //m_Text.text += "tracked: " + tracked + "\n";
            m_Text.text += "Right Pupil:\n";
            XrSingleEyePupilDataHTC rightPupil = out_pupils[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            m_Text.text += "isDiameterValid: " + rightPupil.isDiameterValid + "\tisPositionValid: " + rightPupil.isPositionValid + "\n";
            m_Text.text += "pupilDiameter: " + rightPupil.pupilDiameter + "\n";
            m_Text.text += "position ( " + rightPupil.pupilPosition.x.ToString("F4") + ", " + rightPupil.pupilPosition.y.ToString("F4") + ")\n\n";

            m_Text.text += "Left eye geometric:\n";
            XR_HTC_eye_tracker.Interop.GetEyeGeometricData(out XrSingleEyeGeometricDataHTC[] out_geometrics);
            XrSingleEyeGeometricDataHTC leftGeometric =  out_geometrics[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
            m_Text.text += "isValid: " + leftGeometric.isValid + "\n";
            m_Text.text += "Openness: " + leftGeometric.eyeOpenness.ToString("F3") + "\tSqueeze: " + leftGeometric.eyeSqueeze.ToString("F3") + "\tWide: " + leftGeometric.eyeWide.ToString("F3") +  "\n";
            m_Text.text += "Right eye geometric:\n";
            XrSingleEyeGeometricDataHTC rightGeometric = out_geometrics[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];
            m_Text.text += "isValid: " + rightGeometric.isValid + "\n";
            m_Text.text += "Openness: " + rightGeometric.eyeOpenness.ToString("F3") + "\tSqueeze: " + rightGeometric.eyeSqueeze.ToString("F3") + "\tWide: " + rightGeometric.eyeWide.ToString("F3") + "\n";
        }
    }

}
