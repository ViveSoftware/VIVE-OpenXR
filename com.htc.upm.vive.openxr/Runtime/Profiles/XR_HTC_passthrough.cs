using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIVE.OpenXR.CompositionLayer;
using VIVE.OpenXR.CompositionLayer.Passthrough;

namespace VIVE.OpenXR
{
    public class XR_HTC_passthrough_defs
    {
        public virtual XrResult xrCreatePassthroughHTC(XrPassthroughCreateInfoHTC createInfo, out XrPassthroughHTC passthrough)
        {
            passthrough = 0;
            return XrResult.XR_ERROR_RUNTIME_FAILURE;
        }
        public virtual XrResult xrDestroyPassthroughHTC(XrPassthroughHTC passthrough)
        {
            return XrResult.XR_ERROR_RUNTIME_FAILURE;
        }

        public virtual void GetOriginEndFrameLayerList(out List<IntPtr> layers)
        {
            layers = new List<IntPtr>();
        }

        public virtual void SubmitLayers(List<IntPtr> layers)
        {
        }
        public virtual XrSpace GetTrackingSpace()
        {
            return 0;
        }

        public virtual XrFrameState GetFrameState()
        {
            return new XrFrameState();
        }

    }
    public static class XR_HTC_passthrough
    {
        static XR_HTC_passthrough_defs m_Instance = null;
        public static XR_HTC_passthrough_defs Interop
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new XR_HTC_passthrough_impls();
                }
                return m_Instance;
            }
        }

        public static XrResult xrCreatePassthroughHTC(XrPassthroughCreateInfoHTC createInfo, out XrPassthroughHTC passthrough) 
        { 
            return Interop.xrCreatePassthroughHTC(createInfo,out passthrough); 
        }
        public static XrResult xrDestroyPassthroughHTC(XrPassthroughHTC passthrough) 
        { 
            return Interop.xrDestroyPassthroughHTC(passthrough); 
        }
    }
}

