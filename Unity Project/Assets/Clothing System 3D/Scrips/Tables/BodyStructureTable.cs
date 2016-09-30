// ********************************
//  BodyStructureTable
//
//  Store the table of Body Structures
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

	public class BodyStructureTable {

		//Header Record
		public class BodyStructure {
			public int ID = 0;										//ID of structure
			public string Name = "";								//Name
			public int IDBodyType = 0;								//ID of Body type
			public CSConfig.ClothingMethodEnum ClothingMethod;		//What method to use for clothing chance?
			public List<int> IDBodySubtype = new List<int>();		//List of available subtypes for a structure
		}

		//Cloth Slots record
		public class BodyClothSlots {
			public int IDStructure;
			public int IDSlot;
			public string SlotName;			//Friendly name of slot
			public string ObjectName;		//Name of object that belongs to the slot
		}

		//Prop Slots record
		public class BodyPropsSlots {
			public int IDStructure;
			public int IDSlot;
			public string SlotName;			//Friendly name of slot
			public string ObjectName;		//Name of object that belongs to the slot
		}

		//Skins slots record
		public class BodySkinsSlots {
			public int IDStructure;
			public int IDSlot;
			public string SlotName;			//Friendly name of slot
			public string ObjectName;		//Name of object that belongs to the slot
		}

		//List with header records
		static List<BodyStructure> structureRecords = new List<BodyStructure>();

		//List with cloth slots records
		static List<BodyClothSlots> clothSlots = new List<BodyClothSlots> ();

		//List with props slots records
		static List<BodyPropsSlots> propSlots = new List<BodyPropsSlots>();

		//List with skins slots records
		static List<BodySkinsSlots> skinSlots = new List<BodySkinsSlots>();

		int actualRecord = 0;

		//Class used to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		//Name of the objects/files that holds the tables of records
		const string _strucTable = "BodyStructure";
		const string _clothTable = "ClothSlots";
		const string _propTable = "PropsSlots";
		const string _skinTable = "SkinsSlots";

		static bool strucTableLoaded = false;
		static bool clothTableLoaded = false;
		static bool propTableLoaded = false;
		static bool skinTableLoaded = false;


		//** SaveTable **
		//Save all lists to the database objects
		public void SaveTable(){

			recordset.SaveRecordset (structureRecords, _strucTable);
			recordset.SaveRecordset (clothSlots, _clothTable);
			recordset.SaveRecordset (propSlots, _propTable);
			recordset.SaveRecordset (skinSlots, _skinTable);

		}

		#region "Structure"

		//** AddStructure **
		//Insert new structure header
		public int AddStructure(BodyStructure toAdd){

			int iReturn = 1;

			if (structureRecords.Count > 0)
				iReturn = structureRecords.OrderBy (x => x.ID).Last ().ID + 1;

			toAdd.ID = iReturn;

			structureRecords.Add (toAdd);

			return iReturn;

		}


		//** DeleteStructure **
		//Delete a structure
		public int DeleteStructure(int id){

			int iReturn = 0;

			if (iReturn == 0) {
				//Remove the structure header and all records in the lists
				//that belongs to the structure
				structureRecords.RemoveAll (x => x.ID == id);		//Remove header
				clothSlots.RemoveAll (x => x.IDStructure == id);	//Remove cloth slots
				propSlots.RemoveAll (x => x.IDStructure == id);		//Remove prop slots
				skinSlots.RemoveAll (x => x.IDStructure == id);		//Remove skins slots
			}

			return iReturn;

		}


		//** GetRecord **
		//Return record of header
		public BodyStructure GetRecord(int id){

			BodyStructure recReturn = new BodyStructure ();

			int index = structureRecords.FindIndex (x => x.ID == id);

			if (index > -1)
				recReturn = structureRecords [index];

			return recReturn;

		}


		//** GetList **
		//Return a list with header records
		public List<BodyStructure> GetList(bool onlyCopy = false){

			List<BodyStructure> listReturn = new List<BodyStructure>();

			if (!onlyCopy) {
				listReturn = structureRecords;
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				listReturn.AddRange (structureRecords);
			}

			return listReturn;

		}


		//** LoadStructures **
		//Load the list of records from the database object
		public void LoadStructures(){

			if (!strucTableLoaded) structureRecords = recordset.GetRecordset<BodyStructure>(_strucTable);

			strucTableLoaded = true;

		}

		//** UnloadStructures **
		//Remove the list of records from memory, "unloading" it
		public void UnloadStructures(){

			structureRecords.Clear ();

			strucTableLoaded = false;

		}

		#endregion

		#region "Cloth Slot"

		//** AddClothSlot **
		//Insert a new slot for cloth
		public int AddClothSlot(int idStructure, string partName, string objName){

			int iReturn = 1;

			//Get number of new record
			if (clothSlots.Count(x => x.IDStructure == idStructure) > 0) 
				iReturn = clothSlots.OrderBy (x => x.IDSlot).Where (x => x.IDStructure == idStructure).Last ().IDSlot + 1;

			BodyClothSlots toAdd = new BodyClothSlots ();
			toAdd.IDStructure = idStructure;
			toAdd.SlotName = partName;
			toAdd.ObjectName = objName;
			toAdd.IDSlot = iReturn;

			clothSlots.Add (toAdd);

			return iReturn;

		}


		//** DeleteClothSlot **
		//Remove the selected cloth slot
		public int DeleteClothSlot(int id){
			
			int iReturn = 0;

			clothSlots.RemoveAll (x => x.IDSlot == id);

			return iReturn;

		}


		//** GetClothSlotRecord **
		//Return record of a selected cloth slot
		public BodyClothSlots GetClothSlotRecord(int idSlot, int idStructure){

			BodyClothSlots recReturn = new BodyClothSlots ();

			if (clothSlots.Count == 0)
				Debug.LogWarning ("Cloth Slots table is empty. Returning null record.");
			
			recReturn = clothSlots.Find (x => x.IDSlot == idSlot && x.IDStructure == idStructure);

			return recReturn;

		}


		//** GetClothSlotList //
		//Return a list with slots for cloth of a selected structure
		public List<BodyClothSlots> GetClothSlotList(int idStruct, bool onlyCopy = false){

			List<BodyClothSlots> listReturn = new List<BodyClothSlots> ();

			if (!onlyCopy) {
				listReturn = clothSlots.Where (x => x.IDStructure == idStruct).ToList ();
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				listReturn.AddRange (clothSlots.Where (x => x.IDStructure == idStruct).ToList ());
			}

			return listReturn;

		}


		//** UpdateClothSlots **
		//Update the list of cloth slots records
		public void UpdateClothSlots(int id, List<BodyClothSlots> newList){

			//Remove all previous records
			clothSlots.RemoveAll (x => x.IDStructure == id);

			//Add the new ones
			clothSlots.AddRange(newList);

		}


		//** LoadClothSlots **
		//Load the list of records from the database object
		public void LoadClothSlots(){

			if (!clothTableLoaded) clothSlots = recordset.GetRecordset<BodyClothSlots>(_clothTable);

			clothTableLoaded = true;

		}


		//** UnloadClothSlots **
		//Remove the list from memory, "unloading" it
		public void UnloadClothSlots(){

			clothSlots.Clear ();

			clothTableLoaded = false;

		}

		#endregion

		#region "Prop Slotd"

		//** AddPropSlot **
		//Insert new slot for prop
		public int AddPropSlot(int idStructure, string partName, string objName){
			
			int iReturn = 1;

			//Get number of new record
			if (propSlots.Count(x => x.IDStructure == idStructure) > 0) 
				iReturn = propSlots.OrderBy (x => x.IDSlot).Where (x => x.IDStructure == idStructure).Last ().IDSlot + 1;

			BodyPropsSlots toAdd = new BodyPropsSlots ();
			toAdd.IDStructure = idStructure;
			toAdd.SlotName = partName;
			toAdd.ObjectName = objName;
			toAdd.IDSlot = iReturn;

			propSlots.Add (toAdd);

			return iReturn;

		}


		//** DeletePropSlot **
		//Delete selected prop slot
		public int DeletePropSlot(int id){

			int iReturn = 0;

			propSlots.RemoveAll (x => x.IDSlot == id);

			return iReturn;

		}


		//** GetPropSlotRecord **
		//Return record of a selected prop slot
		public BodyPropsSlots GetPropSlotRecord(int idSlot, int bodyStruct){

			BodyPropsSlots recReturn = new BodyPropsSlots ();

			recReturn = propSlots.Find (x => x.IDStructure == bodyStruct && x.IDSlot == idSlot);

			return recReturn;

		}


		//** GetPropSlotList **
		//Return a list with prop slots from a given structure
		public List<BodyPropsSlots> GetPropSlotList(int idStruct, bool onlyCopy = false){

			List<BodyPropsSlots> listReturn = new List<BodyPropsSlots> ();

			if (!onlyCopy) {
				listReturn = propSlots.Where (x => x.IDStructure == idStruct).ToList ();
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				listReturn.AddRange (propSlots.Where (x => x.IDStructure == idStruct).ToList ());
			}

			return listReturn;

		}


		//** UpdatePropSlots **
		//Update list of prop slots for a structure
		public void UpdatePropSlots(int id, List<BodyPropsSlots> newList){

			//Remove previous records
			propSlots.RemoveAll (x => x.IDStructure == id);

			//Add the new ones
			propSlots.AddRange(newList);

		}


		//** LoadPropSlots **
		//Load the prop slots records from the database object
		public void LoadPropSlots(){

			if (!propTableLoaded) propSlots = recordset.GetRecordset<BodyPropsSlots>(_propTable);

			propTableLoaded = true;

		}


		//** UnloadPropSlots **
		//Remove the records list from memory, "unloading" it
		public void UnloadPropSlots(){

			propSlots.Clear ();

			propTableLoaded = false;

		}

		#endregion

		#region "Skin Slots"

		//** AddSkinSlot **
		//Insert a new skin slot
		public int AddSkinSlot(int idStructure, string partName, string objName){

			int iReturn = 1;

			//Get number of new record
			if (skinSlots.Count(x => x.IDStructure == idStructure) > 0) 
				iReturn = skinSlots.OrderBy (x => x.IDSlot).Where (x => x.IDStructure == idStructure).Last ().IDSlot + 1;

			BodySkinsSlots toAdd = new BodySkinsSlots ();
			toAdd.IDSlot = iReturn;
			toAdd.IDStructure = idStructure;
			toAdd.SlotName = partName;
			toAdd.ObjectName = objName;

			skinSlots.Add (toAdd);

			return iReturn;

		}


		//** DeleteSkinSlot **
		//Delete skin slot
		public int DeleteSkinSlot(int id){

			int iReturn = 0;

			skinSlots.RemoveAll (x => x.IDSlot == id);

			return iReturn;

		}


		//** GetSkinSlotRecord **
		//Retrieve record of a skin slot
		public BodySkinsSlots GetSkinSlotRecord(int id){

			BodySkinsSlots recReturn = new BodySkinsSlots ();

			return recReturn;

		}


		//** GetSkinSlotList **
		//Retrieve a list of skin slots records from a given structure
		public List<BodySkinsSlots> GetSkinSlotList(int idStruct, bool onlyCopy = false){

			List<BodySkinsSlots> listReturn = new List<BodySkinsSlots> ();

			if (!onlyCopy) {
				listReturn = skinSlots.Where (x => x.IDStructure == idStruct).ToList ();
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				listReturn.AddRange (skinSlots.Where (x => x.IDStructure == idStruct).ToList ());
			}

			return listReturn;

		}


		//** UpdateSkinSlots **
		//Update the skins slots list of a structure
		public void UpdateSkinSlots(int id, List<BodySkinsSlots> newList){

			//Remove previous records
			skinSlots.RemoveAll (x => x.IDStructure == id);

			//Add the new ones
			skinSlots.AddRange(newList);

		}


		//** LoadSkinSlots **
		//Load th list of sin slots records from the database object
		public void LoadSkinSlots(){

			if (!skinTableLoaded) skinSlots = recordset.GetRecordset<BodySkinsSlots>(_skinTable);

			skinTableLoaded = true;

		}


		//** UnloadSkinSlots **
		//Remove the list of skins slots from memory, "unloading" it
		public void UnloadSkinSlots(){

			skinSlots.Clear ();

			skinTableLoaded = false;
		
		}

		#endregion
	
	}
}
