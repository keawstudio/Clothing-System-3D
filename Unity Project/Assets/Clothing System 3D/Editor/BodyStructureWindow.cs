using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ClothingSystem3D {

	public class BodyStructureWindow : EditorWindow {

		public static BodyStructureWindow instance;

		int tabSelected = 0;
		int recordSelected = 0;
		int bodyTypeSelected = 0;
		int flagEdit = 0;	//1 - add, 2 - edit

		//Vectors used in the Scrollview
		Vector2 scrollStructures;
		Vector2 scrollClothes;
		Vector2 scrollProps;
		Vector2 scrollSkins;

		//Classes
		BodyStructureTable bodyStrucTable = new BodyStructureTable();
		BodyTypeTable bodyTypesTable = new BodyTypeTable();
		BodySubtypeTable bodySubtypesTable = new BodySubtypeTable();

		//Arrays to store options
		string[] bodyTypesOptions = new string[]{""};
		string[] tabs = new string[]{"Clothes Slots","Props Slots","Skins Areas"};
		bool[] bolSubTypes = new bool[]{false};

		//Records and lists
		BodyStructureTable.BodyStructure bStrucRec = new BodyStructureTable.BodyStructure();
		List<BodyStructureTable.BodyStructure> listStrucs = new List<BodyStructureTable.BodyStructure>();

		List<BodySubtypeTable.BodySubtype> bodySubtypes = new List<BodySubtypeTable.BodySubtype>();
		List<BodyTypeTable.BodyType> bodyTypes = new List<BodyTypeTable.BodyType> ();

		List<BodyStructureTable.BodyClothSlots> clothSlots = new List<BodyStructureTable.BodyClothSlots>();
		List<BodyStructureTable.BodyPropsSlots> propSlots = new List<BodyStructureTable.BodyPropsSlots> ();
		List<BodyStructureTable.BodySkinsSlots> skinSlots = new List<BodyStructureTable.BodySkinsSlots>();

		//Strings for new records
		string sClothSlot = "";
		string sClothObject = "";
		string sPropSlot = "";
		string sPropObject = "";
		string sSkinSlot = "";
		string sSkinObject = "";

		//Show the window
		public static void ShowBodyStructureWindow(){

			instance = (BodyStructureWindow)EditorWindow.GetWindow (typeof(BodyStructureWindow));
			instance.titleContent = new GUIContent ("Body Structure");

		}


		void OnEnable(){

			//return;

			//Load body type options
			bodyTypesTable.LoadTable ();
			bodyTypes = bodyTypesTable.GetList ();

			System.Array.Resize (ref bodyTypesOptions, bodyTypes.Count);

			for (int i = 0; i < bodyTypes.Count; i++)
				bodyTypesOptions [i] = bodyTypes[i].Name;

			//Load body subtypes table
			bodySubtypesTable.LoadTable ();
			bodySubtypes = bodySubtypesTable.GetList ();
			System.Array.Resize (ref bolSubTypes, bodySubtypes.Count);

			bodyStrucTable.LoadStructures ();
			bodyStrucTable.LoadClothSlots ();
			bodyStrucTable.LoadPropSlots ();
			bodyStrucTable.LoadSkinSlots ();

			listStrucs = bodyStrucTable.GetList ();

		}

		void OnDestroy(){
			
			if (EditorUtility.DisplayDialog ("Save data", "Want to save the table?", "Yes", "No")) {
				bodyStrucTable.SaveTable ();

				bodyStrucTable.UnloadStructures();
				bodyStrucTable.UnloadClothSlots();
				bodyStrucTable.UnloadPropSlots();
				bodyStrucTable.UnloadSkinSlots();
				bodyTypesTable.UnloadTable();
				bodySubtypesTable.UnloadTable();

			}

		}

		void OnGUI(){

			//return;

			ShowMainWindow ();

		}

		void ShowMainWindow(){

			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("List of Body Structures",EditorStyles.boldLabel);

			if (GUILayout.Button("Add Structure", GUILayout.Width(150))){
				//Start the add for new structure
				NewRecord();
			}

			ShowList ();

			GUILayout.Space (15);

			EditorGUILayout.HelpBox ("Save the table to avoid losing data.", MessageType.Warning);

			if (GUILayout.Button("Save table", GUILayout.Width(150))){
				//Start the add for new structure
				bodyStrucTable.SaveTable();
			}

			GUILayout.EndVertical ();

			GUILayout.BeginVertical (EditorStyles.helpBox);
			GUILayout.Label ("Details of Body Structure", EditorStyles.boldLabel, GUILayout.Width(300));

			GUILayout.Space (25);

			if (flagEdit > 0)
				ShowDetails ();

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();

		}

		void SelectRecord(int ID){

			//Select structure record
			recordSelected = ID;
			bStrucRec = bodyStrucTable.GetRecord (ID);

			//Body type
			bodyTypeSelected = bodyTypes.FindIndex (x => x.ID == bStrucRec.IDBodyType);

			//Body subtypes
			for (int i = 0; i < bolSubTypes.Length; i++)
				bolSubTypes [i] = false;
			
			int index = 0;

			for (int i = 0; i < bStrucRec.IDBodySubtype.Count; i++) {
				index = bodySubtypes.FindIndex (x => x.ID == bStrucRec.IDBodySubtype [i]);
				if (index > -1)
					bolSubTypes [index] = true;
			}

			//Load slots
			clothSlots = bodyStrucTable.GetClothSlotList(recordSelected);
			propSlots = bodyStrucTable.GetPropSlotList (recordSelected);
			skinSlots = bodyStrucTable.GetSkinSlotList (recordSelected);

			flagEdit = 2;

		}

		void NewRecord(){


			bStrucRec = new BodyStructureTable.BodyStructure ();

			propSlots = new List<BodyStructureTable.BodyPropsSlots> ();
			skinSlots = new List<BodyStructureTable.BodySkinsSlots> ();
			clothSlots = new List<BodyStructureTable.BodyClothSlots> ();

			for (int i = 0; i < bolSubTypes.Length; i++)
				bolSubTypes [i] = false;
			
			recordSelected = 0;
			bodyTypeSelected = 0;

			flagEdit = 1;

		}

		void SaveRecord(){

			//Get the ID of body type
			int ibodyType = bodyTypes[bodyTypeSelected].ID;
			bStrucRec.IDBodyType = ibodyType;

			bStrucRec.IDBodySubtype.Clear ();

			//Get ids of the subtypes selected
			for (int i = 0; i < bolSubTypes.Length; i++) {

				if (bolSubTypes [i]) {
					bStrucRec.IDBodySubtype.Add (bodySubtypes [i].ID);
				}
			}

			if (flagEdit == 1) {
				recordSelected = bodyStrucTable.AddStructure (bStrucRec);
			} else {
				bodyStrucTable.UpdateClothSlots (recordSelected, clothSlots);
				bodyStrucTable.UpdatePropSlots (recordSelected, propSlots);
				bodyStrucTable.UpdateSkinSlots (recordSelected, skinSlots);
			}

			flagEdit = 2;
			listStrucs = bodyStrucTable.GetList ();

			SelectRecord (recordSelected);

		}

		void ShowDetails(){

			GUILayout.Label ("ID: " + recordSelected.ToString ());

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Structure name:");
			bStrucRec.Name = GUILayout.TextField (bStrucRec.Name, GUILayout.Width (250));
			GUILayout.EndHorizontal ();

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Body type:");
			bodyTypeSelected = EditorGUILayout.Popup (bodyTypeSelected, bodyTypesOptions);
			GUILayout.EndHorizontal ();

			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			bStrucRec.ClothingMethod = (CSConfig.ClothingMethodEnum)EditorGUILayout.EnumPopup ("Clothing Method:", bStrucRec.ClothingMethod);
			GUILayout.EndHorizontal ();

			GUILayout.Space (5);

			GUILayout.Label ("Available subtypes:");
			for (int i = 0; i < bolSubTypes.Length; i++) {
				bolSubTypes[i] = GUILayout.Toggle (bolSubTypes[i], bodySubtypes[i].Name);
			}

			GUILayout.Space (15);


			if (recordSelected > 0) {

				tabSelected = GUILayout.Toolbar (tabSelected, tabs);

				switch (tabSelected) {
				case 0:
					ShowClothesPanel ();
					break;
				case 1:
					ShowPropsPanel ();
					break;
				case 2:
					ShowSkinsPanel ();
					break;
				}
			}
			GUILayout.Space (15);

			if (GUILayout.Button ("Save structure")) {
				SaveRecord ();
			}
		}

		void ShowList(){

			//return;

			scrollStructures = GUILayout.BeginScrollView (scrollStructures, GUILayout.Width (200));

			for (int iList = 0; iList < listStrucs.Count; iList++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (listStrucs[iList].Name)) {
					SelectRecord (listStrucs[iList].ID);
				}
				if (GUILayout.Button ("-", GUILayout.Width (25))) {
					bodyStrucTable.DeleteStructure (listStrucs [iList].ID);
				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndScrollView ();

		}

		void ShowClothesPanel(){

			scrollClothes = EditorGUILayout.BeginScrollView (scrollClothes);

			for (int iClothes = 0; iClothes < clothSlots.Count; iClothes++) {
				GUILayout.BeginHorizontal ();

				GUILayout.Label ("Slot name:");
				clothSlots [iClothes].SlotName = GUILayout.TextField (clothSlots [iClothes].SlotName, GUILayout.Width (150));

				GUILayout.Label ("Object part name:");
				clothSlots [iClothes].ObjectName = GUILayout.TextField (clothSlots [iClothes].ObjectName, GUILayout.Width (150));

				if (GUILayout.Button ("-", GUILayout.Width(25))) {
					bodyStrucTable.DeleteClothSlot (clothSlots [iClothes].IDSlot);
					clothSlots = bodyStrucTable.GetClothSlotList (recordSelected, false);
				}

				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.EndScrollView ();

			//Fields for new record
			GUILayout.BeginHorizontal ();

			GUILayout.Label ("Slot name:");
			sClothSlot = GUILayout.TextField (sClothSlot, GUILayout.Width (150));

			GUILayout.Label ("Object part name:");
			sClothObject = GUILayout.TextField (sClothObject, GUILayout.Width (150));

			if (GUILayout.Button ("+", GUILayout.Width(25))) {
				bodyStrucTable.AddClothSlot (recordSelected, sClothSlot, sClothObject);
				sClothSlot = "";
				sClothObject = "";
				clothSlots = bodyStrucTable.GetClothSlotList (recordSelected, false);
			}

			GUILayout.EndHorizontal ();

		}

		void ShowPropsPanel(){

			scrollProps = EditorGUILayout.BeginScrollView (scrollProps);

			for (int iProps = 0; iProps < propSlots.Count; iProps++) {
				GUILayout.BeginHorizontal ();

				GUILayout.Label ("Slot name:");
				propSlots [iProps].SlotName = GUILayout.TextField (propSlots [iProps].SlotName, GUILayout.Width (150));

				GUILayout.Label ("Object part name:");
				propSlots [iProps].ObjectName = GUILayout.TextField (propSlots [iProps].ObjectName, GUILayout.Width (150));

				if (GUILayout.Button ("-", GUILayout.Width(25))) {
					bodyStrucTable.DeletePropSlot (propSlots [iProps].IDSlot);
					propSlots = bodyStrucTable.GetPropSlotList (recordSelected, false);
				}

				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.EndScrollView ();

			//Fields for new record
			GUILayout.BeginHorizontal ();

			GUILayout.Label ("Slot name:");
			sPropSlot = GUILayout.TextField (sPropSlot, GUILayout.Width (150));

			GUILayout.Label ("Object part name:");
			sPropObject = GUILayout.TextField (sPropObject, GUILayout.Width (150));

			if (GUILayout.Button ("+", GUILayout.Width(25))) {
				bodyStrucTable.AddPropSlot (recordSelected, sPropSlot, sPropObject);
				sPropSlot = "";
				sPropObject = "";
				propSlots = bodyStrucTable.GetPropSlotList (recordSelected, false);
			}

			GUILayout.EndHorizontal ();

		}

		void ShowSkinsPanel(){

			scrollSkins = EditorGUILayout.BeginScrollView (scrollSkins);

			for (int iSkins = 0; iSkins < skinSlots.Count; iSkins++) {
				GUILayout.BeginHorizontal ();

				GUILayout.Label ("Area name:");
				skinSlots [iSkins].SlotName = GUILayout.TextField (skinSlots [iSkins].SlotName, GUILayout.Width (150));

				GUILayout.Label ("Object part name:");
				skinSlots [iSkins].ObjectName = GUILayout.TextField (skinSlots [iSkins].ObjectName, GUILayout.Width (150));

				if (GUILayout.Button ("-", GUILayout.Width(25))) {
					bodyStrucTable.DeleteSkinSlot (skinSlots [iSkins].IDSlot);
					skinSlots = bodyStrucTable.GetSkinSlotList (recordSelected, false);
				}

				GUILayout.EndHorizontal ();
			}

			EditorGUILayout.EndScrollView ();

			//Fields for new record
			GUILayout.BeginHorizontal ();

			GUILayout.Label ("Area name:");
			sSkinSlot = GUILayout.TextField (sSkinSlot, GUILayout.Width (150));

			GUILayout.Label ("Object part name:");
			sSkinObject = GUILayout.TextField (sSkinObject, GUILayout.Width (150));

			if (GUILayout.Button ("+", GUILayout.Width(25))) {
				bodyStrucTable.AddSkinSlot (recordSelected, sSkinSlot, sSkinObject);
				sSkinSlot = "";
				sSkinObject = "";
				skinSlots = bodyStrucTable.GetSkinSlotList (recordSelected, false);
			}

			GUILayout.EndHorizontal ();

		}
	}
}