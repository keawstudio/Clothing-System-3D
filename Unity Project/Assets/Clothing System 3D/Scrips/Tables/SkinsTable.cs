// ********************************
//  SkinsTable
//
//  Stores the table of body skins
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

	public class SkinsTable {

		//Skin record structure
		public class Skin {
			public int ID = 0;
			public string Name = "";
			public int IDModel = 0;
			public bool DefaultSkin = false;
		}

		//Skin slot record structure
		public class SkinSlots {
			public int IDSkin = 0;
			public int IDSkinSlot = 0;
			public string MaterialName = "";
		}


		//List of skins
		static List<Skin> skinsList = new List<Skin>();

		//List of skins slots
		static List<SkinSlots> skinksSlotsList = new List<SkinSlots> ();

		//Class used to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		//Name of the objects/files that holds the tables of records
		const string _skinTable = "Skins";
		const string _slotsTable = "SkinsParts";

		static bool tableSkinLoaded = false;
		static bool tableSlotsLoaded = false;


		//** SaveTable **
		//Save all lists to database object
		public void SaveTable(){

			recordset.SaveRecordset (skinsList, _skinTable);
			recordset.SaveRecordset (skinksSlotsList, _slotsTable);

		}

		#region "Skin"

		//** AddSkin **
		//Insert a new skin
		public int AddSkin(string Name, int idModel, bool defSkin){
			
			int iReturn = 1;

			if (skinsList.Count > 0)
				iReturn = skinsList.OrderBy (x => x.ID).Last ().ID + 1;

			Skin toAdd = new Skin ();

			toAdd.ID = iReturn;
			toAdd.IDModel = idModel;
			toAdd.Name = Name;
			toAdd.DefaultSkin = defSkin;

			skinsList.Add (toAdd);

			return iReturn;

		}


		//** DeleteSkin **
		//Delete a skin
		public void DeleteSkin(int id){

			skinsList.RemoveAll (x => x.ID == id);
			skinksSlotsList.RemoveAll (x => x.IDSkin == id);

		}


		//** UpdateSkin **
		//Update skin record
		public void UpdateSkin(int id, string Name, int idModel, bool defSkin){

			Skin record = skinsList.Find (x => x.ID == id);

			if (record != null) {
				record.Name = Name;
				record.IDModel = idModel;
				record.DefaultSkin = defSkin;
			}
		}


		//** GetSkinList **
		//Retrieve a list of skins
		public List<Skin> GetSkinList(bool onlyCopy = false){

			List<Skin> toReturn = new List<Skin> ();

			if (!onlyCopy) {
				toReturn = skinsList;
			} else {
				toReturn.AddRange (skinsList);
			}

			return toReturn;

		}


		//** GetSkinsOfModel **
		//Retrieve a list of skins from a body model
		public List<Skin> GetSkinsOfModel(int IDModel){

			List<Skin> toReturn;

			toReturn = skinsList.FindAll (x => x.IDModel == IDModel);

			return toReturn;
		}


		//** GetDefaultSkin **
		//Retrieve the default skin of the model
		public Skin GetDefaultSkin(int IDModel){

			Skin toReturn = new Skin ();
		
			toReturn = skinsList.Find (x => x.IDModel == IDModel && x.DefaultSkin == true);

			return toReturn;
		
		}


		//** LoadSkinsList **
		//Load the skins list from database object
		public void LoadSkinsList(){

			if (!tableSkinLoaded) skinsList = recordset.GetRecordset<Skin>(_skinTable);

			tableSkinLoaded = true;

		}


		//** UnloadSkinsList
		//Remove the list from memory, unloaing it
		public void UnloadSkinsList(){

			skinsList.Clear ();

			tableSkinLoaded = false;

		}
		#endregion

		#region "Slots"

		//** AddSlot **
		//Insert new slot
		public void AddSlot(int idSkin, int idSlot, string matName){

			SkinSlots toAdd = new SkinSlots ();

			toAdd.IDSkin = idSkin;
			toAdd.IDSkinSlot = idSlot;
			toAdd.MaterialName = matName;

			skinksSlotsList.Add (toAdd);

			//Debug.Log("AddSlot"

		}


		//** DeleteSlot **
		//Delete a slot
		public void DeleteSlot(int idSkin, int idSlot){

			skinksSlotsList.RemoveAll (x => x.IDSkin == idSkin && x.IDSkinSlot == idSlot);

		}


		//** UpdateSlot **
		//Update a slot
		public void UpdateSlot(int idSkin, int idSlot, string matName){

			SkinSlots record = skinksSlotsList.Find (x => x.IDSkin == idSkin && x.IDSkinSlot == idSlot);

			record.MaterialName = matName;

		}


		//** GetSlotsList **
		//Retrieve a list of slots from a skin
		public List<SkinSlots> GetSlotsList(int idSkin, bool onlyCopy = false){

			List<SkinSlots> toReturn = new List<SkinSlots> ();

			if (!onlyCopy) {
				toReturn = skinksSlotsList.FindAll(x => x.IDSkin == idSkin);
			} else {
				toReturn.AddRange (skinksSlotsList.FindAll(x => x.IDSkin == idSkin));
			}

			return toReturn;
				
		}

		//** DeleteSlots **
		//Delete all slots from a skin
		public void DeleteSlots(int idSkin){
			
			skinksSlotsList.RemoveAll(x => x.IDSkin == idSkin);
				
		}


		//** LoadSlotsList **
		//Load slots list from database object
		public void LoadSlotsList(){


			if (!tableSlotsLoaded) skinksSlotsList = recordset.GetRecordset<SkinSlots>(_slotsTable);

			tableSlotsLoaded = true;

		}


		//** UnloadSlotsList **
		//Unload slots list from memory, unloading it
		public void UnloadSlotsList(){

			skinksSlotsList.Clear ();

			tableSlotsLoaded = false;

		}


		//** GetMaterialName **
		//Get name of material
		public string GetMaterialName(int IDSkin, int IDSlot){

			string toReturn = "";

			toReturn = skinksSlotsList.Find (x => x.IDSkin == IDSkin && x.IDSkinSlot == IDSlot).MaterialName;

			return toReturn;

		}
		#endregion
	}
}
