using UnityEditor;
using UnityEngine;

/// <summary>
/// Initialize specialized type handlers for the EditorGUISerializer here
/// </summary>
public class EditorGUISerializerConfig {
	static EditorGUISerializerConfig() {
		InitEditorGUISerializerConfig();
	}

	private static void InitEditorGUISerializerConfig() {
		var cfg = BinarySerializer.Unity.Editor.EditorGUISerializerConfig.Instance;
		cfg.AddObjectHandler<BinarySerializer.Ubisoft.CPA.PS1.MTH3D_Vector_PS1_Short, Vector3>(
				(obj, name) => EditorGUILayout.Vector3Field(name, new Vector3(obj.X, obj.Y, obj.Z)),
				(obj, value) => {
					obj.X = value.x;
					obj.Y = value.y;
					obj.Z = value.z;
				});
	}
}
