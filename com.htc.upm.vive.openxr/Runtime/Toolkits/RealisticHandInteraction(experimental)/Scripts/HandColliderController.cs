// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	public class HandColliderController : MonoBehaviour
	{
		#region Log

		const string LOG_TAG = "VIVE.OpenXR.Toolkits.RealisticHandInteraction.HandColliderController";
		private void DEBUG(string msg) { Debug.Log($"{LOG_TAG}, {msg}"); }
		private void WARNING(string msg) { Debug.LogWarning($"{LOG_TAG}, {msg}"); }
		private void ERROR(string msg) { Debug.LogError($"{LOG_TAG}, {msg}"); }
		int logFrame = 0;
		bool printIntervalLog => logFrame == 0;

		#endregion

		private HandMeshManager m_HandMesh;
		public HandMeshManager handMesh { get { return m_HandMesh; } set { m_HandMesh = value; } }

		private bool m_EnableCollider = true;
		public bool enableCollider
		{
			get { return m_EnableCollider; }
			set
			{
				m_EnableCollider = value;
				rootJoint.gameObject.SetActive(m_EnableCollider);
			}
		}

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

		private float handMass = 1.0f;
		private JointCollider[] jointsCollider = new JointCollider[(int)JointType.Count];
		private Transform rootJoint = null;
		private Transform rootJointParent = null;
		private Rigidbody rootJointRigidbody = null;
		private Vector3 lastRootPos;
		private Quaternion lastRotation;
		private bool isInit = false;
		private bool isTracked = true;
		private List<Vector3> collisionDirections = new List<Vector3>();
		private bool isGrabbing = false;

		#region MonoBehaviour

		private void OnEnable()
		{
			StartCoroutine(WaitForInit());
		}

		private void OnDisable()
		{
			if (rootJoint != null)
			{
				JointCollider jointCollider = rootJoint.GetComponent<JointCollider>();
				if (jointCollider != null)
				{
					jointCollider.RemoveJointCollisionListener(OnJointCollision);
				}
				Destroy(rootJoint.gameObject);
			}
			isInit = false;
		}

		private void Start()
		{
			JointCollider[] fullJointsCollider = FindObjectsOfType<JointCollider>(true);
			for (int i = 0; i < fullJointsCollider.Length - 1; i++)
			{
				for (int j = i + 1; j < fullJointsCollider.Length; j++)
				{
					Physics.IgnoreCollision(fullJointsCollider[i].Collider, fullJointsCollider[j].Collider);
				}
			}
		}

		private void Update()
		{
			if (!isInit) { return; }

			HandPose handPose = HandPoseProvider.GetHandPose(m_HandMesh.isLeft ? HandPoseType.HAND_LEFT : HandPoseType.HAND_RIGHT);
			if (isTracked != handPose.IsTracked())
			{
				isTracked = handPose.IsTracked();
				ChangeTrackingState(isTracked);
			}
			if (!isTracked) { return; }

			for (int i = 0; i < jointsCollider.Length; i++)
			{
				if (jointsCollider[i] == null) { continue; }

				handPose.GetPosition((JointType)i, out Vector3 jointPosition);
				handPose.GetRotation((JointType)i, out Quaternion jointRotation);
				if (i == (int)JointType.Wrist)
				{
					if (lastRootPos == Vector3.zero || lastRotation == Quaternion.identity)
					{
						rootJoint.localPosition = jointPosition;
						rootJoint.localRotation = jointRotation;
					}
					lastRootPos = jointPosition;
					lastRotation = jointRotation;
				}
				jointsCollider[i].transform.rotation = rootJointParent.rotation * jointRotation;
			}
			if (!isGrabbing)
			{
				m_HandMesh.SetJointPositionAndRotation(JointType.Wrist, rootJoint.position, rootJoint.rotation);
			}
		}

		private void FixedUpdate()
		{
			if (lastRootPos == Vector3.zero || lastRotation == Quaternion.identity) { return; }

			if (isGrabbing)
			{
				rootJointRigidbody.velocity = Vector3.zero;
				rootJointRigidbody.angularVelocity = Vector3.zero;
				rootJoint.localPosition = lastRootPos;
				rootJoint.localRotation = lastRotation;
			}
			else
			{
				UpdateVelocity();
				UpdateAngularVelocity();
			}
		}

		#endregion

		private IEnumerator WaitForInit()
		{
			yield return new WaitUntil(() => m_HandMesh != null);

			if (rootJoint != null)
			{
				JointCollider jointCollider = rootJoint.GetComponent<JointCollider>();
				jointCollider.AddJointCollisionListener(OnJointCollision);
				rootJointRigidbody = jointCollider.InitRigidbody(handMass);
				isInit = true;

				DEBUG("Hand Collider init successfully.");
			}
		}

		/// <summary>
		/// Initializes by copying the structure of the hand mesh and sequentially adding joint collider.
		/// </summary>
		public void InitJointColliders(Transform handRootJoint)
		{
			rootJointParent = handRootJoint.parent;
			rootJoint = Instantiate(handRootJoint, rootJointParent);
			rootJoint.name = handRootJoint.name;
			List<Transform> totalTransforms = new List<Transform>() { rootJoint };
			GetAllChildrenTransforms(rootJoint, ref totalTransforms);

			for (int i = 0; i < (int)JointType.Count; i++)
			{
				Transform target = totalTransforms.FirstOrDefault(x => x.name == JointsName[i]);
				if (target != null)
				{
					JointCollider jointCollider = target.gameObject.AddComponent<JointCollider>();
					jointCollider.SetJointId(i);
					jointsCollider[i] = jointCollider;
				}
			}
		}

		private void GetAllChildrenTransforms(Transform parent, ref List<Transform> childrenTransforms)
		{
			foreach (Transform child in parent)
			{
				childrenTransforms.Add(child);
				GetAllChildrenTransforms(child, ref childrenTransforms);
			}
		}

		/// <summary>
		/// Updates the velocity of a Rigidbody.
		/// </summary>
		private void UpdateVelocity()
		{
			Vector3 vel = (lastRootPos - rootJoint.position) / Time.deltaTime;
			if (IsValidVelocity(vel))
			{
				if (collisionDirections.Count > 0)
				{
					float minAngle = float.MaxValue;
					Vector3 closestDirection = Vector3.zero;
					foreach (Vector3 direction in collisionDirections.ToList())
					{
						float angle = Mathf.Abs(Vector3.Angle(direction, vel));
						if (angle < minAngle)
						{
							minAngle = angle;
							closestDirection = direction;
						}
					}
					collisionDirections.Clear();

					Vector3 adjustedDirection = closestDirection;
					if (Vector3.Dot(vel, closestDirection) > 0)
					{
						adjustedDirection *= -1f;
					}
					vel = Vector3.ProjectOnPlane(vel, adjustedDirection);
					if (vel.magnitude > 1)
					{
						vel.Normalize();
					}
				}
				rootJointRigidbody.velocity = vel;
			}
		}

		/// <summary>
		/// Updates the angularVelocity of a Rigidbody.
		/// </summary>
		private void UpdateAngularVelocity()
		{
			Quaternion diffRotation = Quaternion.Inverse(rootJoint.rotation) * lastRotation;
			diffRotation.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);
			Vector3 angVel = (angleInDegree * rotationAxis * Mathf.Deg2Rad) / Time.deltaTime;
			if (IsValidVelocity(angVel))
			{
				rootJointRigidbody.angularVelocity = angVel;
			}
		}

		/// <summary>
		/// Checks if the vector is valid.
		/// </summary>
		/// <param name="vector">The vector to be checked.</param>
		/// <returns>rue if the vector is valid; otherwise, false.</returns>
		private bool IsValidVelocity(Vector3 vector)
		{
			return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z)
				&& !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
		}

		#region Event CallBack

		/// <summary>
		/// When tracking state changing, reset the pose and enable/disable collider.
		/// </summary>
		/// <param name="isTracked">The bool that tracking state.</param>
		private void ChangeTrackingState(bool isTracked)
		{
			if (!isTracked)
			{
				lastRootPos = Vector3.zero;
				lastRotation = Quaternion.identity;
				rootJointRigidbody.velocity = Vector3.zero;
				rootJointRigidbody.angularVelocity = Vector3.zero;
			}
			foreach (JointCollider jointCollider in jointsCollider)
			{
				jointCollider.Collider.enabled = m_EnableCollider && isTracked;
			}
		}

		/// <summary>
		/// Called when the hand begins grabbing an object.
		/// </summary>
		/// <param name="grabber">The grabber that grabbing something.</param>
		public void OnHandBeginGrab(IGrabber grabber)
		{
			EnableKinematic(true);
			isGrabbing = true;
		}

		/// <summary>
		/// Called when the hand ends grabbing an object.
		/// </summary>
		/// <param name="grabber">The grabber that releasing something.</param>
		public void OnHandEndGrab(IGrabber grabber)
		{
			EnableKinematic(false);
			isGrabbing = false;
		}

		/// <summary>
		/// Enable/Disable the rigidbody of hand collider.
		/// </summary>
		/// <param name="enable">The bool that enable or disable the rigidbody.</param>
		private void EnableKinematic(bool enable)
		{
			if (rootJointRigidbody != null)
			{
				rootJointRigidbody.isKinematic = enable;
			}
		}

		/// <summary>
		///  Called when a joint collider collides with another object.
		/// </summary>
		/// <param name="collision">The collision data.</param>
		/// <param name="state">The state of the collision.</param>
		private void OnJointCollision(Collision collision, JointCollider.CollisionState state)
		{
			if (isGrabbing) { return; }

			switch (state)
			{
				case JointCollider.CollisionState.Enter:
				case JointCollider.CollisionState.Stay:
					if (collision.contactCount > 0 && (collision.rigidbody == null || collision.rigidbody.isKinematic))
					{
						ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
						collision.GetContacts(contactPoints);
						foreach (ContactPoint contactPoint in contactPoints)
						{
							collisionDirections.Add(contactPoint.normal * -1f);
						}
					}
					break;
			}
		}

		#endregion
	}
}
