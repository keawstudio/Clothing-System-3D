// ********************************
//  Recordset
//
//  Save and retrieve data im XML files
//
//  Clothing System 3D
//  2016 - Larissa Redeker
//  Kittens and Elves at Work
//  http://www.keawstudio.com
//
// ********************************

using UnityEngine;
using System.Collections.Generic;
using ClothingSystem3D;
using System.Xml.Serialization;
using System.IO;

namespace ClothingSystemDBXML{		//the name of the workspace you want your class belongs to

	public class Recordset {

		//** GetRecordset **
		// Load a list from a XML file
		public List<T> GetRecordset<T> (string tableName){

			CSConfig _config = new CSConfig();

			//List to be returned
			List<T> toReturn = new List<T> ();

			//Create the path to file
			string path = _config.GetFullPath (CSConfig.PathTypeEnum.DatabasePath)+"/"+tableName+".xml";

			if (File.Exists (path)) {

				XmlSerializer reader = new XmlSerializer (typeof(List<T>));

				StreamReader file = new StreamReader (path);

				toReturn = (List<T>)reader.Deserialize (file);

				file.Close ();
			}

			return toReturn;

		}


		//** SaveRecordset **
		//Save a list to a XML file
		public void SaveRecordset<T>(List<T> recToSave, string tableName){

			CSConfig _config = new CSConfig();

			string path = _config.GetFullPath (CSConfig.PathTypeEnum.DatabasePath)+"/"+tableName+".xml";

			XmlSerializer writer = new XmlSerializer (typeof(List<T>));

			FileStream file = File.Create (path);

			writer.Serialize (file, recToSave);

			file.Close ();

		}

	}
}
