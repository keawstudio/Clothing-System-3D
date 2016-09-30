// ********************************
//  ActorScript
//
//  Controls the customization of the actor.
//  See the documentation for information on 
//  how it works.
//
//  Clothing System 3D
//  2016 - Larissa Redeker
//  Kittens and Elves at Work
//  http://www.keawstudio.com
//
// ********************************

using UnityEngine;
using System.Collections.Generic;
using System;

namespace ClothingSystem3D {

	public class ActorScript : MonoBehaviour {

		public class ActorBody {
			public int BodyID;				//ID of the body model in use
			public int BodyStructure;		//ID of body structure
			public int BodySubtype;			//ID of body subtype
			public string BodyModel;		//Name of body prefab
			public GameObject ActorModel;	//Body prefab
			public int EyeID;				//ID of eye model
			public CSConfig.ClothingMethodEnum ClothingType;
			public int SkinID;				//ID of actual skin
			public Transform BonesArmature;	//Skeleton object
		}

		public class ActorCloth	{
			public int ClothID = 0;
			public List<int> ClothSlots = new List<int> ();
		}

		public class ActorProp {
			public int IDProp = 0;
			public int PropSlot = 0;
		}

		private CSConfig _config;

		//Object from where the bones structure will be extracted
		public GameObject bonesSource;

		[Tooltip("If the body model was changed, seek for the cloth's model for the body model?")]
		public bool AutoChangeCloth = true;

		//Main actor properties
		public ActorBody actorBody = new ActorBody ();

		//List of clothes the actor is wearing
		public List<ActorCloth> clothesList = new List<ActorCloth>();

		//List of props the actor is using
		public List<ActorProp> propsList = new List<ActorProp>();

		int oldBodySubtype = 0;

		//Table classes
		BodyModelTable tableBodyModel = new BodyModelTable();
		BodyStructureTable tableBodyStructure = new BodyStructureTable();
		BodySubtypeTable tableBodySubtype = new BodySubtypeTable();
		BodyTypeTable tableBodyType = new BodyTypeTable();
		ClothTable tableCloth = new ClothTable();
		EyeSkinTable tableEyeSkin = new EyeSkinTable();
		EyesTable tableEyes = new EyesTable();
		PropTable tableProps = new PropTable();
		SkinsTable tableSkins = new SkinsTable();


		void Start () {
	
			_config = new CSConfig ();

			if (_config == null) Debug.LogError("CSConfig cannot be loaded. This script will not run properly");

			LoadTables ();

		}

		//** LoadTables
		//Pre-load all tables that will be necessary to work
		public void LoadTables(){

			tableBodyModel.LoadList();

			tableBodyStructure.LoadStructures();
			tableBodyStructure.LoadSkinSlots();
			tableBodyStructure.LoadClothSlots();
			tableBodyStructure.LoadPropSlots();

			tableCloth.LoadSlotsList();
			tableCloth.LoadModelsList();

			tableEyeSkin.LoadMaterialList();

			tableEyes.LoadObjectList();

			tableProps.LoadModelsList();
			tableProps.LoadPropList();

			tableSkins.LoadSlotsList();
			tableSkins.LoadSkinsList();

		}

	
		//** UnloadTables
		//Unload all loaded tables
		public void UnloadTables(){

			tableBodyModel.UnloadList();

			tableBodyStructure.UnloadStructures();
			tableBodyStructure.UnloadSkinSlots();
			tableBodyStructure.UnloadClothSlots();
			tableBodyStructure.UnloadPropSlots();

			tableCloth.UnloadSlotsList();
			tableCloth.UnloadModelsList();

			tableEyeSkin.UnloadMaterialList();

			tableEyes.UnloadObjectList();

			tableProps.UnloadModelsList();
			tableProps.UnloadPropList();

			tableSkins.UnloadSlotsList();
			tableSkins.UnloadSkinsList();

		}


		//** ApplyBodyModel **
		//Change the actual model used by the actor
		public void ApplyBodyModel(int modelID){

			if (_config == null)
				return;

			//Get the name of the prefab to be used
			BodyModelTable.BodyModel model = tableBodyModel.GetModel(modelID);

			if (model == null){
				Debug.LogError ("Cannot retrieve the prefab of Body Model.");
			}

			//Assembly the full name with the path where the models are stored
			string modelPath = _config.GetPathName(CSConfig.PathTypeEnum.BodyModelsPath)+"/"+model.ModelName;

			//Load the model prefab
			GameObject objModel = (GameObject)Resources.Load(modelPath);

			if (objModel == null) {
				//Cannot load the model. Don't continue
				Debug.LogError ("Cannot retrieve the prefab of Body Model. Prefab not found.");
			}

			//Retrieve the bone estructure that need to be used
			SkinnedMeshRenderer bSource = bonesSource.GetComponent<SkinnedMeshRenderer> ();

			//Remove actual model from actor
			if (actorBody.ActorModel != null) Destroy(actorBody.ActorModel);

			//Assign the new model to actor controller
			actorBody.ActorModel = GameObject.Instantiate(objModel) as GameObject;
			actorBody.ActorModel.transform.SetParent (this.gameObject.transform,false);

			//For each sub object of the model set the bone structure to be used
			foreach (Transform child in actorBody.ActorModel.transform){
				SkinnedMeshRenderer bDest = child.GetComponent<SkinnedMeshRenderer> ();
				bDest.bones = bSource.bones;
			}

			oldBodySubtype = actorBody.BodySubtype;

			//Update information about the actor's body model
			actorBody.BodyID = modelID;
			actorBody.BodyStructure = model.IDBodyStructure;
			actorBody.BodySubtype = model.IDBodySubtype;
			actorBody.BodyModel = modelPath;
			actorBody.EyeID = model.IDEye;
			//Find the bone structure object 
			actorBody.BonesArmature = transform.FindChild(model.ArmatureName);

			//Retrieve the clothing method used by this model / body structure
			tableBodyStructure.GetList ();

			actorBody.ClothingType = tableBodyStructure.GetRecord (model.IDBodyStructure).ClothingMethod;

			if (AutoChangeCloth)
				ChangeClothesForModel();
			
			//Apply the default skin
			SkinsTable.Skin tSkin = tableSkins.GetDefaultSkin(actorBody.BodyID);
			if (tSkin != null) {
				actorBody.SkinID = tSkin.ID;
				ApplyBodySkin (actorBody.SkinID);
			} else {
				Debug.LogWarning ("Body model doesn't have a default skin.");
			}

		}


		//** ApplyBodySkin **
		//Change the body skin
		public void ApplyBodySkin(int skinID){

			Transform objFound;

			string matName;

			List<BodyStructureTable.BodySkinsSlots> tListSlots = 
				tableBodyStructure.GetSkinSlotList(actorBody.BodyStructure);

			//Look for the body parts from the model's body structure
			foreach (BodyStructureTable.BodySkinsSlots record in tListSlots) {

				objFound = actorBody.ActorModel.transform.Find (record.ObjectName);

				if (objFound != null) {
					//Find material of the part in skin record
					matName = tableSkins.GetMaterialName(skinID, record.IDSlot);

					//Load the material and apply to the mesh
					matName = _config.GetPathName(CSConfig.PathTypeEnum.SkinsPath)+"/"+matName;

					//Load the skin material from Resources
					Material skinMat = Resources.Load<Material> (matName);

					if (skinMat == null) {
						Debug.LogError ("Skin material cannot be found.");
					}

					//Apply the material to object
					objFound.GetComponent<Renderer> ().sharedMaterial = skinMat;
				}
			}

			//Update actor information with the new skin ID
			actorBody.SkinID = skinID;

			if (actorBody.ClothingType == CSConfig.ClothingMethodEnum.FullObject) {
				//For each cloth used by the actor, try to change its texture to match the skin used
				//Only for Full Object method
				for (int i = 0; i < clothesList.Count; i++)
					UpdateClothSkin (clothesList [i].ClothID);
			}
		}


		//** AddCloth **
		//Add a cloth to actor
		public void AddCloth(int clothID, bool rebuildBody = true){

			//What slots this cloth need?
			List<ClothTable.ClothSlots> tClothSlots = tableCloth.GetSlotsList (clothID);

			bool[] markedForDeath = new bool[clothesList.Count];

			//Check the clothes the actor have, and check the slots
			for (int i = 0; i < tClothSlots.Count; i++) {
				//Find that cloth is using the slot
				for (int k = 0; k < clothesList.Count; k++) {
					int idx = clothesList [k].ClothSlots.FindIndex (x => x == tClothSlots [i].ClothSlot);
					//Existing cloth uses the same slot of the new one? Mark this cloth for death
					if (idx > -1)
						markedForDeath [k] = true;
				}
			}

			string modelName = "";

			//Remove all clothes marked for death
			for (int i = markedForDeath.Length - 1; i >= 0; i--){

				if (markedForDeath[i] == true) {

					//Get cloth model name
					modelName = tableCloth.GetModelObjectName(clothesList[i].ClothID, actorBody.BodySubtype);

					//Seek for the object
					Transform objFound = actorBody.ActorModel.transform.Find(modelName);

					//For body structure that use Full Object clothing method, delete the object
					if (actorBody.ClothingType == CSConfig.ClothingMethodEnum.FullObject) {
						if (objFound != null)
							DestroyObject (objFound.gameObject);
					} else {
						//Hide the oject
						if (objFound != null) objFound.gameObject.SetActive(false);
					}

					clothesList.RemoveAt(i);
				}
			}

			//Add the new cloth to the actor

			//GetType the name of cloth prefab
			modelName = tableCloth.GetModelObjectName (clothID, actorBody.BodySubtype);

			//Create new record to store the information about the cloth
			ActorCloth toAdd = new ActorCloth ();

			toAdd.ClothID = clothID;	//ID of the cloth

			//Transfer slots id to the new record
			foreach (ClothTable.ClothSlots record in tClothSlots)
				toAdd.ClothSlots.Add (record.ClothSlot);

			//Add the cloth to clothes list
			clothesList.Add (toAdd);

			//Assembly the full path of the prefab: folder and name
			string modelObjectName = _config.GetPathName(CSConfig.PathTypeEnum.ClothPath) + "/" + modelName;

			//Load the prefab from the Resources folder
			GameObject tempObject = (GameObject)Resources.Load(modelObjectName);

			if (tempObject == null) {
				//Error loading the object
				Debug.LogError ("Cloth object cannot be found.");
			}

			//Is the body structure using the full object or cloting method?
			if (actorBody.ClothingType == CSConfig.ClothingMethodEnum.FullObject) {
				//If full object, need to add the prefab to the actor

				GameObject clothObject = (GameObject)Instantiate (tempObject);

				//Change the name of the new instantiated object, to remove the "(Clone) "
				//from the name, and make sure the name is the same as in the cloth database
				clothObject.name = modelName;

				//Add the object to actor
				clothObject.transform.SetParent (actorBody.ActorModel.transform, false);

				//Get the bones to be used
				SkinnedMeshRenderer bSource = bonesSource.GetComponent<SkinnedMeshRenderer> ();

				//Check if the model has a skinnedMeshRenderer
				SkinnedMeshRenderer cMesh = clothObject.GetComponent<SkinnedMeshRenderer> ();

				if (cMesh != null)
					//If the model has a skinned, define the bones to be used
					cMesh.bones = bSource.bones;

				//Cloth model has childs?
				//If yes, need to update the bones for each child object
				Transform[] childs = clothObject.GetComponentsInChildren<Transform> ();

				for (int i = 0; i < childs.Length; i++) {

					SkinnedMeshRenderer bDest = childs [i].GetComponent<SkinnedMeshRenderer> ();

					if (bDest != null)
						bDest.bones = bSource.bones;
				}

				//Rebuild the body
				if (rebuildBody) RebuildBody ();

				//The new cloth has a body part? Need to update it's skin
				UpdateClothSkin (clothID);

			} else {
				//Is the body structure using the mesh substitution method?

				//Get the cloth slot name
				BodyStructureTable.BodyClothSlots recClothSloth = tableBodyStructure.GetClothSlotRecord(tClothSlots[0].ClothSlot, actorBody.BodyStructure);

				string slotName = recClothSloth.ObjectName;

				//Get the skinned mesh renderer from cloth prefab
				SkinnedMeshRenderer sourceMesh = tempObject.GetComponent<SkinnedMeshRenderer>();

				if (sourceMesh == null) 
					Debug.LogError ("Cloth doesn't have a SkinnedMeshRendered object. Cannot add cloth to actor.");
				
				//Apply the mesh from cloth prefab to the cloth slot object
				GameObject clothSlotObject = actorBody.ActorModel.transform.Find(slotName).gameObject;

				if (clothSlotObject == null) {
					Debug.LogError ("Error retrieving the cloth slot in the actor's model.");
				} else {
					SkinnedMeshRenderer destMesh = clothSlotObject.GetComponent<SkinnedMeshRenderer> ();

					if (destMesh == null) {
						Debug.LogError ("Cloth slot ["+slotName+"] in the actor's model doesn't have a SkinnedMeshRenderer.");
					} else {
						destMesh.sharedMesh = sourceMesh.sharedMesh;

						clothSlotObject.GetComponent<Renderer> ().sharedMaterial = tempObject.GetComponent<Renderer> ().sharedMaterial;

						clothSlotObject.SetActive (true);
					}
				}
			}

		}


		//** RebuildBody **
		//Rebuild body is used only when the clothing method is Full Object
		//This rotine will hide or show the body part depending of what is used by the clothes
		void RebuildBody(){

			List<BodyStructureTable.BodyClothSlots> tBodySlots = tableBodyStructure.GetClothSlotList(actorBody.BodyStructure);

			string slotName = "";
			int idx = 0;

			bool needToHide = false;

			//Show or hide the body parts
			for (int i = 0; i < tBodySlots.Count; i++) {

				slotName = tBodySlots [i].ObjectName;

				Transform objFound = actorBody.ActorModel.transform.Find(slotName);

				if (objFound != null) {

					needToHide = false;

					//Check if need to hide the object or not
					for (int k = 0; k < clothesList.Count; k++) {
						idx = clothesList [k].ClothSlots.FindIndex (x => x == tBodySlots [i].IDSlot);
						if (idx > -1)
							needToHide = true;
					}

					objFound.gameObject.SetActive (!needToHide);

				}

			}

			//Reapply the skin
			ApplyBodySkin(actorBody.SkinID);


		}


		//** UpdateClothSkin **
		// If using Full Object method, the cloth can have parts of the
		// body, so, need to apply the current skin to these parts
		void UpdateClothSkin(int idCloth){

			//Get actual skin of actor
			List<SkinsTable.SkinSlots> tList = tableSkins.GetSlotsList (actorBody.SkinID);

			//Get skin slots
			List<BodyStructureTable.BodySkinsSlots> tSkinSlots = tableBodyStructure.GetSkinSlotList (actorBody.BodyStructure);

			List<string> tSlotsNames = new List<string> ();

			string tString = "";

			//Get the names of the skins slots, eg, names of the objects that
			//need the skin material.
			for (int i = 0; i < tList.Count; i++) {
				tString = tSkinSlots.Find (x => x.IDSlot == tList [i].IDSkinSlot).ObjectName;
				tSlotsNames.Add (tString);
			}

			//Get the name of the object that holds the cloth
			string modelName = tableCloth.GetModelObjectName (idCloth, actorBody.BodySubtype);

			int index = 0;
			int idSlot = 0;
			string matName = "";

			//Seek for the object
			Transform clothObject = actorBody.ActorModel.transform.Find (modelName);

			if (clothObject != null) {
				//Cloth model has childs?

				Transform[] childs = clothObject.GetComponentsInChildren<Transform> ();

				//The object to be found need to have the name of one of the skins slots
				for (int i = 0; i < childs.Length; i++) {
					index = tSlotsNames.FindIndex (x => x == childs [i].name);

					if (index > -1) {

						//Get the ID of the slot used by the cloth
						idSlot = tSkinSlots.Find (x => x.ObjectName == childs [i].name).IDSlot;

						//Get the material name of the slot
						matName = tList.Find (x => x.IDSkinSlot == idSlot).MaterialName;

						//Assembly the full material name
						matName = _config.GetPathName(CSConfig.PathTypeEnum.SkinsPath)+"/"+matName;

						//Load the material
						Material skinMat = Resources.Load<Material> (matName);

						if (skinMat == null) {
							Debug.LogError ("Skin material not found.");
						}

						//Apply the material to the object
						childs[i].GetComponent<Renderer> ().sharedMaterial = skinMat;

					}
				}
			}

		}

		//** RemoveCloth **
		//Remove the cloth
		public void RemoveCloth(int clothID, int bodySubtype, bool rebuildBody = true){

			RemoveClothPiece (clothID, bodySubtype);

			if (rebuildBody) RebuildBody ();

		}

		//** RemoveAllCloths **
		//Remove all cloths, resseting the actor
		public void RemoveAllCloths(){

			for (int i = 0; i < clothesList.Count; i++) RemoveClothPiece(clothesList[i].ClothID, actorBody.BodySubtype);

			RebuildBody ();
				
		}


		//** RemoveClothPiece **
		private void RemoveClothPiece(int clothID, int bodySubtype){

			//Get the name of the object
			string modelName = tableCloth.GetModelObjectName (clothID, bodySubtype);

			//Seek for the object
			Transform objFound = actorBody.ActorModel.transform.Find (modelName);

			if (objFound != null) {

				//For body structure that use Full Object clothing method, delete the object
				if (actorBody.ClothingType == CSConfig.ClothingMethodEnum.FullObject) {
					if (objFound != null)
						DestroyObject (objFound.gameObject);
				} else {
					//Hide the oject
					objFound.gameObject.SetActive (false);
				}

			}

			//Remove the coth from the list even if the cloth object isn't found
			clothesList.RemoveAll (x => x.ClothID == clothID);

		}

		//** ChangeClothesForModel **
		//This method try to change the cloth object for the current body model
		public void ChangeClothesForModel(){

			//No clothes? Don't need to execute this
			if (clothesList.Count == 0)
				return;
			
			string actualObjectName = "";
			string newObjectName = "";

			List<int> toAdd = new List<int> ();
			List<string> toAddName = new List<string> ();

			//For each cloth that the actor is wearing, seek for the
			//cloth for the actual body model
			for (int i = 0; i < clothesList.Count; i++) {

				newObjectName = tableCloth.GetModelObjectName (clothesList [i].ClothID, actorBody.BodySubtype);

				toAdd.Add (clothesList [i].ClothID);
				toAddName.Add (newObjectName);

			}


			for (int i = 0; i < toAdd.Count; i++) {

				RemoveClothPiece (toAdd[i], oldBodySubtype);

				if (toAddName[i].Length > 0) AddCloth (toAdd[i], false);

			}

			RebuildBody ();

		}


		//** ApplyEyeSkin **
		//Change the skin of the eye
		public void ApplyEyeSkin(int eyeSkinID){


			Transform objFound;

			List<EyeSkinTable.EyeMaterial> eyeSkinsList = tableEyeSkin.GetMaterialList (eyeSkinID);

			List<EyesTable.EyeObject> eyeSlotsList = tableEyes.GetObjectsList (actorBody.EyeID);

			string matName;
			string strDebug;


			for (int i = 0; i < eyeSlotsList.Count; i++) {

				matName = eyeSkinsList.Find(x => x.IDObject == eyeSlotsList[i].ID).MaterialName;

				objFound = actorBody.ActorModel.transform.Find (eyeSlotsList[i].ObjectName);

				if (objFound != null) {
					//Load the material and apply to the mesh
					matName = _config.GetPathName(CSConfig.PathTypeEnum.SkinsPath)+"/"+matName;

					Material skinMat = Resources.Load<Material> (matName);

					if (skinMat == null) {
						Debug.LogError ("Eye skin not found.");
					}

					objFound.GetComponent<Renderer> ().sharedMaterial = skinMat;
				}
			}
		}


		//** AddProp **
		//Append a prop on its designed bone
		//The method of adding prop is the same for both methods
		//because a prop is added to a bone
		public void AddProp(int idProp){

			//Create new record to store the information about the prop
			ActorProp toAdd = new ActorProp ();

			toAdd.IDProp = idProp;	//ID of the prop
			toAdd.PropSlot = tableProps.GetRecord(idProp).IDSlot;	//Id of slot

			//GetType the name of prop prefab
			string modelName = tableProps.GetModelObjectName (idProp, actorBody.BodySubtype);

			//Get the name of slot (bone) where the prop need to be attached
			BodyStructureTable.BodyPropsSlots recTemp = tableBodyStructure.GetPropSlotRecord(toAdd.PropSlot, actorBody.BodyStructure);
			string slotName = "";

			if (recTemp != null)
				slotName = recTemp.ObjectName;

			Transform destSlot = null;

			//Look for the slot (bone) in the skeleton structure
			Transform[] bones = actorBody.BonesArmature.GetComponentsInChildren<Transform> ();

			for (int i = 0; i < bones.Length; i++) {
				if (bones [i].name == slotName) {
					destSlot = bones [i];
					i = bones.Length + 1;
				}
			}

			if (destSlot == null)
				Debug.LogError ("Prop slot bone cannot be found.");

			//Get the name of prop that is using the slot
			int prevPropID = 0;
			ActorProp tRec =  propsList.Find (x => x.PropSlot == toAdd.PropSlot);

			if (tRec != null)
				prevPropID = tRec.IDProp;

			//Remove previous prop in the slot. if it exists
			if (prevPropID > 0) {

				string prevPropName = tableProps.GetModelObjectName (prevPropID, actorBody.BodySubtype);

				Transform[] toRemove = actorBody.BonesArmature.GetComponentsInChildren<Transform> ();
				for (int i = 0; i < toRemove.Length; i++) {
					if (toRemove [i].name == prevPropName) {
						Destroy (toRemove [i].gameObject);
						i = toRemove.Length + 1;
					}
				}
			}

			//Remove the prop from the props list
			propsList.RemoveAll(x => x.PropSlot == toAdd.PropSlot);

			//Add the prop to props list
			propsList.Add (toAdd);

			//Assembly the full path of the prefab: folder and name
			string modelObjectName = _config.GetPathName(CSConfig.PathTypeEnum.PropPath) + "/" + modelName;

			//Load the prefab from the Resources folder
			GameObject tempObject = (GameObject)Resources.Load(modelObjectName);

			if (tempObject == null) {
				//Error loading the object
				Debug.LogError ("Prop object cannot be found.");
			}

			//Create a new instance of the prefab
			GameObject propObject = (GameObject)Instantiate (tempObject);

			//Change the name of the new instantiated object, to remove the "(Clone)"
			//from the name, and make sure the name is the same as in the cloth database
			propObject.name = modelName;

			//Add the object to actor
			propObject.transform.SetParent (destSlot, false);

		}


		//** RemoveAllProps **
		//Remove all the props added to actor
		public void RemoveAllProps(){


			for (int i = 0; i < propsList.Count; i++) {

				RemoveProp (propsList [i].IDProp);
			}

		}


		//** RemoveProp **
		//Remove a prop from actor
		public void RemoveProp(int idProp){

			int propSlot = tableProps.GetRecord(idProp).IDSlot;

			//Get the name of prop prefab
			string modelName = tableProps.GetModelObjectName (idProp, actorBody.BodySubtype);

			//Get the name of slot (bone) where the prop is attached
			BodyStructureTable.BodyPropsSlots recTemp = tableBodyStructure.GetPropSlotRecord(propSlot, actorBody.BodyStructure);
			string slotName = "";

			if (recTemp != null)
				slotName = recTemp.ObjectName;

			Transform destSlot = null;

			//Look for the slot (bone) in the skeleton structure
			Transform[] bones = actorBody.BonesArmature.GetComponentsInChildren<Transform> ();

			for (int i = 0; i < bones.Length; i++) {
				if (bones [i].name == slotName) {
					destSlot = bones [i];
					i = bones.Length + 1;
				}
			}

			if (destSlot == null)
				Debug.LogError ("Prop slot bone cannot be found.");

			//Remove prop in the slot
			if (idProp > 0) {

				Transform[] toRemove = actorBody.BonesArmature.GetComponentsInChildren<Transform> ();
				for (int i = 0; i < toRemove.Length; i++) {
					if (toRemove [i].name == modelName) {
						Destroy (toRemove [i].gameObject);
						i = toRemove.Length + 1;
					}
				}
			}

			//Remove prop from the props list
			propsList.RemoveAll(x => x.PropSlot == propSlot);

		}

	}
}
