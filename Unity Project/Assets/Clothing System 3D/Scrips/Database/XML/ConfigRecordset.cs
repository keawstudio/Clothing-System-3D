// ********************************
//  ConfigRecordset
//
//  Save and restore config data in a XML file
//	The file is stored in the Resources folder
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

namespace ClothingSystemDBXML{

	public class ConfigRecordset {

		string tableName = "csconfig.xml";


		//** GetRecordset **
		public List<T> GetRecordset<T> (){

			string path = Application.dataPath+"/Resources/"+tableName;

			List<T> toReturn = new List<T> ();

			if (File.Exists (path)) {

				XmlSerializer reader = new XmlSerializer (typeof(List<T>));

				StreamReader file = new StreamReader (path);

				toReturn = (List<T>)reader.Deserialize (file);

				file.Close ();
			}

			return toReturn;

		}


		//** SaveRecordset **
		public void SaveRecordset<T>(List<T> recToSave){

			string path = Application.dataPath+"/Resources/"+tableName;

			XmlSerializer writer = new XmlSerializer (typeof(List<T>));

			FileStream file = File.Create (path);

			writer.Serialize (file, recToSave);

			file.Close ();

		}

	}
}
