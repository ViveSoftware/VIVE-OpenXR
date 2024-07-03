// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System;
using System.Text;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	/// <summary>
	/// This class is designed to implement IHandGrabber, allowing objects to grab grabbable objects.
	/// </summary>
	public class HandGrabInteractor : MonoBehaviour, IHandGrabber
	{
		#region Log

		const string LOG_TAG = "VIVE.OpenXR.Toolkits.RealisticHandInteraction.HandGrabInteractor";
		private StringBuilder m_sb = null;
		internal StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		private void DEBUG(string msg) { Debug.Log($"{LOG_TAG}, {msg}"); }
		private void WARNING(string msg) { Debug.LogWarning($"{LOG_TAG}, {msg}"); }
		private void ERROR(string msg) { Debug.LogError($"{LOG_TAG}, {msg}"); }
		int logFrame = 0;
		bool printIntervalLog => logFrame == 0;

		#endregion

		private enum GrabState
		{
			None,
			Hover,
			Grabbing,
		};

		#region Public States 
		private HandGrabInteractable m_Grabbable = null;
		public IGrabbable grabbable => m_Grabbable;
		public bool isGrabbing => m_Grabbable != null;

		[SerializeField]
		private Handedness m_Handedness = Handedness.Left;
		public Handedness handedness => m_Handedness;

		private HandGrabState m_HandGrabState = null;
		public HandGrabState handGrabState => m_HandGrabState;

		[SerializeField]
		private float m_GrabDistance = 0.03f;
		public float grabDistance { get { return m_GrabDistance; } set { m_GrabDistance = value; } }

		public bool isLeft => handedness == Handedness.Left;

		[SerializeField]
		private HandGrabberEvent m_OnBeginGrab = new HandGrabberEvent();
		public HandGrabberEvent onBeginGrab => m_OnBeginGrab;

		[SerializeField]
		private HandGrabberEvent m_OnEndGrab = new HandGrabberEvent();
		public HandGrabberEvent onEndGrab => m_OnEndGrab;
		#endregion

		private readonly float MinGrabScore = 0.25f;
		private readonly float MinDistanceScore = 0.25f;
		private HandGrabInteractable currentCandidate = null;
		private GrabState m_State = GrabState.None;
		private Pose wristPose = Pose.identity;
		private Vector3[] fingerTipPosition = new Vector3[(int)FingerId.Count];
		private OnBeginGrab beginGrabHandler;
		private OnEndGrab endGrabHandler;

		#region MonoBehaviour
		private void Awake()
		{
			m_HandGrabState = new HandGrabState(isLeft);
		}

		private void OnEnable()
		{
			GrabManager.RegisterGrabber(this);
		}

		private void OnDisable()
		{
			GrabManager.UnregisterGrabber(this);
		}

		private void Update()
		{
			m_HandGrabState.UpdateState();

			HandPose handPose = HandPoseProvider.GetHandPose(isLeft ? HandPoseType.HAND_LEFT : HandPoseType.HAND_RIGHT);
			if (handPose != null)
			{
				handPose.GetPosition(JointType.Wrist, out wristPose.position);
				handPose.GetRotation(JointType.Wrist, out wristPose.rotation);
				handPose.GetPosition(JointType.Thumb_Tip, out fingerTipPosition[(int)FingerId.Thumb]);
				handPose.GetPosition(JointType.Index_Tip, out fingerTipPosition[(int)FingerId.Index]);
				handPose.GetPosition(JointType.Middle_Tip, out fingerTipPosition[(int)FingerId.Middle]);
				handPose.GetPosition(JointType.Ring_Tip, out fingerTipPosition[(int)FingerId.Ring]);
				handPose.GetPosition(JointType.Pinky_Tip, out fingerTipPosition[(int)FingerId.Pinky]);
			}

			if (m_State != GrabState.Grabbing)
			{
				FindCandidate();
			}
			switch (m_State)
			{
				case GrabState.None:
					NoneUpdate();
					break;
				case GrabState.Hover:
					HoverUpdate();
					break;
				case GrabState.Grabbing:
					GrabbingUpdate();
					break;
			}
		}
		#endregion

		#region Public Interface
		/// <summary>
		/// Checks if the specified joint type is required.
		/// </summary>
		/// <param name="joint">The joint that need to check.</param>
		/// <returns>True if the joint is required; otherwise, false.</returns>
		public bool IsRequiredJoint(JointType joint)
		{
			if (m_Grabbable != null)
			{
				HandData.GetJointIndex(joint, out int group, out _);
				switch (group)
				{
					case 2: return m_Grabbable.fingerRequirement.thumb == GrabRequirement.Required;
					case 3: return m_Grabbable.fingerRequirement.index == GrabRequirement.Required;
					case 4: return m_Grabbable.fingerRequirement.middle == GrabRequirement.Required;
					case 5: return m_Grabbable.fingerRequirement.ring == GrabRequirement.Required;
					case 6: return m_Grabbable.fingerRequirement.pinky == GrabRequirement.Required;
				}
			}
			return false;
		}

		/// <summary>
		/// Add a listener for the event triggered when the grabber begins grabbing.
		/// </summary>
		/// <param name="handler">The method to be called when the grabber begins grabbing.</param>
		[Obsolete("Please use onBeginGrab instead.")]
		public void AddBeginGrabListener(OnBeginGrab handler)
		{
			beginGrabHandler += handler;
		}

		/// <summary>
		/// Remove a listener for the event triggered when the grabber begins grabbing.
		/// </summary>
		/// <param name="handler">The method to be removed from the event listeners.</param>
		[Obsolete("Please use onBeginGrab instead.")]
		public void RemoveBeginGrabListener(OnBeginGrab handler)
		{
			beginGrabHandler -= handler;
		}

		/// <summary>
		/// Add a listener for the event triggered when the grabber ends grabbing.
		/// </summary>
		/// <param name="handler">The method to be called when the grabber ends grabbing.</param>
		[Obsolete("Please use onEndGrab instead.")]
		public void AddEndGrabListener(OnEndGrab handler)
		{
			endGrabHandler += handler;
		}

		/// <summary>
		/// Remove a listener for the event triggered when the grabber ends grabbing.
		/// </summary>
		/// <param name="handler">The method to be removed from the event listeners.</param>
		[Obsolete("Please use onEndGrab instead.")]
		public void RemoveEndGrabListener(OnEndGrab handler)
		{
			endGrabHandler -= handler;
		}
		#endregion

		/// <summary>
		/// Find the candidate grabbable object for grabber.
		/// </summary>
		private void FindCandidate()
		{
			currentCandidate = null;
			float distanceScore = float.MinValue;
			if (GetClosestGrabbable(m_GrabDistance, out HandGrabInteractable grabbable, out float score) && score > distanceScore)
			{
				distanceScore = score;
				currentCandidate = grabbable;
			}

			if (currentCandidate != null)
			{
				float grabScore = Grab.CalculateHandGrabScore(this, currentCandidate);
				if (distanceScore < MinDistanceScore || grabScore < MinGrabScore)
				{
					currentCandidate = null;
				}
			}
		}

		/// <summary>
		/// Get the closest grabbable object for grabber.
		/// </summary>
		/// <param name="grabDistance">The maximum grab distance between the grabber and the grabbable object.</param>
		/// <param name="grabbable">The closest grabbable object.</param>
		/// <param name="maxScore">The maximum score indicating the closeness of the grabbable object.</param>
		/// <returns>True if a grabbable object is found within the grab distance; otherwise, false.</returns>
		private bool GetClosestGrabbable(float grabDistance, out HandGrabInteractable grabbable, out float maxScore)
		{
			grabbable = null;
			maxScore = 0f;
			foreach (HandGrabInteractable interactable in GrabManager.handGrabbables)
			{
				interactable.ShowIndicator(false, this);

				foreach (Vector3 tipPos in fingerTipPosition)
				{
					float distanceScore = interactable.CalculateDistanceScore(tipPos, grabDistance);
					if (distanceScore > maxScore)
					{
						maxScore = distanceScore;
						grabbable = interactable;
					}
				}
			}
			if (grabbable != null)
			{
				grabbable.ShowIndicator(true, this);
			}
			return grabbable != null;
		}

		/// <summary>
		/// Set the state to GrabState.Hover if a candidate is found.
		/// </summary>
		private void NoneUpdate()
		{
			if (currentCandidate != null)
			{
				m_State = GrabState.Hover;
			}
		}

		/// <summary>
		/// Update the state and related information when the grabber begins grabbing the grabbable.
		/// </summary>
		private void HoverUpdate()
		{
			if (currentCandidate == null)
			{
				m_State = GrabState.None;
				return;
			}

			if (Grab.HandBeginGrab(this, currentCandidate))
			{
				m_State = GrabState.Grabbing;
				m_Grabbable = currentCandidate;
				m_Grabbable.SetGrabber(this);
				m_Grabbable.ShowIndicator(false, this);
				beginGrabHandler?.Invoke(this);
				onBeginGrab?.Invoke(this);

				DEBUG($"The {(m_Handedness == Handedness.Left ? "left" : "right")} hand begins to grab the {m_Grabbable.name}");
			}
		}

		/// <summary>
		/// Update the position of grabbable object according to the movement of the grabber.
		/// </summary>
		private void GrabbingUpdate()
		{
			if (Grab.HandDoneGrab(this, m_Grabbable) || !Grab.HandIsGrabbing(this, m_Grabbable))
			{
				DEBUG($"The {(m_Handedness == Handedness.Left ? "left" : "right")} hand ends to grab the {m_Grabbable.name}");

				endGrabHandler?.Invoke(this);
				onEndGrab?.Invoke(this);
				m_Grabbable.SetGrabber(null);
				m_Grabbable = null;
				m_State = GrabState.Hover;
				return;
			}
			m_Grabbable.UpdatePositionAndRotation(wristPose);
		}
	}
}
