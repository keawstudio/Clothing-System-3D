// ********************************
//  BodyModelTable
//
//  Store the table of Body Models
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

	public class BodyModelTable {

		//Name of the object/file that holds the table of records
		const string _tableName = "BodyModel";

		//Record structure
		public class BodyModel {
			public int ID = 0;					//ID of the body model
			public string Name = "";			//Name
			public int IDBodyStructure = 0;		//ID of Body Structure that the model belongs to
			public int IDBodySubtype = 0;		//ID of Body Subtype that the model belongs to
			public string ModelName = "";		//Name of the prefab that store the model object
			public int IDEye = 0;				//Eye object used in this model
			public string ArmatureName = "";	//Name of the object that holds the bone structure
		}

		//List of records
		static List<BodyModel> modelsList = new List<BodyModel>();

		//Class used to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		static bool tableLoaded = false;

		//** AddModel **
		//Insert new model in the table
		public int AddModel(BodyModel record){

			int iReturn = 1;

			//ID is an "auto-number" field
			if (modelsList.Count > 0)
				iReturn = modelsList.OrderBy (x => x.ID).Last ().ID + 1;

			record.ID = iReturn;

			modelsList.Add (record);

			return iReturn;

		}


		//** GetModel **
		// Retrieve a body model record
		public BodyModel GetModel(int id){

			BodyModel toReturn = new BodyModel ();

			if (modelsList.Count == 0)
				Debug.LogWarning ("Cannot get Body Model. Table is empty.");
			
			toReturn = modelsList.Find (x => x.ID == id);

			return toReturn;
		}


		//** DeleteModel **
		//Delete a model
		public int DeleteModel(int id){

			int iReturn = 0;

			if (iReturn == 0)
				modelsList.RemoveAll (x => x.ID == id);
			
			return iReturn;
		}


		// ** EditModel **
		//Update record
		public void EditModel(BodyModel record){

			BodyModel toChange = modelsList.Find (x => x.ID == record.ID);

			if (toChange != null)
				toChange = record;
			else
				Debug.LogWarning ("Cannot edit Body Model. Record not found");
		}


		//** GetList **
		// Return the list of records
		public List<BodyModel> GetList(bool onlyCopy = false){

			if (modelsList.Count == 0) Debug.LogWarning ("Cannot return list. Table isn't loaded.");

			List<BodyModel> toReturn = new List<BodyModel> ();

			if (!onlyCopy) {
				//Retrieve a reference of the list
				toReturn = modelsList;
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				toReturn.AddRange (modelsList);
			}

			return toReturn;

		}

		//** GetModelsOfType **
		//Return a list of records that belongs to one Body Structure
		public List<BodyModel> GetModelsOfType(int idStruc, bool onlyCopy = false){

			List<BodyModel> toReturn = new List<BodyModel> ();

			if (!onlyCopy) {
				toReturn = modelsList.FindAll(x => x.IDBodyStructure == idStruc);
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				toReturn.AddRange (modelsList.FindAll(x => x.IDBodyStructure == idStruc));
			}

			return toReturn;

		}


		//** LoadList **
		//Load the table from the database file
		public void LoadList(){

			if (!tableLoaded) modelsList = recordset.GetRecordset<BodyModel>(_tableName);

			tableLoaded = true;

		}


		//** UnloadList **
		//Removes the list from memory, "unloading" it
		public void UnloadList(){

			modelsList.Clear ();

			tableLoaded = false;

		}


		//** SaveList **
		//Save the table to the database file
		public void SaveList(){

			recordset.SaveRecordset(modelsList, _tableName);

		}

	}
}
