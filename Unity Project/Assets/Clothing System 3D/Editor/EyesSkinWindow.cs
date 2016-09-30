using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class EyesSkinWindow : EditorWindow {

		public static EyesSkinWindow instance;

		int flagEdit = 0;
		int selectedRecord = 0;

		string skinName = "";
		int eyeIndex = -1;
		int eyeSelected = -1;

		string[] eyesOptions = new string[]{""};

		Vector2 scrollEyeSkins;
		Vector2 scrollObjects;

		List<EyesTable.Eye> eyesList = new List<EyesTable.Eye> ();
		List<EyeSkinTable.EyeSkin> eyeSkinsList = new List<EyeSkinTable.EyeSkin> ();

		EyesTable eyeTable = new EyesTable();
		EyeSkinTable eyeSkinTable = new EyeSkinTable();

		class SkinMaterial {
			public int IDSkinMaterial = 0;
			public int IDEyeObject = 0;
			public string objectName = "";
			public string PrefabName = "";
			public Material Prefab;
			public Material PrefabSelected;
			public bool PrefabError;
		}

		List<SkinMaterial> materialList = new List<SkinMaterial>();

		private CSConfig _config;


		//Show Window
		public static void ShowEyesSkinWindow(){

			instance = (EyesSkinWindow)EditorWindow.GetWindow (typeof(EyesSkinWindow));
			instance.titleContent = new GUIContent ("Eyes Skin List");

		}

		private void OnEnable(){

			_config = new CSConfig ();

			if (_config == null) Debug.LogError("CSConfig cannot be loaded. This script will not run properly");

			eyeTable.LoadTable ();
			eyeTable.LoadObjectList ();

			eyesList = eyeTable.GetList ();

			System.Array.Resize (ref eyesOptions, eyesList.Count);

			for (int i = 0; i < eyesList.Count; i++) {
				eyesOptions [i] = eyesList [i].Name;
			}

			eyeSkinTable.LoadSkinsTable ();
			eyeSkinTable.LoadMaterialList ();

			eyeSkinsList = eyeSkinTable.GetSkinList ();

		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				eyeSkinTable.SaveTables ();

				eyeTable.UnloadTable();
				eyeTable.UnloadObjectList();
				eyeSkinTable.UnloadSkinsTable();

			}

		}

		private void OnGUI(){

			//return;

			ShowItems ();

		}

		private void ShowItems(){

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("List of Eyes Skins",EditorStyles.boldLabel);

			GUILayout.Space (15);

			if (GUILayout.Button("Add Eye Skin", GUILayout.Width(150))){
				//Start the add for new structure
				NewRecord();
			}

			ShowList ();

			GUILayout.Space (15);

			EditorGUILayout.HelpBox ("Save the table to avoid losing data.", MessageType.Warning);

			if (GUILayout.Button("Save table", GUILayout.Width(150))){
				eyeSkinTable.SaveTables ();
			}

			GUILayout.EndVertical ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("Details of Eye Skin", EditorStyles.boldLabel, GUILayout.Width(300));

			GUILayout.Space (15);

			if (flagEdit > 0)
				ShowDetails ();

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();
		}

		private void ShowList(){

			scrollEyeSkins = GUILayout.BeginScrollView (scrollEyeSkins, GUILayout.Width (350));

			for (int iList = 0; iList < eyeSkinsList.Count; iList++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (eyeSkinsList[iList].SkinName, GUILayout.Width(100))) {
					SelectRecord (eyeSkinsList[iList].IDSkin);
				}

				if (GUILayout.Button ("-", GUILayout.Width (25))) {
					eyeTable.DeleteEye(eyeSkinsList[iList].IDSkin);
				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndScrollView ();

		}

		private void ShowDetails(){

			GUILayout.Label ("ID: " + selectedRecord.ToString ());

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Skin name:");
			skinName = GUILayout.TextField (skinName, GUILayout.Width (250));
			GUILayout.EndHorizontal ();

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Eye:");
			eyeIndex = EditorGUILayout.Popup (eyeSelected, eyesOptions);
			if (eyeIndex != eyeSelected) {
				FilterObjects (eyeIndex);
				eyeSelected = eyeIndex;
			}
			GUILayout.EndHorizontal ();

			GUILayout.Space (5);


			scrollObjects = GUILayout.BeginScrollView (scrollObjects);

			for (int iMats = 0; iMats < materialList.Count; iMats++) {

				GUILayout.BeginHorizontal ();
				GUILayout.Label (materialList[iMats].objectName);

				materialList [iMats].PrefabName = GUILayout.TextField (materialList [iMats].PrefabName, GUILayout.Width(150));

				materialList[iMats].Prefab = (Material)EditorGUILayout.ObjectField (materialList[iMats].Prefab, typeof(Material), false, GUILayout.Width (150));
				if (materialList[iMats].Prefab  != null && materialList[iMats].Prefab != materialList[iMats].PrefabSelected) {
					materialList[iMats].PrefabName = _config.GetPrefabPath (CSConfig.PathTypeEnum.SkinsPath, AssetDatabase.GetAssetPath (materialList[iMats].Prefab ));
					materialList[iMats].PrefabSelected = materialList[iMats].Prefab ;
					materialList[iMats].PrefabError  = false;
				}
				GUILayout.EndHorizontal ();
				if (materialList[iMats].PrefabError) EditorGUILayout.HelpBox ("Material cannot be found. Load it again to update the [Material name] field.", MessageType.Error);

				GUILayout.Space (10);
			}

			GUILayout.EndScrollView ();

			GUILayout.Space (15);

			if (GUILayout.Button ("Save Eye Skin")) {
				SaveRecord ();
			}

		}

		private void SaveRecord(){

			int idEye = eyesList [eyeSelected].ID;

			if (flagEdit == 1) {
				selectedRecord = eyeSkinTable.AddSkin (skinName, idEye);
			} else {
				eyeSkinTable.EditSkin (selectedRecord, idEye, skinName);
			}

			eyeSkinTable.DeleteEyeMaterials (selectedRecord);

			for (int i = 0; i < materialList.Count; i++) {
				eyeSkinTable.AddMaterial (selectedRecord, materialList [i].IDEyeObject, materialList [i].PrefabName);
			}

		}

		private void NewRecord(){

			selectedRecord = 0;
			skinName = "";
			eyeIndex = -1;
			eyeSelected = -1;

			materialList.Clear ();

			flagEdit = 1;
		}

		private void SelectRecord(int id){

			flagEdit = 2;

			selectedRecord = id;

			int skinIndex = eyeSkinsList.FindIndex (x => x.IDSkin == id);

			eyeSelected = eyesList.FindIndex (x => x.ID == eyeSkinsList[skinIndex].IDEye);

			FilterObjects (eyeSelected);

			List<EyeSkinTable.EyeMaterial> tList = eyeSkinTable.GetMaterialList (selectedRecord, true);

			if (tList.Count > 0) {
				for (int i = 0; i < materialList.Count; i++) {
					materialList [i].PrefabName = tList.Find (x => x.IDObject == materialList [i].IDEyeObject).MaterialName;

					//try to locate the object
					Material prefabToLoad = (Material)Resources.Load(_config.GetPathName(CSConfig.PathTypeEnum.SkinsPath)+"/"+materialList[i].PrefabName);
					if (prefabToLoad != null) {
						materialList[i].Prefab = prefabToLoad;
						materialList[i].PrefabSelected = prefabToLoad;
					} else {
						materialList[i].PrefabError = true;
					}

				}
			}
			eyeIndex = -1;

			skinName = eyeSkinsList [skinIndex].SkinName;

		}

		private void FilterObjects(int index){

			int idEye = eyesList [index].ID;

			materialList.Clear ();

			List<EyesTable.EyeObject> tList = eyeTable.GetObjectsList (idEye, true);

			for (int i = 0; i < tList.Count; i++) {
				SkinMaterial toAdd = new SkinMaterial ();

				toAdd.IDEyeObject = tList [i].ID;
				toAdd.objectName = tList [i].ObjectName;

				materialList.Add (toAdd);

			}
		}

	}
}