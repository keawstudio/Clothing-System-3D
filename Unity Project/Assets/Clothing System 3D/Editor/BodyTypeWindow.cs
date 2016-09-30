using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class BodyTypeWindow : EditorWindow {

		public static BodyTypeWindow instance;

		List<BodyTypeTable.BodyType> bodyTypeList = new List<BodyTypeTable.BodyType>();
		BodyTypeTable bodyTypeTable = new BodyTypeTable();

		Vector2 scrollPos;
		string newItem = "";


		public static void ShowBodyTypeWindow(){

			instance = (BodyTypeWindow)EditorWindow.GetWindow (typeof(BodyTypeWindow));
			instance.titleContent = new GUIContent ("Body Type List");
		}

		private void OnEnable(){

			bodyTypeTable.LoadTable ();
			bodyTypeList = bodyTypeTable.GetList ();

		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				bodyTypeTable.SaveTable();

				bodyTypeTable.UnloadTable ();
			}

		}

		private void OnGUI(){

			ShowItems ();

		}

		private void ShowItems(){

			int contItems = 0;

			GUILayout.Label ("Table of Body Types", EditorStyles.boldLabel);

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

			for (int iLoopItems = 0; iLoopItems < bodyTypeList.Count; iLoopItems++) {

				EditorGUILayout.BeginHorizontal ();

				GUILayout.Label (bodyTypeList [iLoopItems].ID.ToString(), GUILayout.Width (25));

				bodyTypeList[iLoopItems].Name = GUILayout.TextField(bodyTypeList[iLoopItems].Name,GUILayout.Width(250));

				if (GUILayout.Button("-",GUILayout.Width(25))){
					int canDelete = bodyTypeTable.DeleteRecord (bodyTypeList [iLoopItems].ID);
					if (canDelete > 0) this.ShowNotification(new GUIContent(EditorGUILayout.TextField("Cannot delete type. There is a body structure attached to it.")));
				}
					
				EditorGUILayout.EndHorizontal ();

				contItems++;
			}

			EditorGUILayout.EndScrollView ();

			if (contItems == 0)
				EditorGUILayout.HelpBox ("This table is empty", MessageType.Info);

			GUILayout.Space (15);

			GUILayout.BeginHorizontal ();

			GUILayout.Label ("New body type:");
			newItem = GUILayout.TextField (newItem, GUILayout.Width (200));

			if (GUILayout.Button ("Save", GUILayout.Width (55))) {
				bodyTypeTable.AddRecord (newItem);
				newItem = "";
			}

			GUILayout.EndHorizontal ();

			GUILayout.Space (15);

			GUILayout.BeginHorizontal ();

			//Add button to save the table
			if (GUILayout.Button ("Save records",GUILayout.Width(155))) {
				bodyTypeTable.SaveTable ();
			}
			//Add button to remove all records of the selected type
			if (GUILayout.Button ("Remove all records",GUILayout.Width(155))) {
				bodyTypeTable.ClearTable();
			}

			GUILayout.EndHorizontal ();

		}
	}
}
