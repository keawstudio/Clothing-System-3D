using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class ClothesWindow : EditorWindow {

		public static ClothesWindow instance;

		int flagEdit = 0;
		int selectedRecord = 0;
		int bodyStrucSelected = -1;
		int bodyStructIndex = 0;

		string filterList = "";

		Vector2 scrollClothes;
		Vector2 scrollSlots;
		Vector2 scrollModels;

		//Tables
		ClothTable clothTable = new ClothTable();
		BodyStructureTable bodyStrucTable = new BodyStructureTable();
		BodySubtypeTable bodySubType = new BodySubtypeTable();

		//Records and lists
		ClothTable.Cloth clothRecord = new ClothTable.Cloth();

		List<ClothTable.Cloth> clothList = new List<ClothTable.Cloth>();
		List<BodyStructureTable.BodyClothSlots> bodyStrucSlots = new List<BodyStructureTable.BodyClothSlots>();
		List<BodyStructureTable.BodyStructure> bodyStrucList = new List<BodyStructureTable.BodyStructure>();
		List<ClothTable.ClothModels> clothModels = new List<ClothTable.ClothModels>();

		string[] bodyStructOptions = new string[]{""};
		bool[] selectedSlots = new bool[]{false};

		class ModelPrefab{
			public int IDSubtype = 0;
			public string SubtypeName = "";
			public string PrefabName = "";
			public GameObject Prefab;
			public GameObject PrefabSelected;
			public bool PrefabError;
		}

		List<ModelPrefab> modelsList = new List<ModelPrefab> ();

		private CSConfig _config;


		//Show Window
		public static void ShowClothesWindow(){

			instance = (ClothesWindow)EditorWindow.GetWindow (typeof(ClothesWindow));
			instance.titleContent = new GUIContent ("Clothes List");

		}

		private void OnEnable(){

			_config = new CSConfig ();

			if (_config == null) Debug.LogError("CSConfig cannot be loaded. This script will not run properly");

			bodyStrucTable.LoadStructures ();
			bodyStrucTable.LoadClothSlots ();
			bodySubType.LoadTable ();

			bodyStrucList = bodyStrucTable.GetList();

			System.Array.Resize (ref bodyStructOptions, bodyStrucList.Count);

			for (int i = 0; i < bodyStrucList.Count; i++) {
				bodyStructOptions [i] = bodyStrucList[i].Name;
			}

			clothTable.LoadClothesList ();
			clothTable.LoadSlotsList ();
			clothTable.LoadModelsList ();

			clothList = clothTable.GetList ();

		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				clothTable.SaveTable ();

				clothTable.UnloadClothesList();
				clothTable.UnloadModelsList();
				clothTable.UnloadSlotsList();
				bodyStrucTable.UnloadStructures ();
				bodyStrucTable.UnloadClothSlots();
				bodySubType.UnloadTable();

			}

		}

		private void OnGUI(){

			ShowItems ();

		}

		private void ShowItems(){

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("List of Clothes",EditorStyles.boldLabel);

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

			if (GUILayout.Button("Add Cloth", GUILayout.Width(150))){
				//Start the add for new structure
				NewRecord();
			}

			ShowList ();

			GUILayout.Space (15);

			EditorGUILayout.HelpBox ("Save the table to avoid losing data.", MessageType.Warning);

			if (GUILayout.Button("Save table", GUILayout.Width(150))){
				//Start the add for new structure
				clothTable.SaveTable();
			}

			GUILayout.EndVertical ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("Details of Cloth", EditorStyles.boldLabel, GUILayout.Width(300));

			GUILayout.Space (15);

			if (flagEdit > 0)
				ShowDetails ();

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();

		}

		private void ShowList(){

			scrollClothes = GUILayout.BeginScrollView (scrollClothes, GUILayout.Width (350));

			for (int iList = 0; iList < clothList.Count; iList++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (clothList[iList].Name, GUILayout.Width(200))) {
					SelectRecord (clothList[iList].ID);
				}

				//Show the name of the body structure
				string bodyName = bodyStrucTable.GetRecord(clothList[iList].IDBodyStructure).Name;

				GUILayout.Label(bodyName, GUILayout.Width(100));

				if (GUILayout.Button ("-", GUILayout.Width (25))) {
					clothTable.DeleteCloth(clothList[iList].ID);
				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndScrollView ();

		}

		private void ShowDetails(){

			GUILayout.Label ("ID: " + selectedRecord.ToString ());

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Cloth name:");
			clothRecord.Name = GUILayout.TextField (clothRecord.Name, GUILayout.Width (250));
			GUILayout.EndHorizontal ();

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Body structure:");
			bodyStructIndex = EditorGUILayout.Popup (bodyStrucSelected, bodyStructOptions);
			if (bodyStructIndex != bodyStrucSelected) {
				FilterSlots (bodyStructIndex);
				bodyStrucSelected = bodyStructIndex;
			}
			GUILayout.EndHorizontal ();

			GUILayout.Space (15);

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical (EditorStyles.helpBox);

			GUILayout.Label ("Cloth Meshes", EditorStyles.boldLabel);

			if (bodyStrucSelected > -1) {

				scrollModels = GUILayout.BeginScrollView (scrollModels, GUILayout.Width (250));

				for (int iModels = 0; iModels < modelsList.Count; iModels++) {


					GUILayout.Label (modelsList [iModels].SubtypeName,EditorStyles.boldLabel);

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Prefab name:");
					modelsList [iModels].PrefabName = GUILayout.TextField (modelsList [iModels].PrefabName, GUILayout.Width (150));
					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Prefab object:");
					modelsList[iModels].Prefab = (GameObject)EditorGUILayout.ObjectField (modelsList[iModels].Prefab, typeof(GameObject), false, GUILayout.Width (150));
					if (modelsList[iModels].Prefab  != null && modelsList[iModels].Prefab != modelsList[iModels].PrefabSelected) {
						modelsList[iModels].PrefabName = _config.GetPrefabPath (CSConfig.PathTypeEnum.ClothPath, AssetDatabase.GetAssetPath (modelsList[iModels].Prefab ));
						modelsList[iModels].PrefabSelected = modelsList[iModels].Prefab ;
						modelsList[iModels].PrefabError  = false;
					}
					GUILayout.EndHorizontal ();
					if (modelsList[iModels].PrefabError) EditorGUILayout.HelpBox ("Prefab cannot be found. Load it again to update the [Prefab name] field.", MessageType.Error);

					GUILayout.Space (10);
				}

				GUILayout.EndScrollView ();

			}
			GUILayout.EndVertical ();

			GUILayout.Space (15);

			GUILayout.BeginVertical (EditorStyles.helpBox);

			GUILayout.Label ("Slots occupied by the cloth",EditorStyles.boldLabel);

			if (bodyStrucSelected > -1) {

				scrollSlots = GUILayout.BeginScrollView (scrollSlots, GUILayout.Width (150));

				for (int iSlots = 0; iSlots < bodyStrucSlots.Count; iSlots++) {
					selectedSlots [iSlots] = GUILayout.Toggle (selectedSlots [iSlots], bodyStrucSlots [iSlots].SlotName);
				}

				GUILayout.EndScrollView ();

			}

			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

			GUILayout.Space (15);

			if (GUILayout.Button ("Save Cloth")) {
				SaveRecord ();
			}
		}

		private void NewRecord(){

			clothRecord = new ClothTable.Cloth();

			flagEdit = 1;
			selectedRecord = 0;
			bodyStrucSelected = -1;
			bodyStructIndex = 0;

		}

		private void SelectRecord(int id){

			selectedRecord = id;

			clothRecord = clothTable.GetRecord (id);

			bodyStrucSelected = bodyStrucList.FindIndex(x => x.ID == clothRecord.IDBodyStructure);

			FilterSlots (bodyStrucSelected);

			//Update selected slots array
			List<ClothTable.ClothSlots> tList = clothTable.GetSlotsList(selectedRecord);
			int index = 0;

			for (int i = 0; i < tList.Count; i++) {
				index = bodyStrucSlots.FindIndex (x => x.IDSlot == tList [i].ClothSlot);
				if (index > -1)
					selectedSlots [index] = true;
			}

			//Update models list
			List<ClothTable.ClothModels> mList = clothTable.GetModelsList(selectedRecord);

			for (int i = 0; i < mList.Count; i++) {
				ModelPrefab tRec = modelsList.Find (x => x.IDSubtype == mList [i].IDBodySubtype);
				tRec.PrefabName = mList [i].ModelName;

				//Try to find the object
				GameObject prefabToLoad = (GameObject)Resources.Load(_config.GetPathName(CSConfig.PathTypeEnum.ClothPath)+"/"+tRec.PrefabName);
				if (prefabToLoad != null) {
					tRec.Prefab = prefabToLoad;
					tRec.PrefabSelected = prefabToLoad;
				} else {
					tRec.PrefabError = true;
				}

			}

			flagEdit = 2;

		}

		private void SaveRecord(){

			Debug.Log ("SaveRecord");

			int idBodyStruc = bodyStrucList [bodyStrucSelected].ID;

			if (flagEdit == 1){
				selectedRecord = clothTable.AddCloth (clothRecord.Name, idBodyStruc);
			} else {
				clothTable.UpdateCloth (clothRecord.Name, idBodyStruc, selectedRecord);
			}

			//Update models list
			clothTable.DeleteClothModels(selectedRecord);
			for (int i = 0; i < modelsList.Count; i++) {
				if (modelsList[i].PrefabName.Length > 0)
				clothTable.AddModel (modelsList [i].IDSubtype, selectedRecord, modelsList [i].PrefabName);
			}

			//Update slots list
			List<ClothTable.ClothSlots> tList = new List<ClothTable.ClothSlots>();

			for (int i = 0; i < selectedSlots.Length; i++) {
				if (selectedSlots [i]) {
					ClothTable.ClothSlots toAdd = new ClothTable.ClothSlots ();
					toAdd.ClothID = selectedRecord;
					toAdd.ClothSlot = bodyStrucSlots [i].IDSlot;
					tList.Add (toAdd);
				}
			}
			clothTable.UpdateSlotList (tList, selectedRecord);

			SelectRecord (selectedRecord);

			clothList = clothTable.GetList ();


		}

		private void FilterSlots(int indexStruc){

			//Get the cloth slots from the selected body structure
			int idStruc = bodyStrucList [indexStruc].ID;

			bodyStrucSlots = bodyStrucTable.GetClothSlotList (idStruc);

			selectedSlots = new bool[bodyStrucSlots.Count];

			//Get the body subtypes from the selected structure
			modelsList.Clear();

			List<BodySubtypeTable.BodySubtype> tList = bodySubType.GetList ();
			BodyStructureTable.BodyStructure tRecord = bodyStrucTable.GetRecord (idStruc);

			for (int i = 0; i < tRecord.IDBodySubtype.Count; i++) {
				ModelPrefab toAdd = new ModelPrefab ();
				toAdd.IDSubtype = tRecord.IDBodySubtype [i];
				toAdd.SubtypeName = tList.Find (x => x.ID == tRecord.IDBodySubtype [i]).Name;

				modelsList.Add (toAdd);
			}

		}

		private void FilterList(bool filter){

			if (filter) {
				clothList = clothTable.GetList ().FindAll(x => x.Name.ToLower().Contains(filterList.ToLower()));
			} else {
				clothList = clothTable.GetList ();
				filterList = "";
			}
		}

	}
}