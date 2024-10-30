// ===================== 2022 HTC Corporation. All Rights Reserved. ===================

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using VIVE.OpenXR.EyeTracker;

namespace VIVE.OpenXR
{
    public class XR_HTC_eye_tracker_defs
    {
        public virtual XrResult xrCreateEyeTrackerHTC(XrEyeTrackerCreateInfoHTC createInfo, out XrEyeTrackerHTC eyeTracker)
        {
            eyeTracker = 0;
            return XrResult.XR_ERROR_FEATURE_UNSUPPORTED;
        }
        public virtual XrResult xrDestroyEyeTrackerHTC(XrEyeTrackerHTC eyeTracker)
        {
            return XrResult.XR_ERROR_FEATURE_UNSUPPORTED;
        }
        protected XrEyeGazeDataHTC m_eyeGazes = new XrEyeGazeDataHTC(); //= new XrEyeGazeDataHTC(XrStructureType.XR_TYPE_EYE_GAZE_DATA_HTC, IntPtr.Zero, 0);
        public virtual XrResult xrGetEyeGazeDataHTC(XrEyeTrackerHTC eyeTracker, XrEyeGazeDataInfoHTC gazeInfo, out XrEyeGazeDataHTC eyeGazes)
        {
            m_eyeGazes.type = XrStructureType.XR_TYPE_EYE_GAZE_DATA_HTC;
            m_eyeGazes.next = IntPtr.Zero;
            m_eyeGazes.time = 0;
            eyeGazes = m_eyeGazes;
            return XrResult.XR_ERROR_FEATURE_UNSUPPORTED;
        }
        public virtual bool GetEyeGazeData(out XrSingleEyeGazeDataHTC[] out_gazes)
        {
            m_eyeGazes.type = XrStructureType.XR_TYPE_EYE_GAZE_DATA_HTC;
            m_eyeGazes.next = IntPtr.Zero;
            m_eyeGazes.time = 0;
            out_gazes = m_eyeGazes.gaze;
            return false;
        }
        protected XrEyePupilDataHTC m_pupilData = new XrEyePupilDataHTC();
        public virtual XrResult xrGetEyePupilDataHTC(XrEyeTrackerHTC eyeTracker, XrEyePupilDataInfoHTC pupilDataInfo, out XrEyePupilDataHTC pupilData)
        {
            m_pupilData.type = XrStructureType.XR_TYPE_EYE_PUPIL_DATA_HTC;
            m_pupilData.next = IntPtr.Zero;
            m_pupilData.time = 0;
            pupilData = m_pupilData;
            return XrResult.XR_ERROR_FEATURE_UNSUPPORTED;
        }
        public virtual bool GetEyePupilData(out XrSingleEyePupilDataHTC[] pupilData)
        {
            m_pupilData.type = XrStructureType.XR_TYPE_EYE_PUPIL_DATA_HTC;
            m_pupilData.next = IntPtr.Zero;
            m_pupilData.time = 0;
            pupilData = m_pupilData.pupilData;
            return false;
        }
        protected XrEyeGeometricDataHTC m_eyeGeometricData = new XrEyeGeometricDataHTC();
        public virtual XrResult xrGetEyeGeometricDataHTC(XrEyeTrackerHTC eyeTracker,
            XrEyeGeometricDataInfoHTC info,
            out XrEyeGeometricDataHTC eyeGeometricData)
        {
            m_eyeGeometricData.type = XrStructureType.XR_TYPE_EYE_GEOMETRIC_DATA_HTC;
            m_eyeGeometricData.next = IntPtr.Zero;
            m_eyeGeometricData.time = 0;
            eyeGeometricData = m_eyeGeometricData;
            return XrResult.XR_ERROR_FEATURE_UNSUPPORTED;
        }
        public virtual bool GetEyeGeometricData(out XrSingleEyeGeometricDataHTC[] geometricData)
        {
            m_eyeGeometricData.type = XrStructureType.XR_TYPE_EYE_GEOMETRIC_DATA_HTC;
            m_eyeGeometricData.next = IntPtr.Zero;
            m_eyeGeometricData.time = 0;
            geometricData = m_eyeGeometricData.eyeGeometricData;
            return false;
        }
    }
    public class XR_HTC_eye_tracker
    {
        static XR_HTC_eye_tracker_defs m_Instance = null;
        public static XR_HTC_eye_tracker_defs Interop
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new XR_HTC_eye_tracker_impls();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// An application can create an <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> handle using CreateEyeTracker.
        /// </summary>
        /// <param name="createInfo">The <see cref="XrEyeTrackerCreateInfoHTC">XrEyeTrackerCreateInfoHTC</see> used to specify the eye tracker.</param>
        /// <param name="eyeTracker">The returned XrEyeTrackerHTC handle.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public static XrResult xrCreateEyeTrackerHTC(XrEyeTrackerCreateInfoHTC createInfo, out XrEyeTrackerHTC eyeTracker)
        {
            return Interop.xrCreateEyeTrackerHTC(createInfo,out eyeTracker);
        }
        /// <summary>
        /// Releases the eye tracker and the underlying resources when the eye tracking experience is over.
        /// </summary>
        /// <param name="eyeTracker">An XrEyeTrackerHTC previously created by xrCreateEyeTrackerHTC.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public static XrResult xrDestroyEyeTrackerHTC(XrEyeTrackerHTC eyeTracker)
        {
            return Interop.xrDestroyEyeTrackerHTC(eyeTracker);
        }
        /// <summary>
        /// Retrieves the <see cref="XrEyeGazeDataHTC">XrEyeGazeDataHTC</see> data of a <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see>.
        /// </summary>
        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="gazeInfo">The information to get eye gaze.</param>
        /// <param name="eyeGazes">Output parameter to retrieve a pointer to <see cref="XrEyeGazeDataHTC">XrEyeGazeDataHTC</see> receiving the returned eye poses.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public static XrResult xrGetEyeGazeDataHTC(XrEyeTrackerHTC eyeTracker, XrEyeGazeDataInfoHTC gazeInfo, out XrEyeGazeDataHTC eyeGazes)
        {
            return Interop.xrGetEyeGazeDataHTC(eyeTracker, gazeInfo, out eyeGazes);
        }
        /// <summary>
        /// Retrieves the <see cref="XrEyePupilDataHTC">XrEyePupilDataHTC</see> data of a <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see>.
        /// </summary>
        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="pupilDataInfo">The information to get pupil data.</param>
        /// <param name="pupilData">A pointer to <see cref="XrEyePupilDataHTC">XrEyePupilDataHTC</see> returned by the runtime.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public static XrResult xrGetEyePupilDataHTC(XrEyeTrackerHTC eyeTracker, XrEyePupilDataInfoHTC pupilDataInfo, out XrEyePupilDataHTC pupilData)
        {
            return Interop.xrGetEyePupilDataHTC(eyeTracker, pupilDataInfo, out pupilData);
        }
        /// <param name="eyeTracker">An <see cref="XrEyeTrackerHTC">XrEyeTrackerHTC</see> previously created by <see cref="ViveEyeTrackerHelper.xrCreateEyeTrackerHTCDelegate">xrCreateEyeTrackerHTC</see>.</param>
        /// <param name="info">A pointer to <see cref="XrEyeGeometricDataInfoHTC">XrEyeGeometricDataInfoHTC</see> structure.</param>
        /// <param name="eyeGeometricData">A pointer to <see cref="XrEyeGeometricDataHTC">XrEyeGeometricDataHTC</see> returned by the runtime.</param>
        /// <returns>XR_SUCCESS for success.</returns>
        public static XrResult xrGetEyeGeometricDataHTC(XrEyeTrackerHTC eyeTracker,
            XrEyeGeometricDataInfoHTC info,
            out XrEyeGeometricDataHTC eyeGeometricData)
        {
            return Interop.xrGetEyeGeometricDataHTC(eyeTracker,info, out eyeGeometricData);
        }
    }
}
