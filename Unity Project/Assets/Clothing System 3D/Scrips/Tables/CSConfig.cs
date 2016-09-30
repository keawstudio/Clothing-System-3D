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
using System.IO;
using DBAccess = ClothingSystemDBXML;

namespace ClothingSystem3D {

	public class CSConfig {

		public enum ClothingMethodEnum {
			MeshSubstitution,
			FullObject
		}

		public enum PathTypeEnum {
			DatabasePath,
			ClothPath,
			PropPath,
			SkinsPath,
			BodyModelsPath,
			FolderPrefix
		}

		public class PathInfo {
			public string PathName = "";
			public PathTypeEnum PathType;
		}

		List<PathInfo> Paths = new List<PathInfo>();

		DBAccess.ConfigRecordset recordset = new DBAccess.ConfigRecordset();

		#region "Config functions"

		public string GetPathName(PathTypeEnum pathType){

			string pathRetrieve = Paths.Find (x => x.PathType == pathType).PathName;

			return pathRetrieve;

		}

		public string GetFullPath(PathTypeEnum pathType){

			string path = "";

			string projectFolderPrefix = Paths.Find (x => x.PathType == PathTypeEnum.FolderPrefix).PathName;

			path = Application.dataPath + "/";

			if (projectFolderPrefix.Length > 0)
				path += projectFolderPrefix + "/";

			path += "Resources/"+GetPathName(pathType);

			return path;
		}

		public string GetProjectPath(PathTypeEnum pathType){

			string path = "";

			string projectFolderPrefix = Paths.Find (x => x.PathType == PathTypeEnum.FolderPrefix).PathName;

			if (projectFolderPrefix.Length > 0)
				path += projectFolderPrefix + "/";

			path += "Resources/"+GetPathName(pathType);

			return path;
		}

		public string GetPrefabPath(PathTypeEnum pathType, string path){

			List<PathInfo> tempPaths = Paths.FindAll (x => x.PathType == pathType);

			int pathIndex = 0;

			for (int i = 0; i < tempPaths.Count; i++) {

				pathIndex = path.IndexOf (tempPaths [i].PathName);

				if (pathIndex > -1) {
					//remove the path from the asset path
					path = path.Remove (0, pathIndex + tempPaths [i].PathName.Length + 1);
				}

				pathIndex = path.IndexOf (".");

				if (pathIndex > -1) {
					path = path.Remove (pathIndex);
				}

			}

			return path;
		}
		#endregion

		#region "ConfigTable"
		public CSConfig(){

			//Check if Resources folder at root exists, if not, create it

			string resFolder = Application.dataPath + "/Resources";

			if (!Directory.Exists (resFolder)){
				Debug.LogWarning("A Resources folder need to exist in the root of the Assets folder. Creating it now...");
				Directory.CreateDirectory (resFolder);
			}

			LoadTable ();

			if (Paths.Count < 6) {

				Paths = new List<PathInfo> () {
					new PathInfo {PathName = "Database", PathType = PathTypeEnum.DatabasePath },
					new PathInfo {PathName = "Clothes", PathType = PathTypeEnum.ClothPath },
					new PathInfo {PathName = "Props", PathType = PathTypeEnum.PropPath },
					new PathInfo {PathName = "Skins", PathType = PathTypeEnum.SkinsPath},
					new PathInfo {PathName = "BodyModels", PathType = PathTypeEnum.BodyModelsPath},
					new PathInfo {PathName = "Clothing System 3D", PathType = PathTypeEnum.FolderPrefix},
				};

				SaveTable ();
			}

		}

		public void LoadTable(){

			Paths = recordset.GetRecordset<PathInfo>();

		}

		public void SaveTable(){

			recordset.SaveRecordset (Paths);

		}

		public List<PathInfo> GetList(){

			return Paths;

		}

		#endregion

	}
}
