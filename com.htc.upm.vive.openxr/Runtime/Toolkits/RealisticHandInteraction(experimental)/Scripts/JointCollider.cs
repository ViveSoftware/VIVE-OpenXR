// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	/// <summary>
	/// This class is designed to generate appropriately sized colliders for each joint.
	/// </summary>
	public class JointCollider : MonoBehaviour
	{
		public enum CollisionState
		{
			Enter = 0,
			Stay = 1,
			Exit = 2,
		}

		private CapsuleCollider m_Collider = null;
		public Collider Collider => m_Collider;

		public delegate void OnJointCollision(Collision collision, CollisionState state);
		private OnJointCollision onJointCollision;

		private const float k_ColliderRadius = 1E-6f;
		private const float k_ColliderHeight = 1E-6f;
		private JointType jointType = JointType.Count;

		private void OnEnable()
		{
			InitCollider();
		}

		/// <summary>
		/// Set the joint id and adjust collider size..
		/// </summary>
		/// <param name="id">JointType of joint.</param>
		public void SetJointId(int id)
		{
			InitCollider();

			jointType = (JointType)id;
			switch (jointType)
			{
				case JointType.Palm:
					m_Collider.center = new Vector3(0f, -0.006f, -0.0025f);
					m_Collider.radius = 0.016f;
					m_Collider.height = 0.075f;
					m_Collider.direction = 0;
					break;
				case JointType.Wrist:
					m_Collider.center = new Vector3(0f, -0.003f, 0f);
					m_Collider.radius = 0.02f;
					m_Collider.height = 0.055f;
					m_Collider.direction = 0;
					break;
				case JointType.Thumb_Joint0:
					m_Collider.center = new Vector3(0f, -0.005f, 0f);
					m_Collider.radius = 0.02f;
					m_Collider.height = 0.05f;
					break;
				case JointType.Thumb_Joint1:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.012f;
					m_Collider.height = 0.040f;
					break;
				case JointType.Thumb_Joint2:
					m_Collider.center = new Vector3(0f, -0.003f, 0f);
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Thumb_Tip:
					m_Collider.center = new Vector3(0f, 0f, -0.005f);
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.025f;
					break;
				case JointType.Index_Joint0:
					m_Collider.center = new Vector3(0f, 0f, 0.02f);
					m_Collider.radius = 0.012f;
					m_Collider.height = 0.1f;
					break;
				case JointType.Index_Joint1:
					m_Collider.center = new Vector3(0f, 0f, 0.01f);
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.04f;
					break;
				case JointType.Index_Joint2:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Index_Joint3:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Index_Tip:
					m_Collider.center = new Vector3(0f, -0.001f, -0.0025f);
					m_Collider.radius = 0.008f;
					m_Collider.height = 0.015f;
					break;
				case JointType.Middle_Joint0:
					m_Collider.center = new Vector3(0f, 0f, 0.02f);
					m_Collider.radius = 0.012f;
					m_Collider.height = 0.1f;
					break;
				case JointType.Middle_Joint1:
					m_Collider.center = new Vector3(0f, 0f, 0.01f);
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.04f;
					break;
				case JointType.Middle_Joint2:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Middle_Joint3:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Middle_Tip:
					m_Collider.center = new Vector3(0f, -0.002f, -0.0025f);
					m_Collider.radius = 0.008f;
					m_Collider.height = 0.015f;
					break;
				case JointType.Ring_Joint0:
					m_Collider.center = new Vector3(0f, 0f, 0.02f);
					m_Collider.radius = 0.012f;
					m_Collider.height = 0.1f;
					break;
				case JointType.Ring_Joint1:
					m_Collider.center = new Vector3(0f, 0f, 0.01f);
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.04f;
					break;
				case JointType.Ring_Joint2:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Ring_Joint3:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Ring_Tip:
					m_Collider.center = new Vector3(0f, -0.002f, -0.0025f);
					m_Collider.radius = 0.008f;
					m_Collider.height = 0.015f;
					break;
				case JointType.Pinky_Joint0:
					m_Collider.center = new Vector3(0f, 0f, 0.02f);
					m_Collider.radius = 0.012f;
					m_Collider.height = 0.1f;
					break;
				case JointType.Pinky_Joint1:
					m_Collider.center = new Vector3(0f, 0f, 0.01f);
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.04f;
					break;
				case JointType.Pinky_Joint2:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Pinky_Joint3:
					m_Collider.center = Vector3.zero;
					m_Collider.radius = 0.01f;
					m_Collider.height = 0.01f;
					break;
				case JointType.Pinky_Tip:
					m_Collider.center = new Vector3(0f, -0.002f, -0.0025f);
					m_Collider.radius = 0.006f;
					m_Collider.height = 0.015f;
					break;
			}
		}

		public Rigidbody InitRigidbody(float mass)
		{
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = mass;
			rigidbody.useGravity = false;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			rigidbody.solverIterations = 100;
			rigidbody.solverVelocityIterations = 15;
			return rigidbody;
		}

		private void InitCollider()
		{
			if (m_Collider == null)
			{
				m_Collider = gameObject.AddComponent<CapsuleCollider>();
				m_Collider.radius = k_ColliderRadius;
				m_Collider.height = k_ColliderHeight;
				m_Collider.direction = 2;
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!IsJointCollider(collision.collider))
			{
				onJointCollision?.Invoke(collision, CollisionState.Enter);
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if (!IsJointCollider(collision.collider))
			{
				onJointCollision?.Invoke(collision, CollisionState.Stay);
			}
		}

		private void OnCollisionExit(Collision collision)
		{
			if (!IsJointCollider(collision.collider))
			{
				onJointCollision?.Invoke(collision, CollisionState.Exit);
			}
		}

		private bool IsJointCollider(Collider collider)
		{
			JointCollider jointCollider = collider.GetComponent<JointCollider>();
			return jointCollider != null;
		}

		public void AddJointCollisionListener(OnJointCollision handler)
		{
			onJointCollision += handler;
		}

		public void RemoveJointCollisionListener(OnJointCollision handler)
		{
			onJointCollision -= handler;
		}
	}
}