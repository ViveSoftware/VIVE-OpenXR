using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace VIVE.OpenXR.Toolkits.RealisticHandInteraction
{
	[CustomEditor(typeof(HandGrabInteractor))]
	public class HandGrabInteractorEditor : UnityEditor.Editor
	{
		private SerializedProperty m_Handedness, m_GrabDistance, m_OnBeginGrab, m_OnEndGrab;
		private bool showEvent = false;

		private void OnEnable()
		{
			m_Handedness = serializedObject.FindProperty("m_Handedness");
			m_GrabDistance = serializedObject.FindProperty("m_GrabDistance");
			m_OnBeginGrab = serializedObject.FindProperty("m_OnBeginGrab");
			m_OnEndGrab = serializedObject.FindProperty("m_OnEndGrab");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(m_Handedness);
			EditorGUILayout.PropertyField(m_GrabDistance);
			showEvent = EditorGUILayout.Foldout(showEvent, "Grab Event");
			if (showEvent)
			{
				EditorGUILayout.PropertyField(m_OnBeginGrab);
				EditorGUILayout.PropertyField(m_OnEndGrab);

			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
