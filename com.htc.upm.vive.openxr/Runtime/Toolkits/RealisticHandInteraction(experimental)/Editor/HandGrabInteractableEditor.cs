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
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	[CustomEditor(typeof(HandGrabInteractable))]
	public class HandGrabInteractableEditor : UnityEditor.Editor
	{
		private static HandGrabInteractable activeGrabbable = null;
		private HandGrabInteractable handGrabbable = null;
		private SerializedProperty m_IsGrabbable, m_FingerRequirement, m_Rigidbody, m_GrabPoses, m_ShowAllIndicator, m_OnBeginGrabbed, m_OnEndGrabbed,
			m_OneHandContraintMovement, m_PreviewIndex, grabPoseName, gestureThumbPose, gestureIndexPose, gestureMiddlePose, gestureRingPose, gesturePinkyPose,
			recordedGrabRotations, isLeft, enableIndicator, autoIndicator, indicatorObject, grabOffset;
		private ReorderableList grabPoseList;
		private bool showGrabPoses = false;
		private bool showConstraint = false;
		private bool showEvent = false;

		private void OnEnable()
		{
			handGrabbable = target as HandGrabInteractable;

			m_IsGrabbable = serializedObject.FindProperty("m_IsGrabbable");
			m_FingerRequirement = serializedObject.FindProperty("m_FingerRequirement");
			m_Rigidbody = serializedObject.FindProperty("m_Rigidbody");
			m_GrabPoses = serializedObject.FindProperty("m_GrabPoses");
			m_ShowAllIndicator = serializedObject.FindProperty("m_ShowAllIndicator");
			m_OnBeginGrabbed = serializedObject.FindProperty("m_OnBeginGrabbed");
			m_OnEndGrabbed = serializedObject.FindProperty("m_OnEndGrabbed");
			m_OneHandContraintMovement = serializedObject.FindProperty("m_OneHandContraintMovement");
			m_PreviewIndex = serializedObject.FindProperty("m_PreviewIndex");

			#region ReorderableList
			grabPoseList = new ReorderableList(serializedObject, m_GrabPoses, true, true, true, true);

			grabPoseList.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, "Grab Pose List");
			};

			grabPoseList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				if (!UpdateGrabPose(m_GrabPoses.GetArrayElementAtIndex(index))) { return; }

				if (string.IsNullOrEmpty(grabPoseName.stringValue))
				{
					grabPoseName.stringValue = $"Grab Pose {index + 1}";
				}

				Rect elementRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
				grabPoseName.stringValue = EditorGUI.TextField(elementRect, grabPoseName.stringValue);

				DrawGrabGesture(ref elementRect);
				DrawHandedness(ref elementRect);
				DrawIndicator(ref elementRect);
				DrawMirrorButton(ref elementRect);
				DrawPoseOffset(ref elementRect);
				DrawFineTune(ref elementRect, index);
			};

			grabPoseList.elementHeightCallback = (int index) =>
			{
				if (!UpdateGrabPose(m_GrabPoses.GetArrayElementAtIndex(index))) { return EditorGUIUtility.singleLineHeight; }

				// Including Title, Handness, Show Indicator, Mirror Pose, Position, Rotation, Fine Tune
				int minHeight = 7;
				// To Show GrabGesture
				if (recordedGrabRotations.arraySize == 0)
				{
					minHeight += 5;
				}
				if (enableIndicator.boolValue)
				{
					// To Show Auto Indicator
					minHeight += 1;
					// To Show Indicator Gameobject
					if (!autoIndicator.boolValue)
					{
						minHeight += 1;
					}
				}
				return EditorGUIUtility.singleLineHeight * minHeight + EditorGUIUtility.standardVerticalSpacing * minHeight;
			};

			grabPoseList.onAddCallback = (ReorderableList list) =>
			{
				m_GrabPoses.arraySize++;
				if (UpdateGrabPose(m_GrabPoses.GetArrayElementAtIndex(list.count - 1)))
				{
					grabPoseName.stringValue = $"Grab Pose {list.count}";
				}
			};
			#endregion
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(m_Rigidbody);
			EditorGUILayout.PropertyField(m_IsGrabbable);
			EditorGUILayout.PropertyField(m_FingerRequirement);
			showGrabPoses = EditorGUILayout.Foldout(showGrabPoses, "Grab Pose Settings");
			if (showGrabPoses)
			{
				if (m_GrabPoses.arraySize == 0)
				{
					grabPoseList.elementHeight = EditorGUIUtility.singleLineHeight;
				}
				grabPoseList.DoLayoutList();

				bool isToggle = EditorGUILayout.Toggle("Show All Indicator", m_ShowAllIndicator.boolValue);
				if (isToggle != m_ShowAllIndicator.boolValue)
				{
					m_ShowAllIndicator.boolValue = isToggle;
					for (int i = 0; i < m_GrabPoses.arraySize; i++)
					{
						if (UpdateGrabPose(m_GrabPoses.GetArrayElementAtIndex(i)))
						{
							enableIndicator.boolValue = m_ShowAllIndicator.boolValue;
						}
					}
				}
			}

			showEvent = EditorGUILayout.Foldout(showEvent, "Grabbed Event");
			if (showEvent)
			{
				EditorGUILayout.PropertyField(m_OnBeginGrabbed);
				EditorGUILayout.PropertyField(m_OnEndGrabbed);
			}

			showConstraint = EditorGUILayout.Foldout(showConstraint, "Constraint Movement (Optional)");
			if (showConstraint)
			{
				EditorGUILayout.PropertyField(m_OneHandContraintMovement);
			}
			serializedObject.ApplyModifiedProperties();
		}

		private bool UpdateGrabPose(SerializedProperty grabPose)
		{
			SerializedProperty indicator = grabPose.FindPropertyRelative("indicator");
			if (grabPose == null || indicator == null) { return false; }

			grabPoseName = grabPose.FindPropertyRelative("grabPoseName");
			gestureThumbPose = grabPose.FindPropertyRelative("handGrabGesture.thumbPose");
			gestureIndexPose = grabPose.FindPropertyRelative("handGrabGesture.indexPose");
			gestureMiddlePose = grabPose.FindPropertyRelative("handGrabGesture.middlePose");
			gestureRingPose = grabPose.FindPropertyRelative("handGrabGesture.ringPose");
			gesturePinkyPose = grabPose.FindPropertyRelative("handGrabGesture.pinkyPose");
			recordedGrabRotations = grabPose.FindPropertyRelative("recordedGrabRotations");
			isLeft = grabPose.FindPropertyRelative("isLeft");
			enableIndicator = indicator.FindPropertyRelative("enableIndicator");
			autoIndicator = indicator.FindPropertyRelative("autoIndicator");
			indicatorObject = indicator.FindPropertyRelative("target");
			grabOffset = grabPose.FindPropertyRelative("grabOffset");
			return true;
		}

		private void AddElementHeight(ref Rect rect)
		{
			rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		private void DrawGrabGesture(ref Rect rect)
		{
			if (recordedGrabRotations.arraySize == 0)
			{
				AddElementHeight(ref rect);
				EditorGUI.PropertyField(rect, gestureThumbPose);
				AddElementHeight(ref rect);
				EditorGUI.PropertyField(rect, gestureIndexPose);
				AddElementHeight(ref rect);
				EditorGUI.PropertyField(rect, gestureMiddlePose);
				AddElementHeight(ref rect);
				EditorGUI.PropertyField(rect, gestureRingPose);
				AddElementHeight(ref rect);
				EditorGUI.PropertyField(rect, gesturePinkyPose);
			}
		}

		private void DrawHandedness(ref Rect rect)
		{
			AddElementHeight(ref rect);
			bool isToggle = EditorGUI.Toggle(rect, "Is Left", isLeft.boolValue);
			if (isToggle != isLeft.boolValue)
			{
				isLeft.boolValue = isToggle;
				SwitchRotations(ref recordedGrabRotations);
			}
		}

		private void DrawIndicator(ref Rect rect)
		{
			AddElementHeight(ref rect);
			enableIndicator.boolValue = EditorGUI.Toggle(rect, "Show Indicator", enableIndicator.boolValue);
			if (enableIndicator.boolValue)
			{
				AddElementHeight(ref rect);
				autoIndicator.boolValue = EditorGUI.Toggle(rect, "Auto Generator Indicator", autoIndicator.boolValue);
				if (!autoIndicator.boolValue)
				{
					AddElementHeight(ref rect);
					indicatorObject.objectReferenceValue = (GameObject)EditorGUI.ObjectField(rect, "Indicator", (GameObject)indicatorObject.objectReferenceValue, typeof(GameObject), true);
				}
			}
			else
			{
				m_ShowAllIndicator.boolValue = false;
			}
		}

		private void DrawMirrorButton(ref Rect rect)
		{
			AddElementHeight(ref rect);
			Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
			EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Mirror Pose"));

			Rect mirrorXRect = new Rect(rect.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing, rect.y,
				(rect.width - EditorGUIUtility.labelWidth - EditorGUIUtility.standardVerticalSpacing * 4) / 3, rect.height);
			Rect mirrorYRect = new Rect(mirrorXRect.x + mirrorXRect.width + EditorGUIUtility.standardVerticalSpacing, rect.y, mirrorXRect.width, rect.height);
			Rect mirrorZRect = new Rect(mirrorYRect.x + mirrorYRect.width + EditorGUIUtility.standardVerticalSpacing, rect.y, mirrorYRect.width, rect.height);
			if (GUI.Button(mirrorXRect, "Align X axis"))
			{
				MirrorPose(ref grabOffset, Vector3.right);
			}
			if (GUI.Button(mirrorYRect, "Align Y axis"))
			{
				MirrorPose(ref grabOffset, Vector3.up);
			}
			if (GUI.Button(mirrorZRect, "Align Z axis"))
			{
				MirrorPose(ref grabOffset, Vector3.forward);
			}
		}

		private void DrawPoseOffset(ref Rect rect)
		{
			SerializedProperty srcPos = grabOffset.FindPropertyRelative("sourcePosition");
			SerializedProperty srcRot = grabOffset.FindPropertyRelative("sourceRotation");
			SerializedProperty tgtPos = grabOffset.FindPropertyRelative("targetPosition");
			SerializedProperty tgtRot = grabOffset.FindPropertyRelative("targetRotation");
			AddElementHeight(ref rect);
			EditorGUI.Vector3Field(rect, "Position Offset", tgtPos.vector3Value - srcPos.vector3Value);
			AddElementHeight(ref rect);
			Vector3 rotEulerAngles = (Quaternion.Inverse(srcRot.quaternionValue) * tgtRot.quaternionValue).eulerAngles;
			for (int i = 0; i < 3; i++)
			{
				if (rotEulerAngles[i] > 180)
				{
					rotEulerAngles[i] = 360.0f - rotEulerAngles[i];
				}
			}
			EditorGUI.Vector3Field(rect, "Rotation Offset", rotEulerAngles);
		}

		private void DrawFineTune(ref Rect rect, int index)
		{
			AddElementHeight(ref rect);
			Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
			EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Fine Tune"));

			Rect previewRect = new Rect(rect.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing, rect.y,
				(rect.width - EditorGUIUtility.labelWidth - EditorGUIUtility.standardVerticalSpacing * 4) / 2, rect.height);
			Rect updateRect = new Rect(previewRect.x + previewRect.width + EditorGUIUtility.standardVerticalSpacing, rect.y, previewRect.width, rect.height);
			if (GUI.Button(previewRect, "Preview Grab Pose") && Application.isPlaying)
			{
				activeGrabbable = handGrabbable;
				m_PreviewIndex.intValue = index;
				ShowMeshHandPose();
			}
			GUI.enabled = activeGrabbable == handGrabbable && m_PreviewIndex.intValue == index;
			if (GUI.Button(updateRect, "Update Grab Pose"))
			{
				UpdateGrabPose();
			}
			GUI.enabled = true;
		}

		/// <summary>
		/// Convert the rotation of joints of the current hand into those of another hand.
		/// </summary>
		/// <param name="rotations">Rotation of joints of the current hand.</param>
		private void SwitchRotations(ref SerializedProperty rotations)
		{
			for (int i = 0; i < rotations.arraySize; i++)
			{
				Quaternion rotation = rotations.GetArrayElementAtIndex(i).quaternionValue;
				Quaternion newRotation = Quaternion.Euler(rotation.eulerAngles.x, -rotation.eulerAngles.y, -rotation.eulerAngles.z);
				rotations.GetArrayElementAtIndex(i).quaternionValue = newRotation;
			}
		}

		/// <summary>
		/// Mirrors the pose properties (position and rotation) of a serialized object along a specified mirror axis.
		/// </summary>
		/// <param name="pose">The serialized property representing the pose to be mirrored.</param>
		/// <param name="mirrorAxis">The axis along which the mirroring should occur.</param>
		private void MirrorPose(ref SerializedProperty pose, Vector3 mirrorAxis)
		{
			Vector3 sourcePosition = grabOffset.FindPropertyRelative("sourcePosition").vector3Value;
			Quaternion sourceRotation = grabOffset.FindPropertyRelative("sourceRotation").quaternionValue;
			Vector3 targetPosition = grabOffset.FindPropertyRelative("targetPosition").vector3Value;
			Quaternion targetRotation = grabOffset.FindPropertyRelative("targetRotation").quaternionValue;
			Vector3 reflectNormal = targetRotation * mirrorAxis;

			Vector3 diffPos = sourcePosition - targetPosition;
			Vector3 mirrorPosition = targetPosition + Vector3.Reflect(diffPos, reflectNormal);
			pose.FindPropertyRelative("sourcePosition").vector3Value = mirrorPosition;

			Vector3 sourceForward = sourceRotation * Vector3.forward;
			Vector3 sourceUp = sourceRotation * Vector3.up;
			Quaternion mirroredRotation = Quaternion.LookRotation(Vector3.Reflect(sourceForward, reflectNormal), Vector3.Reflect(sourceUp, reflectNormal));
			pose.FindPropertyRelative("sourceRotation").quaternionValue = mirroredRotation;
		}

		/// <summary>
		/// Obtain the MeshHand and set its position and rotation based on the grabOffset of grabpose.
		/// </summary>
		private void ShowMeshHandPose()
		{
			HandPose handPose = HandPoseProvider.GetHandPose(isLeft.boolValue ? HandPoseType.MESH_LEFT : HandPoseType.MESH_RIGHT);
			if (handPose != null && handPose is MeshHandPose meshHandPose)
			{
				GrabOffset grabOffsetObj = handGrabbable.grabPoses[m_PreviewIndex.intValue].grabOffset;
				Quaternion handRot = handGrabbable.transform.rotation * Quaternion.Inverse(grabOffsetObj.rotOffset);
				Quaternion handRotDiff = handRot * Quaternion.Inverse(grabOffsetObj.sourceRotation);
				Vector3 handPos = handGrabbable.transform.position - handRotDiff * grabOffsetObj.posOffset;
				meshHandPose.SetJointPose(JointType.Wrist, new Pose(handPos, handRot));

				foreach (JointType joint in Enum.GetValues(typeof(JointType)))
				{
					if (joint == JointType.Wrist || joint == JointType.Count) { continue; }

					meshHandPose.GetPosition(joint, out Vector3 pos, local: true);
					Quaternion rot = recordedGrabRotations.GetArrayElementAtIndex((int)joint).quaternionValue;
					meshHandPose.SetJointPose(joint, new Pose(pos, rot), local: true);
				}
			}
		}

		/// <summary>
		/// Update the grabpose based on position and rotation of the MeshHand and Object.
		/// </summary>
		private void UpdateGrabPose()
		{
			HandPose handPose = HandPoseProvider.GetHandPose(isLeft.boolValue ? HandPoseType.MESH_LEFT : HandPoseType.MESH_RIGHT);
			if (handPose != null && handPose is MeshHandPose meshHandPose)
			{
				meshHandPose.GetPosition(JointType.Wrist, out Vector3 wristPosition);
				meshHandPose.GetRotation(JointType.Wrist, out Quaternion wristRotation);

				Quaternion[] fingerJointRotation = new Quaternion[(int)JointType.Count];
				for (int i = 0; i < fingerJointRotation.Length; i++)
				{
					meshHandPose.GetRotation((JointType)i, out Quaternion jointRotation, local: true);
					fingerJointRotation[i] = jointRotation;
				}

				GrabPose grabPose = handGrabbable.grabPoses[m_PreviewIndex.intValue];
				grabPose.Update(grabPoseName.stringValue, fingerJointRotation, isLeft.boolValue);
				grabPose.grabOffset.Update(wristPosition, wristRotation, handGrabbable.transform.position, handGrabbable.transform.rotation);
				handGrabbable.grabPoses[m_PreviewIndex.intValue] = grabPose;
				GrabbablePoseRecorder.SaveChanges();
			}
		}
	}
}
#endif
