// ===================== 2022 HTC Corporation. All Rights Reserved. ===================

using UnityEngine;

using UnityEngine.XR.OpenXR;

using VIVE.OpenXR.EyeTracker;


namespace VIVE.OpenXR
{
    public class XR_HTC_eye_tracker_impls : XR_HTC_eye_tracker_defs
    {
        const string LOG_TAG = "VIVE.OpenXR.XR_HTC_eye_tracker_impls";
        void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }

        private ViveEyeTracker feature = null;
        private void ASSERT_FEATURE()
        {
            if (feature == null) { feature = OpenXRSettings.Instance.GetFeature<ViveEyeTracker>(); }
        }

        public override XrResult xrCreateEyeTrackerHTC(XrEyeTrackerCreateInfoHTC createInfo, out XrEyeTrackerHTC eyeTracker)
        {
            DEBUG("xrCreateEyeTrackerHTC");
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;
            eyeTracker = 0;

            ASSERT_FEATURE();
            if (feature)
            {
                result = (XrResult)feature.CreateEyeTracker(createInfo, out XrEyeTrackerHTC value);
                if (result == XrResult.XR_SUCCESS) { eyeTracker = value; }
            }
            return result;
        }
        public override XrResult xrDestroyEyeTrackerHTC(XrEyeTrackerHTC eyeTracker)
        {
            DEBUG("xrDestroyEyeTrackerHTC");

            ASSERT_FEATURE();
            if (feature) { return (XrResult)feature.DestroyEyeTracker(eyeTracker); }

            return XrResult.XR_ERROR_VALIDATION_FAILURE;
        }
        public override XrResult xrGetEyeGazeDataHTC(XrEyeTrackerHTC eyeTracker, XrEyeGazeDataInfoHTC gazeInfo, out XrEyeGazeDataHTC eyeGazes)
        {
            eyeGazes = m_eyeGazes;
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;

            ASSERT_FEATURE();
            if (feature)
            {
                result = (XrResult)feature.GetEyeGazeData(eyeTracker, gazeInfo, out XrEyeGazeDataHTC gazes);
                if (result == XrResult.XR_SUCCESS) { eyeGazes = gazes; }
            }
            return result;
        }

        public override bool GetEyeGazeData(out XrSingleEyeGazeDataHTC[] out_gazes)
        {
            out_gazes = m_eyeGazes.gaze;
            bool result = false;

            ASSERT_FEATURE();
            if (feature)
            {
                result = feature.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] data);
                if (result) out_gazes = data;
            }
            return result;
        }

        public override XrResult xrGetEyePupilDataHTC(XrEyeTrackerHTC eyeTracker, XrEyePupilDataInfoHTC pupilDataInfo, out XrEyePupilDataHTC pupilData)
        {
            pupilData = m_pupilData;
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;

            ASSERT_FEATURE();
            if (feature)
            {
                result = (XrResult)feature.GetEyePupilData(eyeTracker, pupilDataInfo, out XrEyePupilDataHTC data);
                if (result == XrResult.XR_SUCCESS) { pupilData = data; }
            }
            return result;
        }

        public override bool GetEyePupilData(out XrSingleEyePupilDataHTC[] pupilData)
        {
            pupilData = m_pupilData.pupilData;
            bool result = false;

            ASSERT_FEATURE();
            if (feature)
            {
                result = feature.GetEyePupilData(out XrSingleEyePupilDataHTC[] data);
                if (result) pupilData = data;
            }
            return result;
        }
        public override XrResult xrGetEyeGeometricDataHTC(XrEyeTrackerHTC eyeTracker,
            XrEyeGeometricDataInfoHTC info,
            out XrEyeGeometricDataHTC eyeGeometricData)
        {
            eyeGeometricData = m_eyeGeometricData;
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;

            ASSERT_FEATURE();
            if (feature)
            {
                result = (XrResult)feature.GetEyeGeometricData(eyeTracker, info, out XrEyeGeometricDataHTC geometricData);
                if (result == XrResult.XR_SUCCESS) { eyeGeometricData = geometricData; }
            }
            return result;
        }


        public override bool GetEyeGeometricData(out XrSingleEyeGeometricDataHTC[] geometricData)
        {
            geometricData = m_eyeGeometricData.eyeGeometricData;
            bool result = false;

            ASSERT_FEATURE();
            if (feature)
            {
                result = feature.GetEyeGeometricData(out XrSingleEyeGeometricDataHTC[] data);
                if (result) geometricData = data;
            }
            return result;
        }
    }
}
