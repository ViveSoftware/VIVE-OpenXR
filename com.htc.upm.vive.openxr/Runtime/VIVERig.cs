// "VIVE SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the VIVE SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VIVE.OpenXR
{
	[DisallowMultipleComponent]
	public sealed class VIVERig : MonoBehaviour
	{
		const string LOG_TAG = "VIVE.OpenXR.VIVERig";
		void DEBUG(string msg)
		{
            Debug.Log(LOG_TAG + " " + msg);
		}

		[SerializeField]
		private GameObject m_CameraOffset = null;
		public GameObject CameraOffset { get { return m_CameraOffset; } set { m_CameraOffset = value; } }

		[SerializeField]
		private GameObject m_CameraObject = null;
		[System.Obsolete("No Used")]
		public GameObject CameraObject { get { return m_CameraObject; } set { m_CameraObject = value; } }

		private TrackingOriginModeFlags m_TrackingOriginEx = TrackingOriginModeFlags.Device;
		[SerializeField]
		private TrackingOriginModeFlags m_TrackingOrigin = TrackingOriginModeFlags.Device;
		public TrackingOriginModeFlags TrackingOrigin { get { return m_TrackingOrigin; } set { m_TrackingOrigin = value; } }

		private Vector3 cameraPosOffset = Vector3.zero;
		[SerializeField]
		private float m_CameraYOffset = 1;
		public float CameraYOffset { get { return m_CameraYOffset; } set { m_CameraYOffset = value; } }

        static List<XRInputSubsystem> s_InputSubsystems = new List<XRInputSubsystem>();
        private void OnEnable()
        {
            SubsystemManager.GetInstances(s_InputSubsystems);
            for (int i = 0; i < s_InputSubsystems.Count; i++)
            {
                s_InputSubsystems[i].trackingOriginUpdated += TrackingOriginUpdated;
            }
        }
        private void OnDisable()
        {
            SubsystemManager.GetInstances(s_InputSubsystems);
            for (int i = 0; i < s_InputSubsystems.Count; i++)
            {
                s_InputSubsystems[i].trackingOriginUpdated -= TrackingOriginUpdated;
            }
        }

        float m_LastRecenteredTime = 0.0f;
        private void TrackingOriginUpdated(XRInputSubsystem obj)
        {
            m_LastRecenteredTime = Time.time;
            DEBUG("TrackingOriginUpdated() m_LastRecenteredTime: " + m_LastRecenteredTime);
        }

        XRInputSubsystem m_InputSystem = null;
		void UpdateInputSystem()
		{
			SubsystemManager.GetInstances(s_InputSubsystems);
			if (s_InputSubsystems.Count > 0)
			{
				m_InputSystem = s_InputSubsystems[0];
			}
		}
		private void Awake()
		{
			UpdateInputSystem();
			if (m_InputSystem != null)
			{
				m_InputSystem.TrySetTrackingOriginMode(m_TrackingOrigin);

				TrackingOriginModeFlags mode = m_InputSystem.GetTrackingOriginMode();
				DEBUG("Awake() Tracking mode is set to " + mode);
            }
            else
            {
                DEBUG("Awake() no XRInputSubsystem.");
            }
			m_TrackingOriginEx = m_TrackingOrigin;
		}

		private void Update()
		{
			UpdateInputSystem();
			if (m_InputSystem != null)
			{
				TrackingOriginModeFlags mode = m_InputSystem.GetTrackingOriginMode();
				if ((mode != m_TrackingOrigin || m_TrackingOriginEx != m_TrackingOrigin) && m_TrackingOrigin != TrackingOriginModeFlags.Unknown)
				{
					m_InputSystem.TrySetTrackingOriginMode(m_TrackingOrigin);

					mode = m_InputSystem.GetTrackingOriginMode();
					DEBUG("Update() Tracking mode is set to " + mode);
					m_TrackingOriginEx = m_TrackingOrigin;
				}
			}

			if (m_CameraOffset != null)
			{
				cameraPosOffset.x = m_CameraOffset.transform.localPosition.x;
				cameraPosOffset.y = m_CameraYOffset;
				cameraPosOffset.z = m_CameraOffset.transform.localPosition.z;

				m_CameraOffset.transform.localPosition = cameraPosOffset;
			}
		}
	}
}
