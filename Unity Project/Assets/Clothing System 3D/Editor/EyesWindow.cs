using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class EyesWindow : EditorWindow {

		public static EyesWindow instance;

		int flagEdit = 0;
		int selectedRecord = 0;

		string filterList = "";
		string eyeName = "";
		string objectName = "";

		Vector2 scrollEyes;
		Vector2 scrollObjects;

		List<EyesTable.Eye> eyesList = new List<EyesTable.Eye> ();
		List<EyesTable.EyeObject> eyeObjectList = new List<EyesTable.EyeObject> ();

		EyesTable eyeTable = new EyesTable();

		private CSConfig _config;


		//Show Window
		public static void ShowEyesWindow(){

			instance = (EyesWindow)EditorWindow.GetWindow (typeof(EyesWindow));
			instance.titleContent = new GUIContent ("Eyes List");

		}

		private void OnEnable(){

			_config = new CSConfig ();

			if (_config == null) Debug.LogError("CSConfig cannot be loaded. This script will not run properly");

			eyeTable.LoadTable ();
			eyeTable.LoadObjectList ();

			eyesList = eyeTable.GetList ();

		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				eyeTable.SaveTables ();

				eyeTable.UnloadTable ();
				eyeTable.UnloadObjectList ();

			}

		}

		private void OnGUI(){

			//return;

			ShowItems ();

		}

		private void ShowItems(){

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("List of Eyes",EditorStyles.boldLabel);

			GUILayout.Space (15);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Filter: ");
			filterList = GUILayout.TextField (filterList, GUILayout.Width(150));
			if (GUILayout.Button("Filter", GUILayout.Width(80))){
				FilterList (true);
			}
			if (GUILayout.Button ("x", GUILayout.Width (25))) {
				FilterList (false);
			}
			GUILayout.EndHorizontal ();

			GUILayout.Space (15);

			if (GUILayout.Button("Add Eye", GUILayout.Width(150))){
				//Start the add for new structure
				NewRecord();
			}

			ShowList ();

			GUILayout.Space (15);

			EditorGUILayout.HelpBox ("Save the table to avoid losing data.", MessageType.Warning);

			if (GUILayout.Button("Save table", GUILayout.Width(150))){
				eyeTable.SaveTables ();
			}

			GUILayout.EndVertical ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("Details of Eye", EditorStyles.boldLabel, GUILayout.Width(300));

			GUILayout.Space (15);

			if (flagEdit > 0)
				ShowDetails ();

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();
		}

		private void ShowList(){

			scrollEyes = GUILayout.BeginScrollView (scrollEyes, GUILayout.Width (350));

			for (int iList = 0; iList < eyesList.Count; iList++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (eyesList[iList].Name, GUILayout.Width(100))) {
					SelectRecord (eyesList[iList].ID);
				}

				if (GUILayout.Button ("-", GUILayout.Width (25))) {
					eyeTable.DeleteEye(eyesList[iList].ID);
				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndScrollView ();

		}

		private void ShowDetails(){

			GUILayout.Label ("ID: " + selectedRecord.ToString ());

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Eye name:");
			eyeName = GUILayout.TextField (eyeName, GUILayout.Width (250));
			GUILayout.EndHorizontal ();

			GUILayout.Space (5);

			if (selectedRecord > 0) {

				scrollObjects = GUILayout.BeginScrollView (scrollObjects);

				for (int iMats = 0; iMats < eyeObjectList.Count; iMats++) {

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Object name:");
					eyeObjectList[iMats].ObjectName = GUILayout.TextField (eyeObjectList[iMats].ObjectName, GUILayout.Width (150));

					if (GUILayout.Button ("-")) {
						eyeObjectList.RemoveAt (iMats);
					}
					GUILayout.EndHorizontal ();

					GUILayout.Space (10);
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Object name:");
				objectName = GUILayout.TextField (objectName, GUILayout.Width (150));

				if (GUILayout.Button ("Save")) {
					EyesTable.EyeObject toAdd = new EyesTable.EyeObject ();
					toAdd.IDEye = selectedRecord;
					toAdd.ObjectName = objectName;
					eyeObjectList.Add (toAdd);
					objectName = "";
				}
				GUILayout.EndHorizontal ();

				GUILayout.EndScrollView ();

			}

			GUILayout.Space (15);

			if (GUILayout.Button ("Save Eye")) {
				SaveRecord ();
			}

		}

		private void SaveRecord(){

			if (flagEdit == 1) {
				selectedRecord = eyeTable.AddEye(eyeName);
			} else {
				eyeTable.EditEye(selectedRecord, eyeName);
			}

			eyeTable.DeleteObjects(selectedRecord);

			for (int i = 0; i < eyeObjectList.Count; i++) {
				eyeTable.AddEyeObject(selectedRecord, eyeObjectList[i].ObjectName);
			}

			flagEdit = 2;

			eyesList = eyeTable.GetList();

		}

		private void NewRecord(){

			selectedRecord = 0;
			eyeName = "";

			flagEdit = 1;
		}

		private void SelectRecord(int id){

			flagEdit = 2;

			selectedRecord = id;

			eyeName = eyeTable.GetEyeName (selectedRecord);

			eyeObjectList = eyeTable.GetObjectsList (selectedRecord, true);

		}

		private void FilterList(bool filter){

		}

	}
}