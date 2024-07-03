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
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	/// <summary>
	/// This class is designed to implement IHandGrabbable, allowing objects to be grabbed.
	/// </summary>
	public class HandGrabInteractable : MonoBehaviour, IHandGrabbable
	{
		#region Log

		const string LOG_TAG = "VIVE.OpenXR.Toolkits.RealisticHandInteraction.HandGrabInteractable";
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

		#region Interface Implement
		private HandGrabInteractor m_Grabber = null;
		public IGrabber grabber => m_Grabber;

		public bool isGrabbed => m_Grabber != null;

		[SerializeField]
		private bool m_IsGrabbable = true;
		public bool isGrabbable { get { return m_IsGrabbable; } set { m_IsGrabbable = value; } }

		[SerializeField]
		private FingerRequirement m_FingerRequirement;
		public FingerRequirement fingerRequirement => m_FingerRequirement;

		[SerializeField]
		HandGrabbableEvent m_OnBeginGrabbed = new HandGrabbableEvent();
		public HandGrabbableEvent onBeginGrabbed => m_OnBeginGrabbed;

		[SerializeField]
		HandGrabbableEvent m_OnEndGrabbed = new HandGrabbableEvent();
		public HandGrabbableEvent onEndGrabbed => m_OnEndGrabbed;
		#endregion

		#region Public State

		[SerializeField]
		private Rigidbody m_Rigidbody = null;
		public new Rigidbody rigidbody => m_Rigidbody;

		[SerializeField]
		private List<GrabPose> m_GrabPoses = new List<GrabPose>();
		public List<GrabPose> grabPoses => m_GrabPoses;

		private GrabPose m_BestGrabPose = GrabPose.Identity;
		public GrabPose bestGrabPose => m_BestGrabPose;

		#endregion

		[SerializeField]
		private bool m_ShowAllIndicator = false;
		private List<Collider> allColliders = new List<Collider>();
		private HandGrabInteractor closestGrabber = null;
		private OnBeginGrabbed beginGrabbed;
		private OnEndGrabbed endGrabbed;

		[SerializeField]
		private IOneHandContraintMovement m_OneHandContraintMovement;
		public bool isContraint => m_OneHandContraintMovement != null;

		[SerializeField]
		private int m_PreviewIndex = -1;

		#region MonoBehaviour
		private void Awake()
		{
			allColliders.AddRange(transform.GetComponentsInChildren<Collider>(true));
		}

		private void OnEnable()
		{
			GrabManager.RegisterGrabbable(this);
			Initialize();
		}

		private void OnDisable()
		{
			GrabManager.UnregisterGrabbable(this);
		}
		#endregion

		#region Public Interface
		/// <summary>
		/// Set the grabber for the hand grabbable object.
		/// </summary>
		/// <param name="grabber">The grabber to set.</param>
		public void SetGrabber(IGrabber grabber)
		{
			if (grabber is HandGrabInteractor handGrabber)
			{
				m_Grabber = handGrabber;
				HandPose handPose = HandPoseProvider.GetHandPose(handGrabber.isLeft ? HandPoseType.HAND_LEFT : HandPoseType.HAND_RIGHT);
				handPose.GetPosition(JointType.Wrist, out Vector3 wristPos);
				handPose.GetRotation(JointType.Wrist, out Quaternion wristRot);
				UpdateBestGrabPose(handGrabber.isLeft, new Pose(wristPos, wristRot));
				beginGrabbed?.Invoke(this);
				m_OnBeginGrabbed?.Invoke(this);
				DEBUG($"{transform.name} is grabbed by {handGrabber.name}");
			}
			else
			{
				m_Grabber = null;
				m_BestGrabPose = GrabPose.Identity;
				endGrabbed?.Invoke(this);
				m_OnEndGrabbed?.Invoke(this);
				DEBUG($"{transform.name} is released.");
			}
		}

		/// <summary>
		/// Enable/Disable indicators. If enabled, display the closest indicator based on grabber position.
		/// </summary>
		/// <param name="enable">True to show the indicator, false to hide it.</param>
		/// <param name="grabber">The grabber for which to show or hide this indicator.</param>
		public void ShowIndicator(bool enable, HandGrabInteractor grabber)
		{
			if (enable)
			{
				closestGrabber = grabber;
				if (m_ShowAllIndicator)
				{
					ShowAllIndicator(grabber.isLeft);
				}
				else
				{
					HandPose handPose = HandPoseProvider.GetHandPose(grabber.isLeft ? HandPoseType.HAND_LEFT : HandPoseType.HAND_RIGHT);
					handPose.GetPosition(JointType.Wrist, out Vector3 wristPos);
					handPose.GetRotation(JointType.Wrist, out Quaternion wristRot);
					int index = FindBestGrabPose(grabber.isLeft, new Pose(wristPos, wristRot));
					ShowIndicatorByIndex(index);
				}
			}
			else
			{
				if (closestGrabber == grabber)
				{
					closestGrabber = null;
					ShowIndicatorByIndex(-1);
				}
			}
		}

		/// <summary>
		/// Calculate the shortest distance between the grabber and the grabbable and convert it into a score based on grabDistance.
		/// </summary>
		/// <param name="grabberPos">The current pose of grabber.</param>
		/// <param name="grabDistance">The maximum grab distance between the grabber and the grabbable object.</param>
		/// <returns>The score represents the distance between the grabber and the grabbable.</returns>
		public float CalculateDistanceScore(Vector3 grabberPos, float grabDistance = 0.03f)
		{
			if (!isGrabbable || isGrabbed) { return 0; }
			Vector3 closestPoint = GetClosestPoint(grabberPos);
			float distacne = Vector3.Distance(grabberPos, closestPoint);
			return distacne > grabDistance ? 0 : 1 - (distacne / grabDistance);
		}

		/// <summary>
		/// Add a listener for the event triggered when the grabbable object is grabbed.
		/// </summary>
		/// <param name="handler">The method to be called when the grabbable object is grabbed.</param>
		[Obsolete("Please use onBeginGrabbed instead.")]
		public void AddBeginGrabbedListener(OnBeginGrabbed handler)
		{
			beginGrabbed += handler;
		}

		/// <summary>
		/// Remove a listener for the event triggered when the grabbable object is grabbed.
		/// </summary>
		/// <param name="handler">The method to be removed from the event listeners.</param>
		[Obsolete("Please use onBeginGrabbed instead.")]
		public void RemoveBeginGrabbedListener(OnBeginGrabbed handler)
		{
			beginGrabbed -= handler;
		}

		/// <summary>
		/// Add a listener for the event triggered when the grabbable object is released.
		/// </summary>
		/// <param name="handler">The method to be called when the grabbable object is released.</param>
		[Obsolete("Please use onEndGrabbed instead.")]
		public void AddEndGrabbedListener(OnEndGrabbed handler)
		{
			endGrabbed += handler;
		}

		/// <summary>
		/// Remove a listener for the event triggered when the grabbable object is released.
		/// </summary>
		/// <param name="handler">The method to be removed from the event listeners.</param>
		[Obsolete("Please use onEndGrabbed instead.")]
		public void RemoveEndGrabbedListener(OnEndGrabbed handler)
		{
			endGrabbed -= handler;
		}

		/// <summary>
		/// Update the position and rotation of the self with the pose of the hand that is grabbing it.
		/// </summary>
		/// <param name="grabberPose">The pose of the hand.</param>
		public void UpdatePositionAndRotation(Pose grabberPose)
		{
			if (m_OneHandContraintMovement == null)
			{
				Quaternion handRotDiff = grabberPose.rotation * Quaternion.Inverse(m_BestGrabPose.grabOffset.sourceRotation);
				transform.position = grabberPose.position + handRotDiff * m_BestGrabPose.grabOffset.posOffset;
				transform.rotation = grabberPose.rotation * m_BestGrabPose.grabOffset.rotOffset;
			}

			if (m_OneHandContraintMovement != null)
			{
				m_OneHandContraintMovement.UpdatePose(grabberPose);
				UpdateMeshPoseByGrabbable();
			}
		}
		#endregion

		/// <summary>
		/// Generate all indicators and calculate grab offsets.
		/// </summary>
		private void Initialize()
		{
			if (m_OneHandContraintMovement != null)
			{
				m_OneHandContraintMovement.Initialize(this);
				onBeginGrabbed.AddListener(m_OneHandContraintMovement.OnBeginGrabbed);
				onEndGrabbed.AddListener(m_OneHandContraintMovement.OnEndGrabbed);
			}

			for (int i = 0; i < m_GrabPoses.Count; i++)
			{
				if (m_GrabPoses[i].indicator.enableIndicator || m_ShowAllIndicator)
				{
					if (m_GrabPoses[i].indicator.NeedGenerateIndicator())
					{
						AutoGenerateIndicator(i);
					}
					else
					{
						GrabPose grabPose = m_GrabPoses[i];
						grabPose.indicator.CalculateGrabOffset(transform.position, transform.rotation);
						m_GrabPoses[i] = grabPose;
					}
				}
			}
			ShowIndicatorByIndex(-1);
		}

		/// <summary>
		/// Automatically generate an indicator by the index of the grab pose.
		/// </summary>
		/// <param name="index">The index of the grab pose.</param>
		private void AutoGenerateIndicator(int index)
		{
			AutoGenIndicator autoGenIndicator = new GameObject($"Indicator {index}", typeof(AutoGenIndicator)).GetComponent<AutoGenIndicator>();
			GrabPose grabPose = m_GrabPoses[index];
			Pose handPose = CalculateActualHandPose(grabPose.grabOffset);
			Vector3 closestPoint = GetClosestPoint(handPose.position);
			autoGenIndicator.SetPose(closestPoint, closestPoint - transform.position);
			grabPose.indicator.Update(true, true, autoGenIndicator.gameObject);
			grabPose.indicator.CalculateGrabOffset(transform.position, transform.rotation);
			m_GrabPoses[index] = grabPose;
		}

		/// <summary>
		/// Calculate the point closest to the source position.
		/// </summary>
		/// <param name="sourcePos">The position of source.</param>
		/// <returns>The position which closest to the source position.</returns>
		private Vector3 GetClosestPoint(Vector3 sourcePos)
		{
			Vector3 closestPoint = Vector3.zero;
			float shortDistance = float.MaxValue;
			foreach (var collider in allColliders)
			{
				Vector3 closePoint = collider.ClosestPointOnBounds(sourcePos);
				float distance = Vector3.Distance(sourcePos, closePoint);
				if (collider.bounds.Contains(closePoint))
				{
					Vector3 direction = (closePoint - sourcePos).normalized;
					RaycastHit[] hits = Physics.RaycastAll(sourcePos, direction, distance);
					foreach (var hit in hits)
					{
						if (hit.collider == collider)
						{
							float hitDistance = Vector3.Distance(sourcePos, hit.point);
							if (distance > hitDistance)
							{
								distance = hitDistance;
								closePoint = hit.point;
							}
						}
					}
				}

				if (shortDistance > distance)
				{
					shortDistance = distance;
					closestPoint = closePoint;
				}
			}
			return closestPoint;
		}

		/// <summary>
		/// Find the best grab pose for the grabber and updates the bestGrabPoseId.
		/// </summary>
		/// <param name="isLeft">Whether the grabber is the left hand.</param>
		/// <param name="grabberPose">The pose of the grabber.</param>
		/// <returns>True if a best grab pose is found; otherwise, false.</returns>
		private void UpdateBestGrabPose(bool isLeft, Pose grabberPose)
		{
			int index = FindBestGrabPose(isLeft, grabberPose);
			if (index != -1 && index < m_GrabPoses.Count)
			{
				m_BestGrabPose = m_GrabPoses[index];
			}
			else
			{
				m_BestGrabPose.grabOffset = new GrabOffset(grabberPose.position, grabberPose.rotation, transform.position, transform.rotation);
				HandPose handPose = HandPoseProvider.GetHandPose(isLeft ? HandPoseType.MESH_LEFT : HandPoseType.MESH_RIGHT);
				if (handPose is MeshHandPose meshHandPose)
				{
					Quaternion[] grabRotations = new Quaternion[(int)JointType.Count];
					for (int i = 0; i < (int)JointType.Count; i++)
					{
						meshHandPose.GetRotation((JointType)i, out grabRotations[i], local: true);
					}
					m_BestGrabPose.recordedGrabRotations = grabRotations;
				}
			}
		}

		/// <summary>
		/// Find the best grab pose for the grabber.
		/// </summary>
		/// <param name="isLeft">Whether the grabber is the left hand.</param>
		/// <param name="grabberPose">The pose of the grabber.</param>
		/// <returns>The index of the best grab pose among the grab poses.</returns>
		private int FindBestGrabPose(bool isLeft, Pose grabberPose)
		{
			int index = -1;
			float maxDot = float.MinValue;
			Vector3 currentDirection = grabberPose.position - transform.position;
			for (int i = 0; i < m_GrabPoses.Count; i++)
			{
				if (m_GrabPoses[i].isLeft == isLeft)
				{
					Pose handPose = CalculateActualHandPose(m_GrabPoses[i].grabOffset);
					Vector3 grabDirection = handPose.position - transform.position;
					float dot = Vector3.Dot(currentDirection.normalized, grabDirection.normalized);
					if (dot > maxDot)
					{
						maxDot = dot;
						index = i;
					}
				}
			}
			return index;
		}

		/// <summary>
		/// Show the indicator corresponding to the specified index and hides others.
		/// </summary>
		/// <param name="index">The index of the indicator to show.</param>
		private void ShowIndicatorByIndex(int index)
		{
			foreach (var grabPose in m_GrabPoses)
			{
				grabPose.indicator.SetActive(false);
			}
			if (index >= 0 && index < m_GrabPoses.Count &&
				m_GrabPoses[index].indicator.enableIndicator)
			{
				m_GrabPoses[index].indicator.UpdatePositionAndRotation(transform.position, transform.rotation);
				m_GrabPoses[index].indicator.SetActive(true);
			}
		}

		/// <summary>
		/// Show all indicators corresponding to the specified hand side and hides others.
		/// </summary>
		/// <param name="isLeft">Whether the hand side is left.</param>
		private void ShowAllIndicator(bool isLeft)
		{
			foreach (var grabPose in m_GrabPoses)
			{
				grabPose.indicator.SetActive(false);
			}
			foreach (var grabPose in m_GrabPoses)
			{
				if (grabPose.isLeft == isLeft)
				{
					grabPose.indicator.UpdatePositionAndRotation(transform.position, transform.rotation);
					grabPose.indicator.SetActive(true);
				}
			}
		}

		private void UpdateMeshPoseByGrabbable()
		{
			if (grabber is HandGrabInteractor handGrabber)
			{
				HandPose handPose = HandPoseProvider.GetHandPose(handGrabber.isLeft ? HandPoseType.MESH_LEFT : HandPoseType.MESH_RIGHT);
				if (handPose != null && handPose is MeshHandPose meshHandPose)
				{
					Pose realHandPose = CalculateActualHandPose(m_BestGrabPose.grabOffset);
					meshHandPose.SetJointPose(JointType.Wrist, realHandPose);
				}
			}
		}

		private Pose CalculateActualHandPose(GrabOffset grabOffset)
		{
			Quaternion handRot = transform.rotation * Quaternion.Inverse(grabOffset.rotOffset);
			Quaternion handRotDiff = handRot * Quaternion.Inverse(grabOffset.sourceRotation);
			Vector3 handPos = transform.position - handRotDiff * grabOffset.posOffset;
			return new Pose(handPos, handRot);
		}
	}
}
