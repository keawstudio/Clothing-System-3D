using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class PropsWindow : EditorWindow {

		public static PropsWindow instance;

		int flagEdit = 0;
		int selectedRecord = 0;
		int bodyStrucSelected = -1;
		int bodyStructIndex = 0;
		int selectedSlot = 0;

		string filterList = "";

		Vector2 scrollProps;
		Vector2 scrollSlots;
		Vector2 scrollModels;

		//Tables
		PropTable propTable = new PropTable();
		BodyStructureTable bodyStrucTable = new BodyStructureTable();
		BodySubtypeTable bodySubType = new BodySubtypeTable();

		//Records and lists
		PropTable.Prop propRecord = new PropTable.Prop();

		List<PropTable.Prop> propList = new List<PropTable.Prop>();
		List<BodyStructureTable.BodyPropsSlots> bodyStrucSlots = new List<BodyStructureTable.BodyPropsSlots>();
		List<BodyStructureTable.BodyStructure> bodyStrucList = new List<BodyStructureTable.BodyStructure>();
		List<PropTable.PropModels> propModels = new List<PropTable.PropModels>();

		string[] bodyStructOptions = new string[]{""};
		string[] propSlotsOptions = new string[]{ "" };

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
		public static void ShowPropsWindow(){

			instance = (PropsWindow)EditorWindow.GetWindow (typeof(PropsWindow));
			instance.titleContent = new GUIContent ("Props List");

		}

		private void OnEnable(){

			_config = new CSConfig ();

			if (_config == null) Debug.LogError("CSConfig cannot be loaded. This script will not run properly");

			bodyStrucTable.LoadStructures ();
			bodyStrucTable.LoadPropSlots ();
			bodySubType.LoadTable ();

			bodyStrucList = bodyStrucTable.GetList();

			System.Array.Resize (ref bodyStructOptions, bodyStrucList.Count);

			for (int i = 0; i < bodyStrucList.Count; i++) {
				bodyStructOptions [i] = bodyStrucList[i].Name;
			}

			propTable.LoadPropList ();
			propTable.LoadModelsList ();

			propList = propTable.GetList ();


		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				propTable.SaveTable ();

				propTable.UnloadPropList();
				propTable.UnloadModelsList ();
				bodyStrucTable.UnloadStructures();
				bodyStrucTable.UnloadPropSlots();
				bodySubType.UnloadTable();

			}

		}

		private void OnGUI(){

			ShowItems ();

		}

		private void ShowItems(){

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("List of Props",EditorStyles.boldLabel);

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

			if (GUILayout.Button("Add Prop", GUILayout.Width(150))){
				//Start the add for new structure
				NewRecord();
			}

			ShowList ();

			GUILayout.Space (15);

			EditorGUILayout.HelpBox ("Save the table to avoid losing data.", MessageType.Warning);

			if (GUILayout.Button("Save table", GUILayout.Width(150))){
				//Start the add for new structure
				propTable.SaveTable();
			}

			GUILayout.EndVertical ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("Details of Prop", EditorStyles.boldLabel, GUILayout.Width(300));

			GUILayout.Space (15);

			if (flagEdit > 0)
				ShowDetails ();

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();

		}

		private void ShowList(){

			scrollProps = GUILayout.BeginScrollView (scrollProps, GUILayout.Width (300));

			for (int iList = 0; iList < propList.Count; iList++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (propList[iList].Name, GUILayout.Width(200))) {
					SelectRecord (propList[iList].ID);
				}
				if (GUILayout.Button ("-", GUILayout.Width (25))) {
					propTable.DeleteProp(propList[iList].ID);
				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndScrollView ();

		}

		private void ShowDetails(){

			GUILayout.Label ("ID: " + selectedRecord.ToString ());

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Prop name:");
			propRecord.Name = GUILayout.TextField (propRecord.Name, GUILayout.Width (250));
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

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Slot:");
			selectedSlot = EditorGUILayout.Popup (selectedSlot, propSlotsOptions);
			GUILayout.EndHorizontal ();

			GUILayout.Space (15);

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical (EditorStyles.helpBox);

			GUILayout.Label ("Prop Meshes", EditorStyles.boldLabel);

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
						modelsList[iModels].PrefabName = _config.GetPrefabPath (CSConfig.PathTypeEnum.PropPath, AssetDatabase.GetAssetPath (modelsList[iModels].Prefab ));
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

			GUILayout.EndHorizontal ();

			GUILayout.Space (15);

			if (GUILayout.Button ("Save Prop")) {
				SaveRecord ();
			}
		}

		private void NewRecord(){

			propRecord = new PropTable.Prop();

			flagEdit = 1;
			selectedRecord = 0;
			bodyStrucSelected = -1;
			bodyStructIndex = 0;

		}

		private void SelectRecord(int id){

			selectedRecord = id;

			propRecord = propTable.GetRecord (id);

			bodyStrucSelected = bodyStrucList.FindIndex(x => x.ID == propRecord.IDBodyStructure);

			FilterSlots (bodyStrucSelected);

			selectedSlot = bodyStrucSlots.FindIndex (x => x.IDSlot == propRecord.IDSlot);

			//Update models list
			List<PropTable.PropModels> mList = propTable.GetModelsList(selectedRecord);

			for (int i = 0; i < mList.Count; i++) {
				ModelPrefab tRec = modelsList.Find (x => x.IDSubtype == mList [i].IDBodySubtype);
				tRec.PrefabName = mList [i].ModelName;

				//Try to find the object
				GameObject prefabToLoad = (GameObject)Resources.Load(_config.GetPathName(CSConfig.PathTypeEnum.PropPath)+"/"+tRec.PrefabName);
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

			propRecord.IDBodyStructure = bodyStrucList [bodyStrucSelected].ID;

			propRecord.IDSlot = bodyStrucSlots [selectedSlot].IDSlot;

			if (flagEdit == 1){
				selectedRecord = propTable.AddProp (propRecord);
			} else {
				propTable.UpdateProp (propRecord);
			}

			//Update models list
			propTable.DeletePropModels(selectedRecord);
			for (int i = 0; i < modelsList.Count; i++) {
				if (modelsList[i].PrefabName.Length > 0)
					propTable.AddModel (modelsList [i].IDSubtype, selectedRecord, modelsList [i].PrefabName);
			}

			SelectRecord (selectedRecord);

			propList = propTable.GetList ();


		}

		private void FilterSlots(int indexStruc){

			//Get the cloth slots from the selected body structure
			int idStruc = bodyStrucList [indexStruc].ID;

			bodyStrucSlots = bodyStrucTable.GetPropSlotList (idStruc);

			System.Array.Resize (ref propSlotsOptions, bodyStrucSlots.Count);

			for (int i = 0; i < bodyStrucSlots.Count; i++) {
				propSlotsOptions [i] = bodyStrucSlots [i].SlotName;
			}
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
				propList = propTable.GetList ().FindAll(x => x.Name.ToLower().Contains(filterList.ToLower()));
			} else {
				propList = propTable.GetList ();
				filterList = "";
			}
		}

	}
}