// ********************************
//  setup_scene
//
//  Controls the example scene "setup"
//
//  Clothing System 3D
//  2016 - Larissa Redeker
//  Kittens and Elves at Work
//  http://www.keawstudio.com
//
// ********************************

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ClothingSystem3D;
using System;
using System.Linq;

public class setup_scene : MonoBehaviour {

	public Dropdown dnBodyModels;
	public Dropdown dnBodySkins;
	public Dropdown dnEyeSkin;
	public Dropdown dnBodyType;

	public Toggle optCloth;
	public Toggle optRemoveCloth;

	public Button optProp;

	public Text txtStatus;

	public GameObject objPlayer;

	public Transform clothContent;
	public Transform propContent;

	ActorScript actorScript;

	int modelIndex = 0;
	int selectedBodyType = 0;


	List<BodyModelTable.BodyModel> modelsList;
	List<EyeSkinTable.EyeSkin> eyeSkinsList;
	List<SkinsTable.Skin> skinsList;
	List<BodyTypeTable.BodyType> bodyTypeList;

	[Serializable]
	public class ClothList {
		public int IDCloth;
		public string Name;
		public Button optObject;
	}

	[SerializeField]
	public List<ClothList> clothList = new List<ClothList>();

	[Serializable]
	public class BodyObjects
	{
		public string TypeName;
		public GameObject BodyObject;
	}

	[SerializeField]
	public List<BodyObjects> bodiesObjects = new List<BodyObjects> ();

	[Serializable]
	public class PropList {
		public int IDProp;
		public string Name;
		public Button optObject;
	}

	[SerializeField]
	public List<PropList> propList = new List<PropList>();


	BodyModelTable tableBodyModel = new BodyModelTable();
	ClothTable tableCloth = new ClothTable();
	EyeSkinTable tableEyeSkins = new EyeSkinTable();
	SkinsTable tableSkins = new SkinsTable();
	BodyTypeTable tableBodyType = new BodyTypeTable();
	PropTable tableProp = new PropTable();
	BodyStructureTable tableBodyStructure = new BodyStructureTable();


	// Use this for initialization
	void Start () {

		tableBodyType.LoadTable ();

		tableBodyStructure.LoadStructures ();

		tableBodyModel.LoadList ();

		tableSkins.LoadSkinsList ();

		tableEyeSkins.LoadSkinsTable ();

		tableCloth.LoadClothesList ();
		tableCloth.LoadModelsList ();
		tableCloth.LoadSlotsList ();

		tableProp.LoadPropList();
		tableProp.LoadModelsList();

		LoadBodyTypes ();

	}


	//** LoadBodyTypes **
	//Load the available Actor models
	public void LoadBodyTypes(){

		bodyTypeList = tableBodyType.GetList ();

		dnBodyType.options.Add(new Dropdown.OptionData("Select player type"));

		foreach (BodyTypeTable.BodyType record in bodyTypeList) {
			dnBodyType.options.Add (new Dropdown.OptionData (record.Name));
		}

		dnBodyType.RefreshShownValue ();

	}


	//** ApplyBodyType **
	//Active the selected Actor model
	public void ApplyBodyType(){

		selectedBodyType = bodyTypeList [dnBodyType.value - 1].ID;

		string selType = bodyTypeList [dnBodyType.value - 1].Name;

		foreach (BodyObjects record in bodiesObjects)
			record.BodyObject.SetActive (false);

		GameObject toShow = bodiesObjects.Find (x => x.TypeName == selType).BodyObject;

		toShow.SetActive (true);

		objPlayer = toShow;

		actorScript = objPlayer.GetComponent<ActorScript> ();

		dnBodySkins.ClearOptions ();
		dnEyeSkin.ClearOptions ();

		LoadBodyModels (selectedBodyType);

	}


	//** LoadBodyModels **
	//Load available Body Models for the selected Body Type
	//And fill the dropbox
	public void LoadBodyModels(int idType){

		int selValue = 0;
		int iCont = 0;

		dnBodyModels.options.Clear ();

		modelsList = new List<BodyModelTable.BodyModel>();

		List <BodyModelTable.BodyModel> tModel = tableBodyModel.GetList ();
		List <BodyStructureTable.BodyStructure> tStruc = tableBodyStructure.GetList ();

		var result = from mod in tModel
			from struc in tStruc
				where struc.ID == mod.IDBodyStructure && struc.IDBodyType == idType
			select new {IDModel = mod.ID, Name = mod.Name};


		dnBodyModels.options.Add (new Dropdown.OptionData ("Select a body model"));

		foreach (var record in result) {
			dnBodyModels.options.Add (new Dropdown.OptionData (record.Name));

			BodyModelTable.BodyModel toAdd = new BodyModelTable.BodyModel ();
			toAdd.ID = record.IDModel;
			toAdd.Name = record.Name;
			toAdd.IDBodyStructure = actorScript.actorBody.BodyStructure;
			toAdd.IDBodySubtype = actorScript.actorBody.BodySubtype;
			modelsList.Add (toAdd);
		}

		dnBodyModels.value = 0;
		dnBodyModels.RefreshShownValue ();

	}


	//** ApplyBodyModel **
	//Apply the selected Body Model to the Actor
	public void ApplyBodyModel(){

		if (dnBodyModels.value == 0)
			return;

		int idModel = modelsList [dnBodyModels.value - 1].ID;

		//Send to ActorScript te request to change the body model
		actorScript.ApplyBodyModel (idModel);

		if (!actorScript.AutoChangeCloth) {
			//Change cloth for body type manually
			if (optRemoveCloth.isOn)
				actorScript.RemoveAllCloths ();
			else
				actorScript.ChangeClothesForModel();
		}

		LoadClothes ();
		LoadProps ();
		LoadBodySkins ();
		LoadEyeSkin ();

	}

	void LoadBodySkins(){

		int selValue = 0;
		int iCont = 1;

		skinsList = new List<SkinsTable.Skin> ();

		skinsList = tableSkins.GetSkinsOfModel (actorScript.actorBody.BodyID);

		dnBodySkins.ClearOptions ();

		dnBodySkins.options.Add (new Dropdown.OptionData ("Select body skin"));

		foreach (SkinsTable.Skin record in skinsList) {
			dnBodySkins.options.Add (new Dropdown.OptionData (record.Name));

			if (record.DefaultSkin)
				actorScript.actorBody.SkinID = record.ID;

			if (record.ID == actorScript.actorBody.SkinID)
				selValue = iCont;

			iCont++;
		}

		dnBodySkins.value = selValue;
		dnBodySkins.RefreshShownValue ();

		if (skinsList.Count > 0) ApplyBodySkin ();

	}

	public void ApplyBodySkin(){

		if (dnBodySkins.value == 0)
			return;

		int idSkin = skinsList [dnBodySkins.value - 1].ID;

		actorScript.ApplyBodySkin (idSkin);


	}

	void LoadEyeSkin(){

		dnEyeSkin.ClearOptions ();

		//Body model of actor has eyes? Get their ID
		int idEye = tableBodyModel.GetModel(actorScript.actorBody.BodyID).IDEye;

		if (idEye > 0) {

			eyeSkinsList = new List<EyeSkinTable.EyeSkin> ();

			eyeSkinsList = tableEyeSkins.GetSkinList ();

			eyeSkinsList = eyeSkinsList.FindAll (x => x.IDEye == idEye);

			dnEyeSkin.options.Clear ();

			dnEyeSkin.options.Add (new Dropdown.OptionData ("Select a skin"));

			foreach (EyeSkinTable.EyeSkin record in eyeSkinsList) {
				dnEyeSkin.options.Add (new Dropdown.OptionData (record.SkinName));
			}

		}
	}

	public void ApplyEyeSkin(){

		int idSkin = eyeSkinsList [dnEyeSkin.value - 1].IDSkin;

		actorScript.ApplyEyeSkin (idSkin);

	}

	void LoadClothes(){

		int idBodyStruct = actorScript.actorBody.BodyStructure;
		int idBodySubt = actorScript.actorBody.BodySubtype;

		List<ClothTable.Cloth> tList = tableCloth.GetClothsByBodytype (idBodySubt, idBodyStruct);

		foreach (ClothList tItem in clothList)
			Destroy (tItem.optObject.gameObject);

		clothList.Clear();

		foreach (ClothTable.Cloth cloth in tList) {

			ClothList toAdd = new ClothList ();

			toAdd.IDCloth = cloth.ID;
			toAdd.Name = cloth.Name;

			Button optionList = Instantiate<Button>(optProp);

			Text tText = optionList.GetComponentInChildren<Text> ();
			tText.text = cloth.Name;
			optionList.onClick.AddListener(() => UpdateCloth(toAdd.IDCloth));
			optionList.transform.SetParent (clothContent.transform, false);

			toAdd.optObject = optionList;

			int inActor = actorScript.clothesList.FindIndex (x => x.ClothID == cloth.ID);

			clothList.Add (toAdd);

		}

	}

	void UpdateCloth(int idCloth){

		int inActor = actorScript.clothesList.FindIndex (x => x.ClothID == idCloth);

		if (inActor > -1) {
			actorScript.RemoveCloth (idCloth, actorScript.actorBody.BodySubtype,true);
		} else {
			actorScript.AddCloth (idCloth);
		}

		int iIndex = 0;

		for (int i = 0; i < clothList.Count; i++) {

			iIndex = actorScript.clothesList.FindIndex (x => x.ClothID == clothList [i].IDCloth);

			Text optText = clothList [i].optObject.GetComponentInChildren<Text> ();

			if (iIndex > -1) {
				optText.fontStyle = FontStyle.Bold;
				optText.color = Color.blue;
			} else {
				optText.fontStyle = FontStyle.Normal;
				optText.color = Color.black;
			}
		}

	}

	void LoadProps(){

		int idBodyStruct = actorScript.actorBody.BodyStructure;
		int idBodySubt = actorScript.actorBody.BodySubtype;

		List<PropTable.Prop> tList = tableProp.GetPropsByBodytype (idBodySubt, idBodyStruct);

		foreach (PropList tItem in propList)
			Destroy (tItem.optObject.gameObject);

		propList.Clear();

		foreach (PropTable.Prop prop in tList) {

			PropList toAdd = new PropList ();

			toAdd.IDProp = prop.ID;
			toAdd.Name = prop.Name;

			Button optionList = Instantiate<Button>(optProp);
			Text tText = optionList.GetComponentInChildren<Text> ();
			tText.text = prop.Name;
			optionList.onClick.AddListener(() => ApplyProp(toAdd.IDProp));
			optionList.transform.SetParent (propContent.transform, false);

			toAdd.optObject = optionList;

			propList.Add (toAdd);

		}
	}

	public void ApplyProp(int idProp){

		int inActor = actorScript.propsList.FindIndex (x => x.IDProp == idProp);

		if (inActor > -1) {
			actorScript.RemoveProp(idProp);
		} else {
			actorScript.AddProp(idProp);
		}

		int iIndex = 0;

		for (int i = 0; i < propList.Count; i++) {

			iIndex = actorScript.propsList.FindIndex (x => x.IDProp == propList[i].IDProp);

			Text optText = propList[i].optObject.GetComponentInChildren<Text> ();

			if (iIndex > -1) {
				optText.fontStyle = FontStyle.Bold;
				optText.color = Color.blue;
			} else {
				optText.fontStyle = FontStyle.Normal;
				optText.color = Color.black;
			}
		}

	}

}
