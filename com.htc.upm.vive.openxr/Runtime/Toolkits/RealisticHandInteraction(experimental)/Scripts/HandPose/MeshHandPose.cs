using System.Collections;
using UnityEngine;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	public class MeshHandPose : HandPose
	{
		[SerializeField]
		private HandMeshManager m_HandMesh;
		
		private bool keepUpdate = false;

		protected override void OnEnable()
		{
			StartCoroutine(WaitInit());
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (keepUpdate)
			{
				keepUpdate = false;
				StopCoroutine(UpdatePose());
			}
		}

		public void SetHandMeshRenderer(HandMeshManager handMeshRenderer)
		{
			m_HandMesh = handMeshRenderer;
			SetType(handMeshRenderer.isLeft ? HandPoseType.MESH_LEFT : HandPoseType.MESH_RIGHT);
		}

		public bool SetJointPose(JointType joint, Pose jointPose, bool local = false)
		{
			if (m_HandMesh != null)
			{
				return m_HandMesh.SetJointPositionAndRotation(joint, jointPose.position, jointPose.rotation, local);
			}
			return false;
		}

		private IEnumerator WaitInit()
		{
			yield return new WaitUntil(() => m_Initialized);
			base.OnEnable();
			if (!keepUpdate)
			{
				keepUpdate = true;
				StartCoroutine(UpdatePose());
			}
		}

		private IEnumerator UpdatePose()
		{
			while (keepUpdate)
			{
				yield return new WaitForFixedUpdate();

				HandPose handPose = HandPoseProvider.GetHandPose(m_HandMesh.isLeft ? HandPoseType.HAND_LEFT : HandPoseType.HAND_RIGHT);
				m_IsTracked = handPose.IsTracked();

				for (int i = 0; i < poseCount; i++)
				{
					if (m_HandMesh.GetJointPositionAndRotation((JointType)i, out Vector3 position, out Quaternion rotation) &&
						m_HandMesh.GetJointPositionAndRotation((JointType)i, out Vector3 localPosition, out Quaternion localRotation, local: true))
					{
						m_Position[i] = position;
						m_Rotation[i] = rotation;
						m_LocalPosition[i] = localPosition;
						m_LocalRotation[i] = localRotation;
					}
					else
					{
						m_Position[i] = Vector3.zero;
						m_Rotation[i] = Quaternion.identity;
						m_LocalPosition[i] = Vector3.zero;
						m_LocalRotation[i] = Quaternion.identity;
					}
				}
			}
		}
	}
}
