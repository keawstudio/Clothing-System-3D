using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class BodySubtypeWindow : EditorWindow {

		public static BodySubtypeWindow instance;

		List<BodySubtypeTable.BodySubtype> bodySubtypeList = new List<BodySubtypeTable.BodySubtype>();
		BodySubtypeTable bodySubtypeTable = new BodySubtypeTable();

		Vector2 scrollPos;
		string newItem = "";


		public static void ShowBodySubtypeWindow(){

			instance = (BodySubtypeWindow)EditorWindow.GetWindow (typeof(BodySubtypeWindow));
			instance.titleContent = new GUIContent ("Body Subtype List");
		}

		private void OnEnable(){

			bodySubtypeTable.LoadTable ();
			bodySubtypeList = bodySubtypeTable.GetList ();

		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				bodySubtypeTable.SaveTable();

				bodySubtypeTable.UnloadTable ();
			}

		}

		private void OnGUI(){

			ShowItems ();

		}

		private void ShowItems(){

			int contItems = 0;

			GUILayout.Label ("Table of Body Subtypes", EditorStyles.boldLabel);

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

			for (int iLoopItems = 0; iLoopItems < bodySubtypeList.Count; iLoopItems++) {

				EditorGUILayout.BeginHorizontal ();

				GUILayout.Label (bodySubtypeList [iLoopItems].ID.ToString(), GUILayout.Width (25));

				bodySubtypeList[iLoopItems].Name = GUILayout.TextField(bodySubtypeList[iLoopItems].Name,GUILayout.Width(250));

				if (GUILayout.Button("-",GUILayout.Width(25))){
					int canDelete = bodySubtypeTable.DeleteRecord (bodySubtypeList [iLoopItems].ID);
					if (canDelete > 0) this.ShowNotification(new GUIContent(EditorGUILayout.TextField("Cannot delete subtype. There is a body structure attached to it.")));
				}

				EditorGUILayout.EndHorizontal ();

				contItems++;
			}

			EditorGUILayout.EndScrollView ();

			if (contItems == 0)
				EditorGUILayout.HelpBox ("This table is empty", MessageType.Info);

			GUILayout.Space (15);

			GUILayout.BeginHorizontal ();

			GUILayout.Label ("New body subtype:");
			newItem = GUILayout.TextField (newItem, GUILayout.Width (200));

			if (GUILayout.Button ("Save", GUILayout.Width (55))) {
				bodySubtypeTable.AddRecord (newItem);
				newItem = "";
			}

			GUILayout.EndHorizontal ();

			GUILayout.Space (15);

			GUILayout.BeginHorizontal ();

			//Add button to save the table
			if (GUILayout.Button ("Save records",GUILayout.Width(155))) {
				bodySubtypeTable.SaveTable ();
			}
			//Add button to remove all records of the selected type
			if (GUILayout.Button ("Remove all records",GUILayout.Width(155))) {
				bodySubtypeTable.ClearTable();
			}

			GUILayout.EndHorizontal ();

		}
	}
}
