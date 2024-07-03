// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections.Generic;
using System.Text;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	/// <summary>
	/// This class is designed to edit grab gestures.
	/// </summary>
	[RequireComponent(typeof(HandMeshManager))]
	public class CustomGrabPose : MonoBehaviour
	{
#if UNITY_EDITOR

		#region Log

		const string LOG_TAG = "Wave.Essence.Hand.Interaction.CustomGrabPose";
		private StringBuilder m_sb = null;
		internal StringBuilder sb
		{
			get
			{
				if (m_sb == null) { m_sb = new StringBuilder(); }
				return m_sb;
			}
		}
		private void DEBUG(StringBuilder msg) { Debug.Log($"{LOG_TAG}, {msg}"); }
		private void WARNING(StringBuilder msg) { Debug.LogWarning($"{LOG_TAG}, {msg}"); }
		private void ERROR(StringBuilder msg) { Debug.LogError($"{LOG_TAG}, {msg}"); }
		int logFrame = 0;
		bool printIntervalLog => logFrame == 0;

		#endregion

		/// <summary>
		/// This structure is designed to record the rotation values of each joint at different bend angles.
		/// </summary>
		private struct JointBendingRotation
		{
			public FingerId fingerId;
			public string jointPath;
			public int bending;
			public Quaternion rotation;

			public JointBendingRotation(FingerId in_FingerId, string in_JointPath, int in_Bending, Quaternion in_Rotation)
			{
				fingerId = in_FingerId;
				jointPath = in_JointPath;
				bending = in_Bending;
				rotation = in_Rotation;
			}

			public JointBendingRotation Identity => new JointBendingRotation(FingerId.Invalid, "", -1, Quaternion.identity);
		}

		[SerializeField]
		private AnimationClip animationClip;

		[SerializeField]
		[Range(1, 30)]
		private int thumbBending = 1;
		[SerializeField]
		[Range(1, 30)]
		private int indexBending = 1;
		[SerializeField]
		[Range(1, 30)]
		private int middleBending = 1;
		[SerializeField]
		[Range(1, 30)]
		private int ringBending = 1;
		[SerializeField]
		[Range(1, 30)]
		private int pinkyBending = 1;

		private HandMeshManager m_HandMesh;
		private Dictionary<FingerId, int> fingersBendingMapping = new Dictionary<FingerId, int>()
		{
			{FingerId.Thumb, 1 },
			{FingerId.Index, 1 },
			{FingerId.Middle, 1 },
			{FingerId.Ring, 1 },
			{FingerId.Pinky, 1 },
		};
		private static readonly Dictionary<string, FingerId> jointsPathMapping = new Dictionary<string, FingerId>()
		{
			{"WaveBone_0", FingerId.Invalid },
			{"WaveBone_1", FingerId.Invalid },
			{"WaveBone_2", FingerId.Thumb },
			{"WaveBone_2/WaveBone_3", FingerId.Thumb },
			{"WaveBone_2/WaveBone_3/WaveBone_4", FingerId.Thumb },
			{"WaveBone_2/WaveBone_3/WaveBone_4/WaveBone_5", FingerId.Thumb },
			{"WaveBone_6", FingerId.Index },
			{"WaveBone_6/WaveBone_7", FingerId.Index },
			{"WaveBone_6/WaveBone_7/WaveBone_8", FingerId.Index },
			{"WaveBone_6/WaveBone_7/WaveBone_8/WaveBone_9", FingerId.Index },
			{"WaveBone_6/WaveBone_7/WaveBone_8/WaveBone_9/WaveBone_10", FingerId.Index },
			{"WaveBone_11", FingerId.Middle },
			{"WaveBone_11/WaveBone_12", FingerId.Middle },
			{"WaveBone_11/WaveBone_12/WaveBone_13", FingerId.Middle },
			{"WaveBone_11/WaveBone_12/WaveBone_13/WaveBone_14", FingerId.Middle },
			{"WaveBone_11/WaveBone_12/WaveBone_13/WaveBone_14/WaveBone_15", FingerId.Middle },
			{"WaveBone_16", FingerId.Ring },
			{"WaveBone_16/WaveBone_17", FingerId.Ring },
			{"WaveBone_16/WaveBone_17/WaveBone_18", FingerId.Ring },
			{"WaveBone_16/WaveBone_17/WaveBone_18/WaveBone_19", FingerId.Ring },
			{"WaveBone_16/WaveBone_17/WaveBone_18/WaveBone_19/WaveBone_20", FingerId.Ring },
			{"WaveBone_21", FingerId.Pinky },
			{"WaveBone_21/WaveBone_22", FingerId.Pinky },
			{"WaveBone_21/WaveBone_22/WaveBone_23", FingerId.Pinky },
			{"WaveBone_21/WaveBone_22/WaveBone_23/WaveBone_24", FingerId.Pinky },
			{"WaveBone_21/WaveBone_22/WaveBone_23/WaveBone_24/WaveBone_25", FingerId.Pinky },
		};
		private List<JointBendingRotation> jointsBending = new List<JointBendingRotation>();

		private readonly float k_GrabDistance = 0.1f;
		private HandGrabInteractable candidate = null;
		private Pose wristPose = Pose.identity;
		private Quaternion[] fingerJointRotation = new Quaternion[jointsPathMapping.Count];

		#region MonoBehaviours

		private void OnEnable()
		{
			m_HandMesh = transform.GetComponent<HandMeshManager>();
			if (m_HandMesh == null)
			{
				sb.Clear().Append("Failed to find HandMeshRenderer.");
				ERROR(sb);
			}

			if (animationClip != null)
			{
				EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(animationClip);

				foreach (string propertyName in jointsPathMapping.Keys)
				{
					SetEachFrameRotation(curveBindings, propertyName);
				}
			}
			else
			{
				sb.Clear().Append("Failed to find hand grab animation. The hand model will not change when you change bend angles of finger.");
				sb.Append("However, you still can record  grab pose if you use direct preview mode.");
				WARNING(sb);
			}
		}

		private void OnDisable()
		{
			jointsBending.Clear();
		}

		private void Update()
		{
			if (IsFingerBendingUpdated())
			{
				for (int i = 0; i < fingerJointRotation.Length; i++)
				{
					var jointInfo = jointsPathMapping.ElementAt(i);
					string jointPath = jointInfo.Key;
					FingerId fingerId = jointInfo.Value;
					int bending = -1;
					if (fingersBendingMapping.ContainsKey(fingerId))
					{
						bending = fingersBendingMapping[fingerId] - 1;
					}
					if (jointsBending.Count(x => x.fingerId == fingerId && x.jointPath == jointPath && x.bending == bending) > 0)
					{
						JointBendingRotation jointRotation = jointsBending.FirstOrDefault(x => x.fingerId == fingerId && x.jointPath == jointPath && x.bending == bending);
						fingerJointRotation[i] = jointRotation.rotation;
					}
					else
					{
						fingerJointRotation[i] = Quaternion.identity;
					}
				}

				if (m_HandMesh != null)
				{
					for (int i = 0; i < fingerJointRotation.Length; i++)
					{
						JointType joint = (JointType)i;
						m_HandMesh.GetJointPositionAndRotation(joint, out Vector3 jointPosition, out _, local: true);
						m_HandMesh.SetJointPositionAndRotation(joint, jointPosition, fingerJointRotation[i], local: true);
					}
				}
			}

			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				FindNearInteractable();
				SavePoseWithCandidate();
			}
		}

		#endregion

		/// <summary>
		/// Reads the rotation of each joint frame by frame and records it.
		/// </summary>
		/// <param name="curveBindings">All the float curve bindings currently stored in the clip.</param>
		/// <param name="jointPath">The path of the joint.</param>
		private void SetEachFrameRotation(EditorCurveBinding[] curveBindings, string jointPath)
		{
			const int propertyCount = 4;
			const int animeCount = 30;
			const float animeFPS = 60.0f;

			List<EditorCurveBinding> matchCurve = new List<EditorCurveBinding>();
			foreach (EditorCurveBinding binding in curveBindings)
			{
				if (binding.path.Equals(jointPath))
				{
					matchCurve.Add(binding);
				}

				if (matchCurve.Count == propertyCount)
				{
					break;
				}
			}

			if (matchCurve.Count == propertyCount)
			{
				for (int i = 0; i < animeCount; i++)
				{
					Quaternion rotation = Quaternion.identity;
					foreach (var curveBinding in matchCurve)
					{
						AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, curveBinding);
						switch (curveBinding.propertyName)
						{
							case "m_LocalRotation.x":
								rotation.x = curve.Evaluate(i / animeFPS);
								break;
							case "m_LocalRotation.y":
								rotation.y = curve.Evaluate(i / animeFPS);
								break;
							case "m_LocalRotation.z":
								rotation.z = curve.Evaluate(i / animeFPS);
								break;
							case "m_LocalRotation.w":
								rotation.w = curve.Evaluate(i / animeFPS);
								break;
						}
					}
					jointsBending.Add(new JointBendingRotation(jointsPathMapping[jointPath], jointPath, i, rotation));
				}
			}
		}

		/// <summary>
		/// Checks if the current finger bend angle has changed.
		/// </summary>
		/// <returns>True if the bend angle has changed; otherwise, false.</returns>
		private bool IsFingerBendingUpdated()
		{
			bool updated = false;

			if (fingersBendingMapping[FingerId.Thumb] != thumbBending)
			{
				fingersBendingMapping[FingerId.Thumb] = thumbBending;
				updated = true;
			}
			if (fingersBendingMapping[FingerId.Index] != indexBending)
			{
				fingersBendingMapping[FingerId.Index] = indexBending;
				updated = true;
			}
			if (fingersBendingMapping[FingerId.Middle] != middleBending)
			{
				fingersBendingMapping[FingerId.Middle] = middleBending;
				updated = true;
			}
			if (fingersBendingMapping[FingerId.Ring] != ringBending)
			{
				fingersBendingMapping[FingerId.Ring] = ringBending;
				updated = true;
			}
			if (fingersBendingMapping[FingerId.Pinky] != pinkyBending)
			{
				fingersBendingMapping[FingerId.Pinky] = pinkyBending;
				updated = true;
			}

			return updated;
		}

		/// <summary>
		/// Update hand pose from HandMeshRenderer.
		/// </summary>
		/// <returns>Return true if updating hand pose from HandMeshRenderer; otherwise.</returns>
		private bool UpdateHandPose()
		{
			bool updated = false;
			if (m_HandMesh != null)
			{
				for (int i = 0; i < fingerJointRotation.Length; i++)
				{
					if (i == (int)JointType.Wrist)
					{
						m_HandMesh.GetJointPositionAndRotation(JointType.Wrist, out wristPose.position, out wristPose.rotation);
					}
					m_HandMesh.GetJointPositionAndRotation((JointType)i, out _, out fingerJointRotation[i], local: true);
				}
				updated = true;
			}
			if (!updated)
			{
				sb.Clear().Append("Failed to update hand pose.");
				DEBUG(sb);
			}
			return updated;
		}

		/// <summary>
		/// Finds the nearest interactable object to the hand.
		/// </summary>
		public void FindNearInteractable()
		{
			if (!UpdateHandPose()) { return; }

			candidate = null;
			float maxScore = 0;
			foreach (HandGrabInteractable interactable in GrabManager.handGrabbables)
			{
				float distanceScore = interactable.CalculateDistanceScore(wristPose.position, k_GrabDistance);
				if (distanceScore > maxScore)
				{
					maxScore = distanceScore;
					candidate = interactable;
				}
			}
			if (candidate == null)
			{
				sb.Clear().Append("Unable to find a suitable candidate.");
				WARNING(sb);
			}
		}

		/// <summary>
		/// Save the position and rotation offset with the candidate.
		/// </summary>
		public void SavePoseWithCandidate()
		{
			if (!UpdateHandPose() || candidate == null) { return; }

			Quaternion[] clone = new Quaternion[fingerJointRotation.Length];
			Array.Copy(fingerJointRotation, clone, fingerJointRotation.Length);
			GrabPose grabPose = GrabPose.Identity;

			grabPose.Update($"Grab Pose {candidate.grabPoses.Count + 1}", clone, m_HandMesh.isLeft);
			grabPose.grabOffset = new GrabOffset(wristPose.position, wristPose.rotation, candidate.transform.position, candidate.transform.rotation);
			if (!candidate.grabPoses.Contains(grabPose))
			{
				candidate.grabPoses.Add(grabPose);
			}
			GrabbablePoseRecorder.SaveChanges();

			sb.Clear().Append("Save grab pose successfully.");
			DEBUG(sb);
		}
#endif
	}
}
