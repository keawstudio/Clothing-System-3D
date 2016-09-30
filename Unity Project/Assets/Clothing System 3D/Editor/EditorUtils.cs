using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ClothingSystem3D {

	public static class EditorUtils {

		public static T CreateAsset<T>(string path) where T : ScriptableObject {

			T dataClass = (T) ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(dataClass, path);
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
			return dataClass;

		}


	}
}