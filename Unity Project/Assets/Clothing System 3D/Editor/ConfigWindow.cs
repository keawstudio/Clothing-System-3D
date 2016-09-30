using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class ConfigWindow : EditorWindow {

		public static ConfigWindow instance;

		CSConfig config;

		List<CSConfig.PathInfo> paths = new List<CSConfig.PathInfo> ();

		Vector2 scrollPos;


		public static void ShowConfigWindow(){

			instance = (ConfigWindow)EditorWindow.GetWindow (typeof(ConfigWindow));
			instance.titleContent = new GUIContent ("Configuration");
		}

		private void OnEnable(){

			config = new CSConfig ();
			paths = config.GetList ();

		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				config.SaveTable ();
			}

		}

		private void OnGUI(){

			ShowItems ();

		}

		private void ShowItems(){

			int contItems = 0;

			GUILayout.Label ("Paths Configuration", EditorStyles.boldLabel);

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

			foreach (CSConfig.PathInfo configItem in paths) {

				EditorGUILayout.BeginHorizontal ();

				GUILayout.Label (configItem.PathType.ToString(), GUILayout.Width (150));

				configItem.PathName = GUILayout.TextField(configItem.PathName, GUILayout.Width(250));

				EditorGUILayout.EndHorizontal ();

			}

			EditorGUILayout.EndScrollView ();

			GUILayout.Space (15);


			//Add button to save the table
			if (GUILayout.Button ("Save Configuration", GUILayout.Width(155))) {
				config.SaveTable ();
			}

		}
	}
}
