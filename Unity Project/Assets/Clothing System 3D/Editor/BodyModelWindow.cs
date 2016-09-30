using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class BodyModelWindow : EditorWindow {

		public static BodyModelWindow instance;

		List<BodyModelTable.BodyModel> bodyModelList = new List<BodyModelTable.BodyModel>();
		List<BodyStructureTable.BodyStructure> bodyStrucList = new List<BodyStructureTable.BodyStructure>();
		List<BodySubtypeTable.BodySubtype> bodySubtList = new List<BodySubtypeTable.BodySubtype> ();
		List<EyesTable.Eye> eyesList = new List<EyesTable.Eye> ();

		BodyModelTable.BodyModel record = new BodyModelTable.BodyModel ();

		BodyModelTable bodyModelTable = new BodyModelTable();
		BodyStructureTable bodyStrucTable = new BodyStructureTable();
		BodySubtypeTable bodySubType = new BodySubtypeTable();
		EyesTable eyeTable = new EyesTable();

		Vector2 scrollPos;

		int flagEdit = 0;
		int bodyStrucSelected = -1;
		int bodyStructIndex = 0;
		int bodySubtypeSelected = 0;
		int eyeSelected = -1;

		string[] bodyStructOptions = new string[]{""};
		string[] bodySubtypeOptions = new string[]{""};
		string[] optEyes = new string[]{"-- No eyes --"};

		GameObject prefab;
		GameObject prefabSelected;
		bool prefabError;

		class ListModels {
			public int IDModel = 0;
			public string Name = "";
			public string Structure = "";
			public string BodySubtype = "";
			public bool PrefabError = false;
		}

		List<ListModels> modelsList = new List<ListModels>();

		private CSConfig _config;

		public static void ShowBodyModelWindow(){

			instance = (BodyModelWindow)EditorWindow.GetWindow (typeof(BodyModelWindow));
			instance.titleContent = new GUIContent ("Body Model List");
		}

		private void OnEnable(){

			_config = new CSConfig ();

			if (_config == null) Debug.LogError("CSConfig cannot be loaded. This script will not run properly");

			bodyStrucTable.LoadStructures ();
			bodyStrucTable.LoadSkinSlots ();
			bodySubType.LoadTable ();

			bodyStrucList = bodyStrucTable.GetList();

			System.Array.Resize (ref bodyStructOptions, bodyStrucList.Count);

			for (int i = 0; i < bodyStrucList.Count; i++) {
				bodyStructOptions [i] = bodyStrucList[i].Name;
			}

			eyeTable.LoadTable ();
			eyesList = eyeTable.GetList ();

			System.Array.Resize (ref optEyes, eyesList.Count + 1);

			for (int i = 1; i <= eyesList.Count; i++) {
				optEyes [i] = eyesList [i - 1].Name;
			}

			bodySubType.LoadTable ();
			bodySubtList = bodySubType.GetList ();

			bodyModelTable.LoadList();
			GetModelsList ();

		}

		private void OnDestroy(){

			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				bodyModelTable.SaveList();

				bodyModelTable.UnloadList ();
				bodyStrucTable.UnloadStructures ();
				bodyStrucTable.UnloadSkinSlots ();
				bodySubType.UnloadTable ();
				eyeTable.UnloadTable ();
			}

		}

		private void OnGUI(){

			ShowItems ();

		}

		private void ShowItems(){

			int contItems = 0;

			GUILayout.Label ("Table of Body Models", EditorStyles.boldLabel);

			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();

			GUILayout.Label ("", GUILayout.Width (25));
			GUILayout.Label ("Model name", GUILayout.Width (200));
			GUILayout.Label ("Body Structure", GUILayout.Width (150));
			GUILayout.Label ("Body Subtype", GUILayout.Width (150));
			GUILayout.Label ("Prefab status", GUILayout.Width (100));


			GUILayout.EndHorizontal ();

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);

			for (int iLoopItems = 0; iLoopItems < modelsList.Count; iLoopItems++) {

				EditorGUILayout.BeginHorizontal ();

				GUILayout.Label (modelsList[iLoopItems].IDModel.ToString(), GUILayout.Width (25));

				if (GUILayout.Button (modelsList[iLoopItems].Name, GUILayout.Width (200))) {
					SelectRecord (modelsList[iLoopItems].IDModel);
				}

				GUILayout.Label (modelsList [iLoopItems].Structure, GUILayout.Width (150));
				GUILayout.Label (modelsList [iLoopItems].BodySubtype, GUILayout.Width (150));

				Color oldColor = GUI.contentColor;

				if (modelsList [iLoopItems].PrefabError) {
					GUI.contentColor = Color.red;
					GUILayout.Label ("Not found",GUILayout.Width (100));
				} else {
					GUI.contentColor = Color.blue;
					GUILayout.Label ("OK",GUILayout.Width (100));
				}

				GUI.contentColor = oldColor;

				if (GUILayout.Button("-",GUILayout.Width(25))){
					int canDelete = bodyModelTable.DeleteModel(bodyModelList [iLoopItems].ID);
					if (canDelete > 0) this.ShowNotification(new GUIContent(EditorGUILayout.TextField("Cannot delete model. There is a skin attached to it.")));
				}

				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (5);

				contItems++;
			}

			EditorGUILayout.EndScrollView ();

			if (contItems == 0)
				EditorGUILayout.HelpBox ("This table is empty", MessageType.Info);

			GUILayout.Space (15);


			if (flagEdit > 0) {
				GUILayout.BeginVertical (EditorStyles.helpBox);

				GUILayout.BeginHorizontal ();

				GUILayout.Label ("Model name:", GUILayout.Width(150));
				record.Name = GUILayout.TextField(record.Name,GUILayout.Width(250));

				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();

				GUILayout.Label ("Model prefab:", GUILayout.Width(150));
				record.ModelName = GUILayout.TextField (record.ModelName, GUILayout.Width (150));
				prefab = (GameObject)EditorGUILayout.ObjectField (prefab, typeof(GameObject), false, GUILayout.Width (150));
				if (prefab  != null && prefab != prefabSelected) {
					record.ModelName = _config.GetPrefabPath (CSConfig.PathTypeEnum.BodyModelsPath, AssetDatabase.GetAssetPath (prefab ));
					prefabSelected = prefab ;
					prefabError  = false;
				}
				GUILayout.EndHorizontal ();
				if (prefabError) EditorGUILayout.HelpBox ("Prefab cannot be found. Load it again to update the [Prefab name] field.", MessageType.Error);

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Bone Structure name:", GUILayout.Width (150));
				record.ArmatureName = GUILayout.TextField (record.ArmatureName, GUILayout.Width (250));
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Body structure:", GUILayout.Width(150));
				bodyStructIndex = EditorGUILayout.Popup (bodyStrucSelected, bodyStructOptions,GUILayout.Width(150));
				if (bodyStructIndex != bodyStrucSelected) {
					FilterSlots (bodyStructIndex);
					bodyStrucSelected = bodyStructIndex;
				}
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Body subtype:", GUILayout.Width(150));
				bodySubtypeSelected = EditorGUILayout.Popup (bodySubtypeSelected, bodySubtypeOptions, GUILayout.Width (150));
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Eyes:", GUILayout.Width(150));
				eyeSelected = EditorGUILayout.Popup (eyeSelected, optEyes, GUILayout.Width (150));

				if (GUILayout.Button ("Save", GUILayout.Width (55))) {
					SaveRecord ();
				}
				GUILayout.EndHorizontal ();

				GUILayout.EndVertical ();
			}

			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Add model", GUILayout.Width (155))) {

				flagEdit = 1;
				record = new BodyModelTable.BodyModel ();

			}

			//Add button to save the table
			if (GUILayout.Button ("Save table",GUILayout.Width(155))) {
				bodyModelTable.SaveList();
			}

			GUILayout.EndHorizontal ();

		}

		private void GetModelsList(){

			modelsList.Clear ();
			bodyModelList = bodyModelTable.GetList (true);

			BodyStructureTable.BodyStructure searchStruct = new BodyStructureTable.BodyStructure ();
			BodySubtypeTable.BodySubtype searchSubtype = new BodySubtypeTable.BodySubtype ();

			for (int i = 0; i < bodyModelList.Count; i++) {
				ListModels toAdd = new ListModels ();
				toAdd.IDModel = bodyModelList [i].ID;
				toAdd.Name = bodyModelList [i].Name;
				searchStruct = bodyStrucList.Find (x => x.ID == bodyModelList [i].IDBodyStructure);
				if (searchStruct != null) toAdd.Structure = searchStruct.Name;

				searchSubtype = bodySubtList.Find (x => x.ID == bodyModelList [i].IDBodySubtype);
				if (searchSubtype != null)
					toAdd.BodySubtype = searchSubtype.Name;

				GameObject prefabToLoad = (GameObject)Resources.Load(_config.GetPathName(CSConfig.PathTypeEnum.BodyModelsPath)+"/"+bodyModelList[i].ModelName);
				if (prefabToLoad == null) 
					toAdd.PrefabError = true;

				modelsList.Add (toAdd);

			}

		}

		private void SaveRecord(){

			int idBodyStruc = bodyStrucList [bodyStrucSelected].ID;
			int idSubtype = bodyStrucList [bodyStrucSelected].IDBodySubtype [bodySubtypeSelected];
			int idEye = 0;

			if (eyeSelected > 0 ) idEye = eyesList [eyeSelected - 1].ID;

			record.IDBodyStructure = idBodyStruc;
			record.IDBodySubtype = idSubtype;
			record.IDEye = idEye;

			if (flagEdit == 1) {
				bodyModelTable.AddModel (record);
			} else {
				bodyModelTable.EditModel (record);
			}

			flagEdit = 0;
			GetModelsList ();

		}

		private void SelectRecord(int id){

			record = bodyModelTable.GetModel (id);

			bodyStrucSelected = bodyStrucList.FindIndex (x => x.ID == record.IDBodyStructure);

			eyeSelected = eyesList.FindIndex (x => x.ID == record.IDEye) + 1;

			FilterSlots (bodyStrucSelected);

			for (int i = 0; i < bodyStrucList [bodyStrucSelected].IDBodySubtype.Count; i++) {
				if (bodyStrucList [bodyStrucSelected].IDBodySubtype [i] == record.IDBodySubtype)
					bodySubtypeSelected = i;
			}

			GameObject prefabToLoad = (GameObject)Resources.Load(_config.GetPathName(CSConfig.PathTypeEnum.BodyModelsPath)+"/"+record.ModelName);
			if (prefabToLoad != null) {
				prefab = prefabToLoad;
				prefabSelected = prefabToLoad;
				prefabError = false;
			} else {
				prefabError = true;
			}

			flagEdit = 2;

		}

		private void FilterSlots(int indexStruc){

			//Filter available body subtypes
			int totalSub = bodyStrucList [indexStruc].IDBodySubtype.Count;
			bodySubtypeOptions = new string[totalSub];

			int idStruc = bodyStrucList [indexStruc].ID;

			for (int i = 0; i < totalSub; i++) {
				bodySubtypeOptions [i] = bodySubtList.Find (x => x.ID == bodyStrucList [indexStruc].IDBodySubtype [i]).Name;

			}
		}
	}
}
