// Copyright HTC Corporation All Rights Reserved.

using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace VIVE.OpenXR
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "VIVE XR Path Enumeration",
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        Company = "HTC",
        Desc = "The extension provides more flexibility for the user paths and input/output source paths related to an interaction profile. Developers can use this extension to obtain the path that the user has decided on.",
        DocumentationLink = "..\\Documentation",
        Version = "1.0.6",
        OpenxrExtensionStrings = kOpenxrExtensionString,
        FeatureId = featureId)]
#endif
    public class VivePathEnumeration : OpenXRFeature
    {
		const string LOG_TAG = "VIVE.OpenXR.VivePathEnumeration ";
        StringBuilder m_sb = null;
        StringBuilder sb {
            get {
                if (m_sb == null) { m_sb = new StringBuilder(); }
                return m_sb;
            }
        }
		void DEBUG(StringBuilder msg) { Debug.Log(msg); }
        void WARNING(StringBuilder msg) { Debug.LogWarning(msg); }

        /// <summary>
        /// OpenXR specification <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#XR_HTC_path_enumeration">12.1. XR_HTC_path_enumeration</see>.
        /// </summary>
        public const string kOpenxrExtensionString = "XR_HTC_path_enumeration";

        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "vive.wave.openxr.feature.pathenumeration";

        #region OpenXR Life Cycle
#pragma warning disable
        private bool m_XrInstanceCreated = false;
#pragma warning enable
        private XrInstance m_XrInstance = 0;
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrCreateInstance">xrCreateInstance</see> is done.
        /// </summary>
        /// <param name="xrInstance">The created instance.</param>
        /// <returns>True for valid <see cref="XrInstance">XrInstance</see></returns>
        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (!OpenXRRuntime.IsExtensionEnabled(kOpenxrExtensionString))
            {
                sb.Clear().Append(LOG_TAG).Append("OnInstanceCreate() ").Append(kOpenxrExtensionString).Append(" is NOT enabled."); WARNING(sb);
                return false;
            }

            m_XrInstanceCreated = true;
            m_XrInstance = xrInstance;
            sb.Clear().Append(LOG_TAG).Append("OnInstanceCreate() ").Append(m_XrInstance); DEBUG(sb);

            return base.OnInstanceCreate(xrInstance);
        }

        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrDestroyInstance">xrDestroyInstance</see> is done.
        /// </summary>
        /// <param name="xrInstance">The instance to destroy.</param>
        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            if (m_XrInstance == xrInstance)
            {
                m_XrInstanceCreated = false;
                m_XrInstance = 0;
            }
            sb.Clear().Append(LOG_TAG).Append("OnInstanceDestroy() ").Append(xrInstance); DEBUG(sb);
        }

        private XrSystemId m_XrSystemId = 0;
        /// <summary>
        /// Called when the <see cref="XrSystemId">XrSystemId</see> retrieved by <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrGetSystem">xrGetSystem</see> is changed.
        /// </summary>
        /// <param name="xrSystem">The system id.</param>
        protected override void OnSystemChange(ulong xrSystem)
        {
            m_XrSystemId = xrSystem;
            sb.Clear().Append(LOG_TAG).Append("OnSystemChange() ").Append(m_XrSystemId); DEBUG(sb);
        }

        private bool m_XrSessionCreated = false;
        private XrSession m_XrSession = 0;
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrCreateSession">xrCreateSession</see> is done.
        /// </summary>
        /// <param name="xrSession">The created session ID.</param>
        protected override void OnSessionCreate(ulong xrSession)
        {
            m_XrSession = xrSession;
            m_XrSessionCreated = true;
            sb.Clear().Append(LOG_TAG).Append("OnSessionCreate() ").Append(m_XrSession); DEBUG(sb);
        }
        /// <summary>
        /// Called when <see href="https://registry.khronos.org/OpenXR/specs/1.0/html/xrspec.html#xrDestroySession">xrDestroySession</see> is done.
        /// </summary>
        /// <param name="xrSession">The session ID to destroy.</param>
        protected override void OnSessionDestroy(ulong xrSession)
        {
            sb.Clear().Append(LOG_TAG).Append("OnSessionDestroy() ").Append(xrSession); DEBUG(sb);
            m_XrSession = 0;
            m_XrSessionCreated = false;
        }
        #endregion
    }
}