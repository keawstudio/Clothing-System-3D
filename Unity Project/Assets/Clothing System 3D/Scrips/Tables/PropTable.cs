// ********************************
//  PropTable
//
//  Store the table of props
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

	public class PropTable {

		//Prop record structure
		public class Prop {
			public int ID = 0;
			public string Name = "";
			public int IDBodyStructure = 0;
			public int IDSlot = 0;
		}

		//Prop model record structure
		public class PropModels {
			public int PropID = 0;
			public int IDBodySubtype = 0;
			public string ModelName = "";
		}

		//List of props records
		static List<Prop> propList = new List<Prop>();

		//List of prop models records
		static List <PropModels> propModelsList = new List<PropModels> ();

		//Class used to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		//Name of the objects/files that holds the tables of records
		const string _propTable = "Props";
		const string _modelsTable = "PropsModels";

		static bool tablePropLoaded = false;
		static bool tableModelLoaded = false;


		//** SaveTable **
		//Save all lists to database object
		public void SaveTable(){

			recordset.SaveRecordset (propList, _propTable);
			recordset.SaveRecordset (propModelsList, _modelsTable);

		}


		#region "Prop"

		//** AddProp **
		//Insert a new prop
		public int AddProp(Prop record){

			int iReturn = 1;

			if (propList.Count > 0)
				iReturn = propList.OrderBy (x => x.ID).Last ().ID + 1;

			record.ID = iReturn;

			propList.Add (record);

			return iReturn;

		}


		//** UpdateProp **
		//Update prop record
		public void UpdateProp(Prop record){

			Prop toUpdate = propList.Find (x => x.ID == record.ID);

			if (toUpdate != null)
				toUpdate = record;

		}


		//** DeleteProp **
		//Delete a prop
		public void DeleteProp(int id){

			//Delete the prop
			propList.RemoveAll(x => x.ID == id);

			//Delete all models of the prop
			propModelsList.RemoveAll (x => x.PropID == id);

		}


		//** GetRecord **
		//Return a prop record
		public Prop GetRecord(int id){

			Prop toReturn = new Prop ();

			toReturn = propList.Find (x => x.ID == id);

			return toReturn;
		}


		//** GetList **
		//Return a list of props
		public List<Prop> GetList(bool onlyCopy = false){

			List<Prop> toReturn = new List<Prop> ();

			if (!onlyCopy) {
				toReturn = propList;
			} else {
				toReturn.AddRange (propList);
			}

			return toReturn;
		}


		//** LoadPropList **
		//Load props list from database object
		public void LoadPropList(){

			if (!tablePropLoaded) propList = recordset.GetRecordset<Prop>(_propTable);

			tablePropLoaded = true;

		}


		//** UnloadPropList **
		//Remove the list from memory, unloading it
		public void UnloadPropList(){

			propList.Clear ();

			tablePropLoaded = false;

		}
		#endregion


		#region "Models"

		//** AddModel **
		//Insert a new model
		public void AddModel(int IDSubtype, int IDCloth, string prefabName){

			PropModels toAdd = new PropModels ();

			toAdd.PropID = IDCloth;
			toAdd.IDBodySubtype = IDSubtype;
			toAdd.ModelName = prefabName;

			propModelsList.Add (toAdd);

		}


		//** LoadModelsList **
		//Load the models list from database object
		public void LoadModelsList(){

			if (!tableModelLoaded) propModelsList = recordset.GetRecordset<PropModels>(_modelsTable);

			tableModelLoaded = true;

		}


		//** UnloadModelsList **
		//Remove the list from memory, unloading it
		public void UnloadModelsList(){

			propModelsList.Clear ();

			tableModelLoaded = false;

		}


		//** DeleteClothModels **
		//Delete models of a prop
		public void DeletePropModels(int idProp){

			propModelsList.RemoveAll (x => x.PropID == idProp);

		}


		//** GetModelsList **
		//Retrieve a list of models of a prop
		public List<PropModels> GetModelsList(int idCloth){

			List<PropModels> toReturn = new List<PropModels> ();

			toReturn = propModelsList.FindAll (x => x.PropID == idCloth);

			return toReturn;

		}


		//** GetPropsByBodytype **
		//Retrieve a list of props of a body type
		public List<Prop> GetPropsByBodytype(int idBodytype, int idBodyStruct){

			List<Prop> toReturn = new List<Prop>();

			var query2 = from p1 in propList from p2 in propModelsList where p2.PropID == p1.ID && p2.IDBodySubtype == idBodytype && p1.IDBodyStructure == idBodyStruct
				select new {PropName = p1.Name, PropID = p1.ID, ModelID = p2.ModelName};

			foreach (var rec in query2) {

				Prop toAdd = new Prop();

				toAdd.ID = rec.PropID;
				toAdd.Name = rec.PropName;
				toAdd.IDBodyStructure = idBodyStruct;

				toReturn.Add (toAdd);

			}

			return toReturn;

		}


		//** GetModelObjectName **
		//Retrieve the name of a prop object for a specified bpdy subtype
		public string GetModelObjectName(int idProp, int bodySubtype){

			string toReturn = "";

			PropModels tRec = propModelsList.Find (x => x.PropID == idProp && x.IDBodySubtype == bodySubtype);

			if (tRec != null) toReturn = tRec.ModelName;

			return toReturn;

		}


		#endregion
	}
}
