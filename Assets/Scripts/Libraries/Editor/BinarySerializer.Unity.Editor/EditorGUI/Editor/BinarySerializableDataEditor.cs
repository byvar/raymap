using UnityEditor;

namespace BinarySerializer.Unity.Editor
{
	[CustomEditor(typeof(BinarySerializableDataComponent))]
	[CanEditMultipleObjects]
	public class BinarySerializableDataEditor : UnityEditor.Editor
	{
		private EditorGUISerializer _serializer;
		private bool _dataFoldout = true;

		public override void OnInspectorGUI()
		{
			BinarySerializableDataComponent comp = (BinarySerializableDataComponent)serializedObject.targetObject;

			_dataFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_dataFoldout, comp.Data.GetType().Name);

			EditorGUI.indentLevel++;

			if (_dataFoldout)
				comp.Data.Serialize(_serializer ??= new EditorGUISerializer(comp.Data.Context));

			EditorGUI.indentLevel--;

			EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}