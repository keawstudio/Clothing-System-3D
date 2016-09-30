// ********************************
//  BodySubtypeTable
//
//  Store the table of Body Subtypes
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

	public class BodySubtypeTable {

		//Name of the object/file that holds the table of records
		const string _tableName = "BodySubtypes";

		//Record structure
		public class BodySubtype {
			public int ID;
			public string Name;
		}

		//List of records
		static List<BodySubtype> records = new List<BodySubtype>();

		//Class used to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		static bool tableLoaded = false;


		//** AddRecord **
		//Add a record
		public int AddRecord(string name){

			int newID = 1;

			if (records.Count > 0)
				newID = records.OrderBy (x => x.ID).Last ().ID + 1;

			BodySubtype toAdd = new BodySubtype ();

			toAdd.ID = newID;
			toAdd.Name = name;

			records.Add (toAdd);

			return newID;
		}


		//** DeleteRecord **
		//Delete a record
		public int DeleteRecord(int id){

			int errorReturn = 0;

			//Need to check first if the record is used in another table

			if (errorReturn == 0) {
				//Not found in another table, proceed to deletion
				BodySubtype itemToRemove = records.SingleOrDefault (x => x.ID == id);

				if (itemToRemove != null)
					records.Remove (itemToRemove);
			}

			return errorReturn;

		}


		//** GetRecord **
		//Return a record
		public BodySubtype GetRecord(int id){

			BodySubtype recReturn = new BodySubtype ();

			recReturn = records.Find (x => x.ID == id);

			return recReturn;

		}


		//** GetList **
		//Return a list of records
		public List<BodySubtype> GetList(bool onlyCopy = false){

			List<BodySubtype> toReturn = new List<BodySubtype> ();

			if (!onlyCopy) {
				//Get the reference to the list
				toReturn = records;
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				toReturn.AddRange (records);
			}

			return toReturn;

		}


		//** ClearTable **
		//Remove all records from table
		public void ClearTable(){

			records.Clear ();
			SaveTable ();

		}


		//** UnloadTable **
		//Remove list from memory, "unloading" it
		public void UnloadTable(){

			records.Clear ();

			tableLoaded = false;

		}


		//** LoadTable **
		//Load records list from the database object
		public void LoadTable(){

			if (!tableLoaded) records = recordset.GetRecordset<BodySubtype> (_tableName);

			tableLoaded = true;

		}


		//** SaveTable **
		//Save records list in the database object
		public void SaveTable(){

			recordset.SaveRecordset (records, _tableName);

		}
	}
}
