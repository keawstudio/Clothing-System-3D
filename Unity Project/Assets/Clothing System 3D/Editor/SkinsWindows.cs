using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class SkinsWindow : EditorWindow {

		public static SkinsWindow instance;

		int flagEdit = 0;
		int selectedRecord = 0;
		int bodyStrucSelected = -1;
		int bodyModelSelected = -1;
		int bodyModelIndex = 0;

		string filterList = "";
		string skinName = "";
		bool defSkin = false;

		Vector2 scrollSkins;
		Vector2 scrollMaterial;

		SkinsTable skinTable = new SkinsTable();
		BodyStructureTable bodyStrucTable = new BodyStructureTable();
		BodySubtypeTable bodySubType = new BodySubtypeTable();
		BodyModelTable bodyModel = new BodyModelTable();

		List<BodyStructureTable.BodyPropsSlots> bodyStrucSlots = new List<BodyStructureTable.BodyPropsSlots>();
		List<BodyStructureTable.BodyStructure> bodyStrucList = new List<BodyStructureTable.BodyStructure>();
		List<BodyModelTable.BodyModel> bodyModelList = new List<BodyModelTable.BodyModel>();

		List<SkinsTable.Skin> listSkins = new List<SkinsTable.Skin> ();
			
		string[] bodyModelOptions = new string[]{""};
		string[] bodySubtypeOptions = new string[]{""};

		class MaterialPrefab{
			public int IDSlot = 0;
			public string SlotName = "";
			public string PrefabName = "";
			public Material Prefab;
			public Material PrefabSelected;
			public bool PrefabError;
		}

		List<MaterialPrefab> materialList = new List<MaterialPrefab> ();

		private CSConfig _config;


		//Show Window
		public static void ShowSkinsWindow(){

			instance = (SkinsWindow)EditorWindow.GetWindow (typeof(SkinsWindow));
			instance.titleContent = new GUIContent ("Skins List");

		}

		private void OnEnable(){

			_config = new CSConfig ();

			if (_config == null) Debug.LogError("CSConfig cannot be loaded. This script will not run properly");

			bodyStrucTable.LoadStructures ();
			bodyStrucTable.LoadSkinSlots ();
			bodySubType.LoadTable ();

			bodyStrucList = bodyStrucTable.GetList();

			bodyModel.LoadList ();
			bodyModelList = bodyModel.GetList ();

			System.Array.Resize (ref bodyModelOptions, bodyModelList.Count);

			for (int i = 0; i < bodyModelList.Count; i++) {
				bodyModelOptions[i] = bodyModelList[i].Name;
			}

			skinTable.LoadSkinsList ();
			listSkins = skinTable.GetSkinList ();

			skinTable.LoadSlotsList ();


		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				skinTable.SaveTable ();

				skinTable.UnloadSkinsList();
				skinTable.UnloadSlotsList();
				bodyStrucTable.UnloadStructures();
				bodyStrucTable.UnloadSkinSlots();
				bodySubType.UnloadTable();
				bodyModel.UnloadList();

			}

		}

		private void OnGUI(){

			//return;

			ShowItems ();

		}

		private void ShowItems(){

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("List of Skins",EditorStyles.boldLabel);

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

			if (GUILayout.Button("Add Skin", GUILayout.Width(150))){
				//Start the add for new structure
				NewRecord();
			}

			ShowList ();

			GUILayout.Space (15);

			EditorGUILayout.HelpBox ("Save the table to avoid losing data.", MessageType.Warning);

			if (GUILayout.Button("Save table", GUILayout.Width(150))){
				//Start the add for new structure
				skinTable.SaveTable();
			}

			GUILayout.EndVertical ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("Details of Skin", EditorStyles.boldLabel, GUILayout.Width(300));

			GUILayout.Space (15);

			if (flagEdit > 0)
				ShowDetails ();

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();
		}

		private void ShowList(){

			scrollSkins = GUILayout.BeginScrollView (scrollSkins, GUILayout.Width (350));

			for (int iList = 0; iList < listSkins.Count; iList++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (listSkins[iList].Name, GUILayout.Width(100))) {
					SelectRecord (listSkins[iList].ID);
				}

				GUILayout.Label(bodyModelList.Find(x => x.ID == listSkins[iList].IDModel).Name);
					
				if (GUILayout.Button ("-", GUILayout.Width (25))) {
					skinTable.DeleteSkin(listSkins[iList].ID);
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
			GUILayout.Label ("Body Model:");
			bodyModelIndex = EditorGUILayout.Popup (bodyModelSelected, bodyModelOptions);
			if (bodyModelIndex != bodyModelSelected) {
				FilterSlots (bodyModelIndex);
				bodyModelSelected = bodyModelIndex;
			}
			GUILayout.EndHorizontal ();

			defSkin = GUILayout.Toggle (defSkin, "Default model's skin");

			if (bodyModelSelected > -1) {

				scrollMaterial = GUILayout.BeginScrollView (scrollMaterial);

				for (int iMats = 0; iMats < materialList.Count; iMats++) {

					GUILayout.Label (materialList [iMats].SlotName,EditorStyles.boldLabel);

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Material name:");
					materialList [iMats].PrefabName = GUILayout.TextField (materialList [iMats].PrefabName, GUILayout.Width (150));
					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Material:");
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

			}

			GUILayout.Space (15);

			if (GUILayout.Button ("Save Skin")) {
				SaveRecord ();
			}

		}

		private void SaveRecord(){

			int idModel = 0;

			idModel = bodyModelList [bodyModelSelected].ID;

			if (flagEdit == 1) {
				selectedRecord = skinTable.AddSkin (skinName, idModel, defSkin);
			} else {
				skinTable.UpdateSkin (selectedRecord, skinName, idModel, defSkin);
			}

			skinTable.DeleteSlots (selectedRecord);

			for (int i = 0; i < materialList.Count; i++) {
				if (materialList [i].PrefabName.Length > 0) {
					skinTable.AddSlot (selectedRecord, materialList [i].IDSlot, materialList [i].PrefabName);
				}
			}

			flagEdit = 2;

			listSkins = skinTable.GetSkinList ();

		}

		private void NewRecord(){

			selectedRecord = 0;
			skinName = "";
			bodyStrucSelected = -1;
			bodyModelSelected = -1;
			defSkin = false;

			flagEdit = 1;
		}

		private void SelectRecord(int id){

			flagEdit = 2;

			selectedRecord = id;

			SkinsTable.Skin tRec = listSkins.Find (x => x.ID == id);
			skinName = tRec.Name;
			bodyModelSelected = bodyModelList.FindIndex (x => x.ID == tRec.IDModel);
			defSkin = tRec.DefaultSkin;

			FilterSlots(bodyModelSelected);

			List<SkinsTable.SkinSlots> tList = skinTable.GetSlotsList (selectedRecord,true);
			//Fill the material list
			for (int i = 0; i < tList.Count; i++) {
				MaterialPrefab tMat = new MaterialPrefab ();
				tMat = materialList.Find (x => x.IDSlot == tList [i].IDSkinSlot);
				tMat.PrefabName = tList [i].MaterialName;

				//Try to find the object
				Material prefabToLoad = (Material)Resources.Load(_config.GetPathName(CSConfig.PathTypeEnum.SkinsPath)+"/"+tMat.PrefabName);
				if (prefabToLoad != null) {
					tMat.Prefab = prefabToLoad;
					tMat.PrefabSelected = prefabToLoad;
				} else {
					tMat.PrefabError = true;
				}

			}
		}

		private void FilterSlots(int indexStruc){

			int idStruc = bodyModelList [indexStruc].IDBodyStructure;

			//Filter available slots
			materialList.Clear();
			List<BodyStructureTable.BodySkinsSlots> tList = new List<BodyStructureTable.BodySkinsSlots> ();
			tList = bodyStrucTable.GetSkinSlotList (idStruc);

			for (int i = 0; i < tList.Count; i++) {
				MaterialPrefab toAdd = new MaterialPrefab ();

				toAdd.SlotName = tList [i].SlotName;
				toAdd.IDSlot = tList [i].IDSlot;

				materialList.Add (toAdd);

			}
		}

		private void FilterList(bool filter){

		}

	}
}