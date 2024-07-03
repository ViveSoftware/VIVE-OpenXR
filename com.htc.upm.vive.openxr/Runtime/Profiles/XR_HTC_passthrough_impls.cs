using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using VIVE.OpenXR.CompositionLayer;
using VIVE.OpenXR.CompositionLayer.Passthrough;

namespace VIVE.OpenXR
{
    public class XR_HTC_passthrough_impls : XR_HTC_passthrough_defs
    {
        const string LOG_TAG = "VIVE.OpenXR.XR_HTC_passthrough_impls";
        void DEBUG(string msg) { Debug.Log(LOG_TAG + " " + msg); }
        public XR_HTC_passthrough_impls() { DEBUG("XR_HTC_passthrough_impls()"); }
        private ViveCompositionLayerPassthrough feature = null;

        private void ASSERT_FEATURE()
        {
            if (feature == null) { feature = OpenXRSettings.Instance.GetFeature<ViveCompositionLayerPassthrough>(); }
        }

        public override XrResult xrCreatePassthroughHTC(XrPassthroughCreateInfoHTC createInfo, out XrPassthroughHTC passthrough)
        {
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;
            passthrough = 0;
            ASSERT_FEATURE();
#if UNITY_STANDALONE
            if(feature)
                result =  feature.CreatePassthroughHTC(createInfo,out passthrough);
#endif
            return result;
        }
        public override XrResult xrDestroyPassthroughHTC(XrPassthroughHTC passthrough)
        {
            XrResult result = XrResult.XR_ERROR_VALIDATION_FAILURE;
            ASSERT_FEATURE();
#if UNITY_STANDALONE
            if(feature)
                result =  feature.DestroyPassthroughHTC(passthrough);
#endif
            return result;
        }

        public override void GetOriginEndFrameLayerList(out List<IntPtr> layers)
        {
            ASSERT_FEATURE();
            layers = new List<IntPtr>();
#if UNITY_STANDALONE
            if (feature)
                feature.GetOriginEndFrameLayerList(out layers);

            else
                layers = new List<IntPtr>();
#endif
        }
        public override void SubmitLayers(List<IntPtr> layers)
        {
            ASSERT_FEATURE();
#if UNITY_STANDALONE
            if (feature)
                feature.SubmitLayers(layers);
#endif
        }

        public override XrSpace GetTrackingSpace()
        {
            ASSERT_FEATURE();
#if UNITY_STANDALONE
            if (feature)
                return feature.GetTrackingSpace();
#endif
            return 0;
        }

        public override XrFrameState GetFrameState()
        {
            ASSERT_FEATURE();
#if UNITY_STANDALONE
            if (feature)
                return feature.GetFrameState();
#endif
            return new XrFrameState();
        }
    }
}

