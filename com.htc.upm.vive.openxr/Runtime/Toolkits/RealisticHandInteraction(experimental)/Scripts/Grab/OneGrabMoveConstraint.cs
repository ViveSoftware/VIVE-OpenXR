using System.Text;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction 
{
	public class OneGrabMoveConstraint : IOneHandContraintMovement
	{
		#region Log

		const string LOG_TAG = "VIVE.OpenXR.Toolkits.RealisticHandInteraction.OneGrabMoveConstraint";
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

		[SerializeField]
		private Transform m_Constraint;
		[SerializeField]
		private ConstraintInfo m_NegativeXMove = ConstraintInfo.Identity;
		private float defaultNegativeXPos = 0.0f;
		[SerializeField]
		private ConstraintInfo m_PositiveXMove = ConstraintInfo.Identity;
		private float defaultPositiveXPos = 0.0f;
		[SerializeField]
		private ConstraintInfo m_NegativeYMove = ConstraintInfo.Identity;
		private float defaultNegativeYPos = 0.0f;
		[SerializeField]
		private ConstraintInfo m_PositiveYMove = ConstraintInfo.Identity;
		private float defaultPositiveYPos = 0.0f;
		[SerializeField]
		private ConstraintInfo m_NegativeZMove = ConstraintInfo.Identity;
		private float defaultNegativeZPos = 0.0f;
		[SerializeField]
		private ConstraintInfo m_PositiveZMove = ConstraintInfo.Identity;
		private float defaultPositiveZPos = 0.0f;
		private Pose previousHandPose = Pose.identity;
		private GrabPose currentGrabPose = GrabPose.Identity;

		public override void Initialize(IGrabbable grabbable)
		{
			if (grabbable is HandGrabInteractable handGrabbable)
			{
				if (m_Constraint == null)
				{
					m_Constraint = handGrabbable.transform;
					WARNING("Since no constraint object is set, self will be used as the constraint object.");
				}
			}

			if (m_NegativeXMove.enableConstraint) { defaultNegativeXPos = m_Constraint.position.x - m_NegativeXMove.value; }
			if (m_PositiveXMove.enableConstraint) { defaultPositiveXPos = m_Constraint.position.x + m_PositiveXMove.value; }
			if (m_NegativeYMove.enableConstraint) { defaultNegativeYPos = m_Constraint.position.y - m_NegativeYMove.value; }
			if (m_PositiveYMove.enableConstraint) { defaultPositiveYPos = m_Constraint.position.y + m_PositiveYMove.value; }
			if (m_NegativeZMove.enableConstraint) { defaultNegativeZPos = m_Constraint.position.z - m_NegativeZMove.value; }
			if (m_PositiveZMove.enableConstraint) { defaultPositiveZPos = m_Constraint.position.z + m_PositiveZMove.value; }
		}

		public override void OnBeginGrabbed(IGrabbable grabbable)
		{
			if (grabbable is HandGrabInteractable handGrabbable)
			{
				currentGrabPose = handGrabbable.bestGrabPose;
			}

			if (grabbable.grabber is HandGrabInteractor handGrabber)
			{
				HandPose handPose = HandPoseProvider.GetHandPose(handGrabber.isLeft ? HandPoseType.HAND_LEFT : HandPoseType.HAND_RIGHT);
				handPose.GetPosition(JointType.Wrist, out Vector3 wristPos);
				handPose.GetRotation(JointType.Wrist, out Quaternion wristRot);
				previousHandPose = new Pose(wristPos, wristRot);
			}
		}

		public override void UpdatePose(Pose handPose)
		{
			if (previousHandPose == Pose.identity)
			{
				previousHandPose = handPose;
				return;
			}

			Quaternion previousRotOffset = previousHandPose.rotation * Quaternion.Inverse(currentGrabPose.grabOffset.sourceRotation);
			Vector3 previousPos = previousHandPose.position + previousRotOffset * currentGrabPose.grabOffset.posOffset;

			Quaternion currentRotOffset = handPose.rotation * Quaternion.Inverse(currentGrabPose.grabOffset.sourceRotation);
			Vector3 currentPos = handPose.position + currentRotOffset * currentGrabPose.grabOffset.posOffset;

			Vector3 handOffset = currentPos - previousPos;

			if (m_NegativeXMove.enableConstraint)
			{
				float x = (m_Constraint.position + handOffset).x;
				x = Mathf.Max(defaultNegativeXPos, x);
				m_Constraint.position = new Vector3(x, m_Constraint.position.y, m_Constraint.position.z);
			}
			if (m_PositiveXMove.enableConstraint)
			{
				float x = (m_Constraint.position + handOffset).x;
				x = Mathf.Min(defaultPositiveXPos, x);
				m_Constraint.position = new Vector3(x, m_Constraint.position.y, m_Constraint.position.z);
			}
			if (m_NegativeYMove.enableConstraint)
			{
				float y = (m_Constraint.position + handOffset).y;
				y = Mathf.Max(defaultNegativeYPos, y);
				m_Constraint.position = new Vector3(m_Constraint.position.x, y, m_Constraint.position.z);
			}
			if (m_PositiveYMove.enableConstraint)
			{
				float y = (m_Constraint.position + handOffset).y;
				y = Mathf.Min(defaultPositiveYPos, y);
				m_Constraint.position = new Vector3(m_Constraint.position.x, y, m_Constraint.position.z);
			}
			if (m_NegativeZMove.enableConstraint)
			{
				float z = (m_Constraint.position + handOffset).z;
				z = Mathf.Max(defaultNegativeZPos, z);
				m_Constraint.position = new Vector3(m_Constraint.position.x, m_Constraint.position.y, z);
			}
			if (m_PositiveZMove.enableConstraint)
			{
				float z = (m_Constraint.position + handOffset).z;
				z = Mathf.Min(defaultPositiveZPos, z);
				m_Constraint.position = new Vector3(m_Constraint.position.x, m_Constraint.position.y, z);
			}

			previousHandPose = handPose;
		}

		public override void OnEndGrabbed(IGrabbable grabbable)
		{
			currentGrabPose = GrabPose.Identity;
			previousHandPose = Pose.identity;
		}
	}
}
