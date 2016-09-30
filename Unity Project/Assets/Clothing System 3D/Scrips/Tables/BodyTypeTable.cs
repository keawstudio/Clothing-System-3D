// ********************************
//  BodyTypeTable
//
//  Store the table of Body Types
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

	public class BodyTypeTable {

		//Name of the object/file that holds the table of records
		const string _tableName = "BodyTypes";

		//Record structure
		public class BodyType {
			public int ID;
			public string Name;
		}

		//List with records
		static List<BodyType> records = new List<BodyType>();

		//Class to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		static bool tableLoaded = false;


		//** AddRecord **
		//Add a record
		public int AddRecord(string name){

			int newID = 1;

			if (records.Count > 0)
				newID = records.OrderBy (x => x.ID).Last ().ID + 1;

			BodyType toAdd = new BodyType ();

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
				BodyType itemToRemove = records.SingleOrDefault (x => x.ID == id);

				if (itemToRemove != null)
					records.Remove (itemToRemove);
			}

			return errorReturn;

		}


		//** GetRecord **
		//Return a record
		public BodyType GetRecord(int id){

			BodyType recReturn = new BodyType ();

			recReturn = records.Find (x => x.ID == id);

			return recReturn;

		}


		//** GetList **
		//Return a list of records
		public List<BodyType> GetList(bool onlyCopy = false){

			List<BodyType> toReturn = new List<BodyType> ();

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
		//Remove list from memory, unloading it
		public void UnloadTable(){

			records.Clear ();

			tableLoaded = false;

		}


		//** LoadTable **
		//Load list of records from database object
		public void LoadTable(){

			if (!tableLoaded) records = recordset.GetRecordset<BodyType>(_tableName);

			tableLoaded = true;

		}


		//** SaveTable **
		//Save list of records to database object
		public void SaveTable(){

			recordset.SaveRecordset (records, _tableName);

		}
	}
}
