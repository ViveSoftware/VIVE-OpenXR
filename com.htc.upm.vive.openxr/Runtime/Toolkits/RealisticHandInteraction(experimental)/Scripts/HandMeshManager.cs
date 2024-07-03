using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	/// <summary>
	/// This class is designed to manage the positions of various joint nodes in the hand model.
	/// </summary>
	public class HandMeshManager : MonoBehaviour
	{
		#region Log

		const string LOG_TAG = "VIVE.OpenXR.Toolkits.RealisticHandInteraction.HandMeshManager";
		private void DEBUG(string msg) { Debug.Log($"{LOG_TAG}, {msg}"); }
		private void WARNING(string msg) { Debug.LogWarning($"{LOG_TAG}, {msg}"); }
		private void ERROR(string msg) { Debug.LogError($"{LOG_TAG}, {msg}"); }
		int logFrame = 0;
		bool printIntervalLog => logFrame == 0;

		#endregion

		[SerializeField]
		private Handedness m_Handedness;
		public bool isLeft { get { return m_Handedness == Handedness.Left; } }

		[SerializeField]
		private bool m_EnableCollider = false;
		public bool enableCollider
		{
			get { return m_EnableCollider; }
			set
			{
				m_EnableCollider = value;
				if (m_EnableCollider && m_HandCollider == null)
				{
					InitHandCollider();
				}
				else if (m_HandCollider != null)
				{
					m_HandCollider.enableCollider = m_EnableCollider;
				}
			}
		}

		private HandColliderController m_HandCollider = null;
		public HandColliderController handCollider => m_HandCollider;

		[SerializeField]
		private Transform[] m_HandJoints = new Transform[k_JointCount];

		private const int k_JointCount = (int)JointType.Count;
		private const int k_RootId = (int)JointType.Wrist;
		private bool updateRoot = false;
		private int updatedFrameCount = 0;
		private bool isGrabbing = false;
		private bool isConstraint = false;
		private HandGrabInteractor handGrabber;
		private Quaternion[] grabJointsRotation = new Quaternion[k_JointCount];

		#region MonoBehaviours

		private void OnEnable()
		{
			bool empty = m_HandJoints.Any(x => x == null);
			if (empty)
			{
				ClearJoints();
				FindJoints();
			}

			if (m_EnableCollider && m_HandCollider == null)
			{
				InitHandCollider();
			}

			MeshHandPose meshHandPose = transform.gameObject.AddComponent<MeshHandPose>();
			meshHandPose.SetHandMeshRenderer(this);
		}

		private void OnDisable()
		{
			if (m_HandCollider != null)
			{
				Destroy(m_HandCollider);
			}

			MeshHandPose meshHandPose = transform.GetComponent<MeshHandPose>();
			if (meshHandPose != null)
			{
				Destroy(meshHandPose);
			}
		}

		private void Update()
		{
			HandData handData = CachedHand.Get(isLeft);
			EnableHandModel(handData.isTracked);
			if (!handData.isTracked) { return; }

			//if (m_UseRuntimeModel || (!m_UseRuntimeModel && m_UseScale))
			//{
			//	Vector3 scale = Vector3.one;
			//	if (GetHandScale(ref scale, isLeft))
			//	{
			//		m_HandJoints[rootId].localScale = scale;
			//	}
			//	else
			//	{
			//		m_HandJoints[rootId].localScale = Vector3.one;
			//	}
			//}

			if (Time.frameCount - updatedFrameCount > 5)
			{
				updateRoot = false;
			}
			if (!updateRoot)
			{
				Vector3 rootPosition = Vector3.zero;
				Quaternion rootRotation = Quaternion.identity;
				handData.GetJointPosition((JointType)k_RootId, ref rootPosition);
				handData.GetJointRotation((JointType)k_RootId, ref rootRotation);

				m_HandJoints[k_RootId].position = m_HandJoints[k_RootId].parent.position + rootPosition;
				m_HandJoints[k_RootId].rotation = m_HandJoints[k_RootId].parent.rotation * rootRotation;
			}

			for (int i = 0; i < m_HandJoints.Length; i++)
			{
				if (m_HandJoints[i] == null || i == k_RootId) { continue; }

				Quaternion jointRotation = Quaternion.identity;
				handData.GetJointRotation((JointType)i, ref jointRotation);
				m_HandJoints[i].rotation = m_HandJoints[k_RootId].parent.rotation * jointRotation;
			}

			if (isGrabbing)
			{
				for (int i = 0; i < m_HandJoints.Length; i++)
				{
					if (i == k_RootId) { continue; }

					Quaternion currentRotation = m_HandJoints[i].rotation;
					Quaternion maxRotation = m_HandJoints[i].parent.rotation * grabJointsRotation[i];
					if (isConstraint ||
						handGrabber.IsRequiredJoint((JointType)i) ||
						OverFlex(currentRotation, maxRotation) >= 0 ||
						FlexAngle(currentRotation, maxRotation) >= 110)
					{
						m_HandJoints[i].rotation = maxRotation;
					}
				}
			}
		}

		#endregion

		#region Public Interface

		public void OnHandBeginGrab(IGrabber grabber)
		{
			if (grabber is HandGrabInteractor handGrabber)
			{
				this.handGrabber = handGrabber;

				if (grabber.grabbable is HandGrabInteractable handGrabbable)
				{
					if (handGrabbable.bestGrabPose != GrabPose.Identity)
					{
						if (handGrabbable.bestGrabPose.recordedGrabRotations.Length == (int)JointType.Count)
						{
							grabJointsRotation = handGrabbable.bestGrabPose.recordedGrabRotations;
						}
						else if (handGrabbable.bestGrabPose.handGrabGesture != HandGrabGesture.Identity)
						{
							for (int i = 0; i < grabJointsRotation.Length; i++)
							{
								HandData.GetDefaultJointRotationInGesture(isLeft, handGrabbable.bestGrabPose.handGrabGesture, (JointType)i, ref grabJointsRotation[i]);
							}
						}
						isGrabbing = true;
						isConstraint = handGrabbable.isContraint;
					}
				}
			}

			if (m_EnableCollider && m_HandCollider != null)
			{
				m_HandCollider.OnHandBeginGrab(grabber);
			}
		}

		public void OnHandEndGrab(IGrabber grabber)
		{
			isGrabbing = false;
			this.handGrabber = null;

			if (m_EnableCollider && handCollider != null)
			{
				handCollider.OnHandEndGrab(grabber);
			}
		}

		/// <summary>
		/// Gets the position and rotation of the specified joint.
		/// </summary>
		/// <param name="joint">The joint type to get position and rotation from.</param>
		/// <param name="position">The position of the joint.</param>
		/// <param name="rotation">The rotation of the joint.</param>
		/// <param name="local">Whether to get the local position and rotation.</param>
		/// <returns>True if the joint position and rotation are successfully obtained; otherwise, false.</returns>
		public bool GetJointPositionAndRotation(JointType joint, out Vector3 position, out Quaternion rotation, bool local = false)
		{
			position = Vector3.zero;
			rotation = Quaternion.identity;
			int jointId = (int)joint;
			if (jointId >= 0 && jointId < k_JointCount && m_HandJoints[jointId] != null)
			{
				if (!local)
				{
					position = m_HandJoints[jointId].position;
					rotation = m_HandJoints[jointId].rotation;
				}
				else
				{
					position = m_HandJoints[jointId].localPosition;
					rotation = m_HandJoints[jointId].localRotation;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Sets the position and rotation of the specified joint.
		/// </summary>
		/// <param name="joint">The joint type to set position and rotation for.</param>
		/// <param name="position">The new position of the joint.</param>
		/// <param name="rotation">The new rotation of the joint.</param>
		/// <param name="local">Whether to set the local position and rotation.</param>
		/// <returns>True if the joint position and rotation are successfully set; otherwise, false.</returns>
		public bool SetJointPositionAndRotation(JointType joint, Vector3 position, Quaternion rotation, bool local = false)
		{
			int jointId = (int)joint;
			if (jointId >= 0 && jointId < k_JointCount && m_HandJoints[jointId] != null)
			{
				if (!local)
				{
					m_HandJoints[jointId].position = position;
					m_HandJoints[jointId].rotation = rotation;
				}
				else
				{
					m_HandJoints[jointId].localPosition = position;
					m_HandJoints[jointId].localRotation = rotation;
				}

				if (joint == JointType.Wrist)
				{
					updatedFrameCount = Time.frameCount;
					updateRoot = true;
				}
				return true;
			}
			return false;
		}

		#endregion

		#region Name Definition
		// The order of joint name MUST align with runtime's definition
		private readonly string[] JointsName = new string[]
		{
				"WaveBone_0",  // WVR_HandJoint_Palm = 0
				"WaveBone_1", // WVR_HandJoint_Wrist = 1
				"WaveBone_2", // WVR_HandJoint_Thumb_Joint0 = 2
				"WaveBone_3", // WVR_HandJoint_Thumb_Joint1 = 3
				"WaveBone_4", // WVR_HandJoint_Thumb_Joint2 = 4
				"WaveBone_5", // WVR_HandJoint_Thumb_Tip = 5
				"WaveBone_6", // WVR_HandJoint_Index_Joint0 = 6
				"WaveBone_7", // WVR_HandJoint_Index_Joint1 = 7
				"WaveBone_8", // WVR_HandJoint_Index_Joint2 = 8
				"WaveBone_9", // WVR_HandJoint_Index_Joint3 = 9
				"WaveBone_10", // WVR_HandJoint_Index_Tip = 10
				"WaveBone_11", // WVR_HandJoint_Middle_Joint0 = 11
				"WaveBone_12", // WVR_HandJoint_Middle_Joint1 = 12
				"WaveBone_13", // WVR_HandJoint_Middle_Joint2 = 13
				"WaveBone_14", // WVR_HandJoint_Middle_Joint3 = 14
				"WaveBone_15", // WVR_HandJoint_Middle_Tip = 15
				"WaveBone_16", // WVR_HandJoint_Ring_Joint0 = 16
				"WaveBone_17", // WVR_HandJoint_Ring_Joint1 = 17
				"WaveBone_18", // WVR_HandJoint_Ring_Joint2 = 18
				"WaveBone_19", // WVR_HandJoint_Ring_Joint3 = 19
				"WaveBone_20", // WVR_HandJoint_Ring_Tip = 20
				"WaveBone_21", // WVR_HandJoint_Pinky_Joint0 = 21
				"WaveBone_22", // WVR_HandJoint_Pinky_Joint0 = 22
				"WaveBone_23", // WVR_HandJoint_Pinky_Joint0 = 23
				"WaveBone_24", // WVR_HandJoint_Pinky_Joint0 = 24
				"WaveBone_25" // WVR_HandJoint_Pinky_Tip = 25
		};
		#endregion

		private void GetAllChildrenTransforms(Transform parent, ref List<Transform> childrenTransforms)
		{
			foreach (Transform child in parent)
			{
				childrenTransforms.Add(child);
				GetAllChildrenTransforms(child, ref childrenTransforms);
			}
		}

		public void FindJoints()
		{
			List<Transform> totalTransforms = new List<Transform>() { transform };
			GetAllChildrenTransforms(transform, ref totalTransforms);

			for (int i = 0; i < m_HandJoints.Length; i++)
			{
				Transform jointTransform = totalTransforms.FirstOrDefault(x => x.name == JointsName[i]);
				if (jointTransform != null)
				{
					m_HandJoints[i] = jointTransform;
				}
			}
		}

		public void ClearJoints()
		{
			Array.Clear(m_HandJoints, 0, m_HandJoints.Length);
		}

		private void InitHandCollider()
		{
			m_HandCollider = gameObject.AddComponent<HandColliderController>();
			m_HandCollider.InitJointColliders(m_HandJoints[k_RootId]);
			m_HandCollider.handMesh = this;
		}

		private void EnableHandModel(bool enable)
		{
			if (m_HandJoints[k_RootId].gameObject.activeSelf != enable)
			{
				m_HandJoints[k_RootId].gameObject.SetActive(enable);
			}
		}

		/// <summary>
		/// Calculate whether the current rotation exceeds the maximum rotation.
		/// If the product is greater than 0, it exceeds.
		/// </summary>
		/// <param name="currentRot">Current rotation</param>
		/// <param name="maxRot">Maximum rotation</param>
		/// <returns>The return value represents the dot product between the cross product of two rotations and the -x axis direction of the current rotation.</returns>
		private float OverFlex(Quaternion currentRot, Quaternion maxRot)
		{
			Vector3 currFwd = currentRot * Vector3.forward;
			Vector3 maxFwd = maxRot * Vector3.forward;
			return Vector3.Dot(currentRot * Vector3.left, Vector3.Cross(currFwd, maxFwd));
		}

		/// <summary>
		/// Calculate the angle between the y-axis directions of two rotations.
		/// </summary>
		/// <param name="currentRot">Current rotation</param>
		/// <param name="maxRot">Maximum rotation</param>
		/// <returns>The return value represents the angle between the up directions of the two rotation</returns>
		private float FlexAngle(Quaternion currentRot, Quaternion maxRot)
		{
			Vector3 currFwd = currentRot * Vector3.up;
			Vector3 maxFwd = maxRot * Vector3.up;
			return Mathf.Acos(Vector3.Dot(currFwd, maxFwd) / (currFwd.magnitude * maxFwd.magnitude)) * Mathf.Rad2Deg;
		}
	}
}
