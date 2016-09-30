// ********************************
//  ClothTable
//
//  Stores table of Cloths
//
//  Clothing System 3D
//  2016 - Larissa Redeker
//  Kittens and Elves at Work
//  http://www.keawstudio.com
//
// ********************************

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DBAccess = ClothingSystemDBXML;

namespace ClothingSystem3D {

	public class ClothTable {

		//Cloth header record structure
		public class Cloth {
			public int ID = 0;					//Id of the cloth
			public string Name = "";			//Name
			public int IDBodyStructure = 0;		//ID of the body structure that this cloth fits
		}

		//Cloth slots record structure
		public class ClothSlots {
			public int ClothID = 0;				//ID of cloth
			public int ClothSlot = 0;			//ID of cloth slot used by this cloth, in the body structure
		}

		//Cloth models record structure
		public class ClothModels {
			public int ClothID = 0;				//ID of cloth
			public int IDBodySubtype = 0;		//ID of body subtype
			public string ModelName = "";		//Name of the prefab to be used
		}
	
		//List of cloth head records
		static List<Cloth> clothList = new List<Cloth>();

		//List of slots record
		static List <ClothSlots> clothSlotsList = new List<ClothSlots>();

		//List of models record
		static List <ClothModels> clothModelsList = new List<ClothModels> ();

		//Class used to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		//Name of the objects/files that holds the tables of records
		const string _clothTable = "Cloths";
		const string _modelsTable = "ClothModels";
		const string _propsTable = "ClothParts";

		static bool clothHeaderLoaded = false;
		static bool clothSlotsLoaded = false;
		static bool clothModelsLoaded = false;


		//** SaveTable **
		//Sae all lists to database object
		public void SaveTable(){

			recordset.SaveRecordset (clothList, _clothTable);
			recordset.SaveRecordset (clothSlotsList, _propsTable);
			recordset.SaveRecordset (clothModelsList, _modelsTable);

		}

		#region "Cloth"

		//** AddCloth **
		//Add a new cloth header
		public int AddCloth(string name, int bodyStruc){

			int iReturn = 1;

			if (clothList.Count > 0)
				iReturn = clothList.OrderBy (x => x.ID).Last ().ID + 1;

			Cloth toAdd = new Cloth ();
			toAdd.ID = iReturn;
			toAdd.Name = name;
			toAdd.IDBodyStructure = bodyStruc;

			clothList.Add (toAdd);

			return iReturn;

		}


		//** UpdateCloth **
		//Update cloth record
		public void UpdateCloth(string name, int bodyStruct, int id){

			Cloth toUpdate = clothList.Find (x => x.ID == id);

			if (toUpdate != null){
				toUpdate.Name = name;
				toUpdate.IDBodyStructure = bodyStruct;
			}

		}


		//** DeleteCloth **
		//Delete cloth record
		public void DeleteCloth(int id){

			//Delete header
			clothList.RemoveAll(x => x.ID == id);

			//Delete all slots
			clothSlotsList.RemoveAll (x => x.ClothID == id);

			//Delete all models
			clothModelsList.RemoveAll (x => x.ClothID == id);

		}


		//** GetRecord
		//Get specific cloth record
		public Cloth GetRecord(int id){

			Cloth toReturn = new Cloth ();

			toReturn = clothList.Find (x => x.ID == id);

			return toReturn;
		}


		//** GetList **
		//Retrieve the list of clothes headers
		public List<Cloth> GetList(bool onlyCopy = false){

			List<Cloth> toReturn = new List<Cloth> ();

			if (!onlyCopy) {
				toReturn = clothList;
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				toReturn.AddRange (clothList);
			}

			return toReturn;
		}


		//** LoadClothesList **
		//Load the list of header records from database object
		public void LoadClothesList(){

			if (!clothHeaderLoaded) clothList = recordset.GetRecordset<Cloth>(_clothTable);

			clothHeaderLoaded = true;

		}


		//** UnloadClothesList **
		//Remove thelist from memory, unloading it
		public void UnloadClothesList(){

			clothList.Clear ();

			clothHeaderLoaded = false;

		}

		#endregion

		#region "Slots"

		//** AddClothSlot **
		//Insert a new slot
		public void AddClothSlot(int idCloth, int idSlot){

			ClothSlots toAdd = new ClothSlots ();

			toAdd.ClothID = idCloth;
			toAdd.ClothSlot = idSlot;

			clothSlotsList.Add (toAdd);

		}


		//** DeleteClothSlot **
		//Remove a slot
		public void DeleteClothSlot(int idCloth, int idSlot){

			clothSlotsList.RemoveAll (x => x.ClothID == idCloth && x.ClothSlot == idSlot);

		}


		//** GetSlotsList **
		//Retrieve slots list of a cloth
		public List<ClothSlots> GetSlotsList(int idCloth){

			List<ClothSlots> toReturn = new List<ClothSlots> ();

			toReturn = clothSlotsList.FindAll (x => x.ClothID == idCloth);

			return toReturn;

		}

		//** UpdateSlotList **
		//Update slots list of a cloth
		public void UpdateSlotList(List<ClothSlots> list, int idCloth){

			//Remove previous records
			clothSlotsList.RemoveAll (x => x.ClothID == idCloth);

			//Add the new ones
			clothSlotsList.AddRange (list);

		}


		//** LoadSlotsList **
		//Load the list of slots from database object
		public void LoadSlotsList(){

			if (!clothSlotsLoaded) clothSlotsList = recordset.GetRecordset<ClothSlots>(_propsTable);

			clothSlotsLoaded = true;

		}


		//** UnloadSlotsList **
		//Remove the list from memory, unloading it
		public void UnloadSlotsList(){

			clothSlotsList.Clear ();

			clothSlotsLoaded = false;

		}


		#endregion

		#region "Models"

		//** AddModel **
		//Insert new model
		public void AddModel(int IDSubtype, int IDCloth, string prefabName){

			ClothModels toAdd = new ClothModels ();

			toAdd.ClothID = IDCloth;
			toAdd.IDBodySubtype = IDSubtype;
			toAdd.ModelName = prefabName;

			clothModelsList.Add (toAdd);

		}


		//** LoadModelsList **
		//Load the list of models from database object
		public void LoadModelsList(){

			if (!clothModelsLoaded) clothModelsList = recordset.GetRecordset<ClothModels>(_modelsTable);

			clothModelsLoaded = true;

		}


		//** UnloadModelsList **
		//Remove list from memory, unloading it
		public void UnloadModelsList(){

			clothModelsList.Clear ();

			clothModelsLoaded = false;

		}


		//** DeleteClothModels **
		//Remove models from a given cloth
		public void DeleteClothModels(int idCloth){

			clothModelsList.RemoveAll (x => x.ClothID == idCloth);

		}


		//** GetModelsList **
		//Retrieve list of models from a cloth
		public List<ClothModels> GetModelsList(int idCloth){

			List<ClothModels> toReturn = new List<ClothModels> ();

			toReturn = clothModelsList.FindAll (x => x.ClothID == idCloth);

			return toReturn;

		}

		//** GetModelObjectName **
		//Get the name of the prefab used by the cloth with the body subtype
		public string GetModelObjectName(int idCloth, int bodySubtype){

			string toReturn = "";

			ClothModels tRec = clothModelsList.Find (x => x.ClothID == idCloth && x.IDBodySubtype == bodySubtype);

			if (tRec != null) toReturn = tRec.ModelName;

			return toReturn;

		}


		//** GetClothsByBodytype **
		//Retrieve a ist of cloths that have models for a specified body structure and body type
		public List<Cloth> GetClothsByBodytype(int idBodytype, int idBodyStruct){

			List<Cloth> toReturn = new List<Cloth>();

			var query2 = from p1 in clothList from p2 in clothModelsList where p2.ClothID == p1.ID && p2.IDBodySubtype == idBodytype && p1.IDBodyStructure == idBodyStruct
				select new {ClothName = p1.Name, ClothID = p1.ID, ModelID = p2.ModelName};

			foreach (var rec in query2) {

				Cloth toAdd = new Cloth ();

				toAdd.ID = rec.ClothID;
				toAdd.Name = rec.ClothName;
				toAdd.IDBodyStructure = idBodyStruct;

				toReturn.Add (toAdd);

			}

			return toReturn;

		}

		#endregion
	}
}
