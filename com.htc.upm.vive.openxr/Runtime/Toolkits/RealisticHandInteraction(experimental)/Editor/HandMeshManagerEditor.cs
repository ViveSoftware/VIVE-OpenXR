using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	[CustomEditor(typeof(HandMeshManager))]
	public class HandMeshManagerEditor : Editor
	{
		private HandMeshManager m_HandMesh;
		private SerializedProperty m_Handedness, m_EnableCollider, m_HandJoints;

		private bool showJoints = false;
		public static readonly GUIContent findJoints = EditorGUIUtility.TrTextContent("Find Joints");
		public static readonly GUIContent clearJoints = EditorGUIUtility.TrTextContent("All Clear");

		private void OnEnable()
		{
			m_HandMesh = target as HandMeshManager;
			m_Handedness = serializedObject.FindProperty("m_Handedness");
			m_EnableCollider = serializedObject.FindProperty("m_EnableCollider");
			m_HandJoints = serializedObject.FindProperty("m_HandJoints");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

			EditorGUILayout.HelpBox("Please check if your model is used to bind left hand poses", MessageType.None);
			EditorGUILayout.PropertyField(m_Handedness, new GUIContent("Handedness"));

			EditorGUILayout.HelpBox("Please check if you want the hand model with collision enabled.", MessageType.None);
			EditorGUILayout.PropertyField(m_EnableCollider, new GUIContent("Enable Collider"));

			showJoints = EditorGUILayout.Foldout(showJoints, "Hand Bones Reference");
			if (showJoints)
			{
				EditorGUILayout.HelpBox("Please change rotation to make sure your model should palm faces forward and fingers points up in global axis.", MessageType.Info);

				using (new EditorGUILayout.HorizontalScope())
				{
					using (new EditorGUI.DisabledScope())
					{
						if (GUILayout.Button(findJoints))
						{
							m_HandMesh.FindJoints();
						}
					}

					using (new EditorGUI.DisabledScope())
					{
						if (GUILayout.Button(clearJoints))
						{
							m_HandMesh.ClearJoints();
						}
					}
				}

				bool isDetected = false;
				for (int i = 0; i < m_HandJoints.arraySize; i++)
				{
					SerializedProperty bone = m_HandJoints.GetArrayElementAtIndex(i);
					Transform boneTransform = (Transform)bone.objectReferenceValue;
					if (boneTransform != null)
					{
						isDetected = true;
						break;
					}
				}
				if (isDetected)
				{
					for (int i = 0; i < m_HandJoints.arraySize; i++)
					{
						SerializedProperty bone = m_HandJoints.GetArrayElementAtIndex(i);
						EditorGUILayout.PropertyField(bone, new GUIContent(((JointType)i).ToString()));
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif