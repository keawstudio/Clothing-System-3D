// ********************************
//  EyeSkinTable
//
//  Store the table for eye skins
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

	public class EyeSkinTable {

		//Record struture for eye header
		public class EyeSkin{
			public int IDEye = 0;
			public int IDSkin = 0;
			public string SkinName = "";
		}

		//Record structure for skins materials
		public class EyeMaterial {
			public int ID = 0;
			public int IDEyeSkin = 0;
			public int IDObject = 0;
			public string MaterialName = "";
		}

		//List of eye skins
		static List<EyeSkin> skinsList = new List<EyeSkin> ();

		//List of skin materials
		static List<EyeMaterial> materialList = new List<EyeMaterial>();

		//Class used to save and retrieve data
		DBAccess.Recordset recordset = new DBAccess.Recordset();

		//Name of the objects/files that holds the tables of records
		const string _materialTable = "EyeSkinMaterial";
		const string _skinTable = "EyeSkins";

		static bool materialTableLoaded = false;
		static bool skinsTableLoaded = false;


		//** SaveTables **
		//Save lists in the database object
		public void SaveTables(){

			recordset.SaveRecordset (skinsList, _skinTable);
			recordset.SaveRecordset (materialList, _materialTable);

		}

		#region "Skin"

		//** AddSkin **
		//Insert new skin
		public int AddSkin(string name, int idEye){

			int toReturn = 1;

			if (skinsList.Count > 0)
				toReturn = skinsList.OrderBy (x => x.IDSkin).Last ().IDSkin + 1;

			EyeSkin toAdd = new EyeSkin ();

			toAdd.IDSkin = toReturn;
			toAdd.IDEye = idEye;
			toAdd.SkinName = name;

			skinsList.Add (toAdd);

			return toReturn;
		}


		//** DeleteSkin **
		//Delete a skin
		public void DeleteSkin(int idSkin){

			//Remove the skin info
			skinsList.RemoveAll (x => x.IDSkin == idSkin);

			//Remove the materials
			materialList.RemoveAll (x => x.IDEyeSkin == idSkin);

		}


		//** EditSkin **
		// Update skin information
		public void EditSkin(int idSkin, int idEye, string name){

			EyeSkin toEdit = new EyeSkin ();

			toEdit = skinsList.Find (x => x.IDSkin == idSkin);

			toEdit.IDEye = idEye;
			toEdit.SkinName = name;

		}


		//** GetSkinList **
		//Retrieve the list of skins
		public List<EyeSkin> GetSkinList(bool onlyCopy = false){

			List<EyeSkin> toReturn = new List<EyeSkin> ();

			if (!onlyCopy) {
				toReturn = skinsList;
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				toReturn.AddRange (skinsList);
			}

			return toReturn;

		}


		//** LoadSkinsTable **
		//Load records list from database object
		public void LoadSkinsTable(){

			if (!skinsTableLoaded) skinsList = recordset.GetRecordset<EyeSkin>(_skinTable);

			skinsTableLoaded = true;

		}


		//** UnloadSkinsTable **
		//Remove the list from memory, unloading it
		public void UnloadSkinsTable(){

			skinsList.Clear ();

			skinsTableLoaded = false;

		}

		#endregion

		#region "Material"

		//** AddMaterial **
		//Insert new material
		public int AddMaterial(int idSkin, int idObject, string name){

			int toReturn = 1;

			List<EyeMaterial> tList = materialList.Where(x => x.IDEyeSkin == idSkin).ToList();
				
			if (tList.Count > 0)
				toReturn = tList.OrderBy(x => x.ID).Last().ID + 1;

			EyeMaterial toAdd = new EyeMaterial ();
			toAdd.ID = toReturn;
			toAdd.IDEyeSkin = idSkin;
			toAdd.IDObject = idObject;
			toAdd.MaterialName = name;

			materialList.Add (toAdd);

			return toReturn;

		}


		//** DeleteEyeMaterials **
		//Delete all material from the skin
		public void DeleteEyeMaterials(int idEyeSkin){

			materialList.RemoveAll (x => x.IDEyeSkin == idEyeSkin);

		}


		//** UpdateMaterial **
		//Update material information
		public void UpdateMaterial(int id, int idEyeSkin, string name){

			materialList.Find (x => x.ID == id && x.IDEyeSkin == idEyeSkin).MaterialName = name;

		}


		//** GetMaterialList **
		//Retrieve the list of materials from a skin
		public List<EyeMaterial> GetMaterialList (int idEyeSkin, bool onlyCopy = false){

			List<EyeMaterial> toReturn = new List<EyeMaterial> ();

			if (!onlyCopy) {
				toReturn = materialList.Where (x => x.IDEyeSkin == idEyeSkin).ToList ();
			} else {
				//Retrieve a copy of the list. Changes made to the variable that holds
				//the coppy will not affect the original one
				toReturn.AddRange(materialList.Where (x => x.IDEyeSkin == idEyeSkin).ToList ());
			}
			return toReturn;

		}


		//** LoadMaterialList **
		//Load the material list from the database object
		public void LoadMaterialList(){

			if (!materialTableLoaded) materialList = recordset.GetRecordset<EyeMaterial>(_materialTable);

			materialTableLoaded = true;

		}

		public void UnloadMaterialList(){

			materialList.Clear ();

			materialTableLoaded = false;

		}
		#endregion
	}
}
