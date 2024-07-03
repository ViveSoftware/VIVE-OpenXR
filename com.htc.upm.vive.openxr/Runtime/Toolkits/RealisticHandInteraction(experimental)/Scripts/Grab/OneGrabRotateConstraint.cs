using System.Text;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	public class OneGrabRotateConstraint : IOneHandContraintMovement
	{
		#region Log

		const string LOG_TAG = "VIVE.OpenXR.Toolkits.RealisticHandInteraction.OneGrabRotateConstraint";
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

		private enum RotationAxis
		{
			XAxis = 0,
			YAxis = 1,
			ZAxis = 2,
		}

		[SerializeField]
		private Transform m_Constraint;
		[SerializeField]
		private Transform m_Pivot;
		[SerializeField]
		private RotationAxis m_RotationAxis = RotationAxis.XAxis;
		[SerializeField]
		private ConstraintInfo m_ClockwiseAngle = ConstraintInfo.Identity;
		[SerializeField]
		private ConstraintInfo m_CounterclockwiseAngle = ConstraintInfo.Identity;
		private float totalRotationAngle = 0.0f;
		private Pose previousHandPose = Pose.identity;

		public override void Initialize(IGrabbable grabbable)
		{
			if (grabbable is HandGrabInteractable handGrabbable)
			{
				if (m_Constraint == null)
				{
					m_Constraint = handGrabbable.transform;
					WARNING("Since no constraint object is set, self will be used as the constraint object.");
				}
				if (m_Pivot == null)
				{
					m_Pivot = handGrabbable.transform;
					WARNING("Since no pivot is set, self will be used as the pivot.");
				}
			}
		}

		public override void OnBeginGrabbed(IGrabbable grabbable)
		{
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

			Vector3 axis = Vector3.zero;
			switch (m_RotationAxis)
			{
				case RotationAxis.XAxis: axis = Vector3.right; break;
				case RotationAxis.YAxis: axis = Vector3.up; break;
				case RotationAxis.ZAxis: axis = Vector3.forward; break;
			}
			Vector3 worldAxis = m_Pivot.TransformDirection(axis);

			Vector3 previousOffset = previousHandPose.position - m_Pivot.position;
			Vector3 previousVector = Vector3.ProjectOnPlane(previousOffset, worldAxis);

			Vector3 targetOffset = handPose.position - m_Pivot.position;
			Vector3 targetVector = Vector3.ProjectOnPlane(targetOffset, worldAxis);

			float angleDelta = Vector3.Angle(previousVector, targetVector);
			angleDelta *= Vector3.Dot(Vector3.Cross(previousVector, targetVector), worldAxis) > 0.0f ? 1.0f : -1.0f;

			float previousAngle = totalRotationAngle;
			totalRotationAngle += angleDelta;
			if (m_CounterclockwiseAngle.enableConstraint)
			{
				totalRotationAngle = Mathf.Max(totalRotationAngle, -m_CounterclockwiseAngle.value);
			}
			if (m_ClockwiseAngle.enableConstraint)
			{
				totalRotationAngle = Mathf.Min(totalRotationAngle, m_ClockwiseAngle.value);
			}
			angleDelta = totalRotationAngle - previousAngle;
			m_Constraint.RotateAround(m_Pivot.position, worldAxis, angleDelta);

			previousHandPose = handPose;
		}

		public override void OnEndGrabbed(IGrabbable grabbable)
		{
			previousHandPose = Pose.identity;
		}
	}
}
