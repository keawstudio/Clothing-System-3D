// ********************************
//  [class-name]
//
//  [class-description]
//
//  [project-name]
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

	public class EyesTable {

		//Eye header record structure
		public class Eye {
			public int ID = 0;
			public string Name = "";
		}

		//Eye objects record structure
		public class EyeObject {
			public int ID = 0;
			public int IDEye = 0;
			public string ObjectName = "";
		}

		//List of eyes
		static List<Eye> eyesList = new List<Eye>();

		//List of eyes objects
		static List<EyeObject> eyesObjectsList = new List<EyeObject> ();

		//Class used to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		//Name of the objects/files that holds the tables of records
		const string _eyeTable = "Eye";
		const string _eyeObject = "EyeObjects";

		static bool tableEyeLoaded = false;
		static bool tableObjectsLoaded = false;


		//** SaveTables **
		//Save all tables to database object
		public void SaveTables(){

			recordset.SaveRecordset (eyesList, _eyeTable);
			recordset.SaveRecordset (eyesObjectsList, _eyeObject);

		}

		#region "Eye"

		//** AddEye **
		//Add eye record
		public int AddEye(string name){

			int toReturn = 1;

			if (eyesList.Count > 0)
				toReturn = eyesList.OrderBy (x => x.ID).Last ().ID + 1;

			Eye toAd = new Eye ();

			toAd.ID = toReturn;
			toAd.Name = name;

			eyesList.Add (toAd);

			return toReturn;

		}


		//** EditEye **
		//Edit eye record
		public void EditEye(int id, string name){

			Eye toEdit = eyesList.Find (x => x.ID == id);

			toEdit.Name = name;

		}


		//** GetEyeName **
		//Get the name of the eye
		public string GetEyeName(int idEye){

			string toReturn = "";

			toReturn = eyesList.Find (x => x.ID == idEye).Name;

			return toReturn;
		}


		//** DeleteEye **
		//Delete an eye
		public int DeleteEye(int id){

			int toReturn = 0;

			//Check if the eye can be deleted

			if (toReturn == 0) {
				eyesList.RemoveAll (x => x.ID == id);
				eyesObjectsList.RemoveAll (x => x.IDEye == id);
			}

			return toReturn;

		}


		//** GetList **
		//Get list of eyes
		public List<Eye> GetList(bool onlyCopy = false){

			List<Eye> toReturn = new List<Eye> ();

			if (!onlyCopy) {
				toReturn = eyesList;
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				toReturn.AddRange (eyesList);
			}

			return toReturn;

		}


		//** LoadTable **
		//Retrieve the eyes list from the database object
		public void LoadTable(){

			if (!tableEyeLoaded) eyesList = recordset.GetRecordset<Eye>(_eyeTable);

			tableEyeLoaded = true;

		}


		//** UnloadTable
		//Remove the list from memory, unloading it
		public void UnloadTable(){

			eyesList.Clear ();

			tableEyeLoaded = false;

		}
		#endregion

		#region "EyeObject"

		//** AddEyeObject **
		//Insert new eye object
		public int AddEyeObject(int idEye, string objectName){

			int toReturn = 1;

			List<EyeObject> tList = eyesObjectsList.Where (x => x.IDEye == idEye).ToList();

			if (tList.Count > 0)
				toReturn = tList.OrderBy (x => x.ID).Last ().ID + 1;

			EyeObject toAdd = new EyeObject ();

			toAdd.ID = toReturn;
			toAdd.IDEye = idEye;
			toAdd.ObjectName = objectName;

			eyesObjectsList.Add (toAdd);

			return toReturn;

		}


		//** DeleteEyeObject **
		//Delet eye object
		public int DeleteEyeObject(int idEye, int idObject){

			int toReturn = 0;

			eyesObjectsList.RemoveAll (x => x.IDEye == idEye && x.ID == idObject);

			return toReturn;

		}


		//** DeleteObjects **
		//Delete all objects of an eye
		public void DeleteObjects(int idEye){

			eyesObjectsList.RemoveAll (x => x.IDEye == idEye);

		}


		//** EditEyeObject **
		//Edit an eye object
		public void EditEyeObject(int idEye, int idObject, string objectName){

			EyeObject toEdit = eyesObjectsList.Find (x => x.IDEye == idEye && x.ID == idObject);

			toEdit.ObjectName = objectName;

		}


		//** GetObjectsList **
		//Get the list of objects of an eye
		public List<EyeObject> GetObjectsList(int idEye, bool onlyCopy = false){

			List<EyeObject> toReturn = new List<EyeObject> ();

			if (!onlyCopy) {
				toReturn = eyesObjectsList.Where (x => x.IDEye == idEye).ToList();
			} else {
				toReturn.AddRange(eyesObjectsList.Where(x => x.IDEye == idEye).ToList());
			}
			return toReturn;

		}


		//** LoadObjectList **
		//Load the objects list from the database object
		public void LoadObjectList(){

			if (!tableObjectsLoaded) eyesObjectsList = recordset.GetRecordset<EyeObject>(_eyeObject);

			tableObjectsLoaded = true;

		}


		//** UnloadObjectList **
		//Remove the list form memory, unloading it
		public void UnloadObjectList(){

			eyesObjectsList.Clear ();

			tableObjectsLoaded = false;

		}
		#endregion



	}
}
