using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using System;
namespace VIVE.FacialTracking
{
    public class FacialManager 
    {
        private class FacialData
        {
            public bool isStarted = false;
            public XrFacialTrackingTypeHTC trackingType;
            public bool isActive;
            public ulong trackerHandle;
            public int updatedFrame = -1;
            public Dictionary<XrEyeShapeHTC, float> eyeWeightings = new Dictionary<XrEyeShapeHTC, float>();
            public Dictionary<XrLipShapeHTC, float> LipWeightings = new Dictionary<XrLipShapeHTC, float>();
            public float[] blendshapes;
            public bool isCreated { get { return trackerHandle != default; } }
            public FacialData(XrFacialTrackingTypeHTC type)
            {
                trackingType = type;
                for (int i = 0; i < (int)XrEyeShapeHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC; ++i) eyeWeightings.Add((XrEyeShapeHTC)i, 0.0f);
                for (int i = 0; i < (int)XrLipShapeHTC.XR_LIP_SHAPE_MAX_ENUM_HTC; ++i) LipWeightings.Add((XrLipShapeHTC)i, 0.0f);
                int shapeSize = type == XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC
                    ? (int)XrEyeShapeHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC : (int)XrLipShapeHTC.XR_LIP_SHAPE_MAX_ENUM_HTC;
                blendshapes = new float[shapeSize];
            }
            public void ClearData()
            {
                Array.Clear(blendshapes, 0, blendshapes.Length);
                eyeWeightings.Clear();
                LipWeightings.Clear();
            }
        }

        private FacialData eyeFacialData = new FacialData(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC);
        private FacialData lipFacialData = new FacialData(XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_LIP_DEFAULT_HTC);
        private VIVE_FacialTracking_OpenXR_API feature;
        private static bool isInitialized;
        private static bool isSystemSupportEye;
        private static bool isSystemSupportLip;

        private FacialData getFacialData(XrFacialTrackingTypeHTC type)
        {
            if (type == XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC)
                return eyeFacialData;
            else
                return lipFacialData;
        }

        private void OnFeatureSessionCreate(ulong xrSession)
        {
            TryCreateFacialTracker(eyeFacialData);
            TryCreateFacialTracker(lipFacialData);
        }
        private void OnFeatureSessionDestroy(ulong xrSession)
        {
            TryDestroyFacialTracker(eyeFacialData);
            TryDestroyFacialTracker(lipFacialData);
        }

        private void OnFeatureSystemChange(ulong systemId)
        {
            UpdateSystemSupported();
        }

        private void UpdateSystemSupported()
        {
            if (feature == null || !feature.IsEnabledAndInitialized)
            {
                isSystemSupportEye = false;
                isSystemSupportLip = false;
            }
            else
            {
                feature.SystemSupportsFacialTracking(out var result,out isSystemSupportEye,out isSystemSupportLip);
                if (result != XrResult.XR_SUCCESS)
                {
                    Debug.LogWarning("Fail SystemSupportsHandTracking: " + result);
                }
                if (!isSystemSupportEye)
                {
                    UnityEngine.Debug.Log("Initial eyetracking failed , the device may not support EyeExpression");
                }
                if (!isSystemSupportLip)
                {
                    UnityEngine.Debug.Log("Initial liptracking failed , the device may not support LipExpression");
                }
            }
        }


        public bool Initialize()
        {
            if(feature == null)
                feature = OpenXRSettings.Instance.GetFeature<VIVE_FacialTracking_OpenXR_API>();
            if (!isInitialized)
            {
                if (feature != null)
                {
                    feature.onSessionCreate += OnFeatureSessionCreate;
                    feature.onSessionDestroy += OnFeatureSessionDestroy;
                    feature.onSystemChange += OnFeatureSystemChange;
                }
                if (feature != null && feature.IsEnabledAndInitialized)
                {
                    UpdateSystemSupported();
                    isInitialized = true;
                }
            }

            return isInitialized;
        }
        private bool TryCreateFacialTracker(FacialData facialData)
        {
            if (!facialData.isStarted) return false;
            if (!Initialize()) { return false; }
            if (facialData.trackingType == XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC && !isSystemSupportEye) { return false; }
            if (facialData.trackingType == XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_LIP_DEFAULT_HTC && !isSystemSupportLip) { return false; }
            if (!feature.IsSessionCreated) { return false; }
            if (!facialData.isCreated)
            {
                if (!feature.TryCreateFacialTracker(facialData.trackingType, out facialData.trackerHandle, out var result))
                {
                    facialData.trackerHandle = default;
                    Debug.LogWarning("Fail CreateFacialTracker: " + result);
                }
            }

            return facialData.isCreated;
        }

        private void TryDestroyFacialTracker(FacialData facialData)
        {
            if (!facialData.isCreated) { return; }
            if (!Initialize()) { return; }

            if(!feature.TryDestroFacialTracker(facialData.trackerHandle, out var res))
            {
                Debug.LogWarning("Fail DestroyFacialTracker: " + res);
            }
            else
            {
                Debug.Log("Success DestroyFacialTracker: " + res);
            }
            facialData.trackerHandle = default;
            facialData.ClearData();
        }
        private void TryGetFacialData(FacialData facialData)
        {
            if(facialData.isCreated)
            {
                if(facialData.updatedFrame != Time.frameCount)
                {
                    int maxExpressionCount = facialData.trackingType == XrFacialTrackingTypeHTC.XR_FACIAL_TRACKING_TYPE_EYE_DEFAULT_HTC 
                        ? (int)XrEyeShapeHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC : (int)XrLipShapeHTC.XR_LIP_SHAPE_MAX_ENUM_HTC;
                    facialData.updatedFrame = Time.frameCount;
                    if (!feature.TryGetFacialData(
                        facialData.trackerHandle,
                        out facialData.isActive,
                        maxExpressionCount,
                        out var result,
                        facialData.blendshapes))
                    {
                        facialData.isActive = false;
                        Debug.LogWarning("Fail TryGetFacialData: " + result);
                    }
                }

            }

        }
        public void StartFramework(XrFacialTrackingTypeHTC type) 
        {
            var facialdata = getFacialData(type);
            facialdata.isStarted = true;
            TryCreateFacialTracker(facialdata);
        }
        public void StopFramework(XrFacialTrackingTypeHTC type) 
        {
            var facialdata = getFacialData(type);
            facialdata.isStarted = false;
            TryDestroyFacialTracker(facialdata);
        }
        public bool GetWeightings<T>(out Dictionary<T, float> shapes)
        {
            if (typeof(T) == typeof(XrEyeShapeHTC))
            {
                TryGetFacialData(eyeFacialData);
                for (int i = 0; i < (int)XrEyeShapeHTC.XR_EYE_EXPRESSION_MAX_ENUM_HTC; ++i)
                {
                    eyeFacialData.eyeWeightings[(XrEyeShapeHTC)i] = eyeFacialData.blendshapes[i];
                }
                shapes = (Dictionary<T, float>)(System.Object)(eyeFacialData.eyeWeightings);
                return true;
            }
            else if (typeof(T) == typeof(XrLipShapeHTC))
            {
                TryGetFacialData(lipFacialData);
                for (int i = 0; i < (int)XrLipShapeHTC.XR_LIP_SHAPE_MAX_ENUM_HTC; ++i)
                {
                    lipFacialData.LipWeightings[(XrLipShapeHTC)i] = lipFacialData.blendshapes[i];
                }
                shapes = (Dictionary<T, float>)(System.Object)(lipFacialData.LipWeightings);
                return true;
            }
            else
            {
                shapes = default;
                return false;
            }
        }

    }
}