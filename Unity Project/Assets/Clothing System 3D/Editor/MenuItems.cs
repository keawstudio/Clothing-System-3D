using UnityEngine;
using UnityEditor;

namespace ClothingSystem3D {

	public static class MenuItems {

		const string _menuName = "Clothing System 3D";

		[MenuItem(_menuName+"/Body Types Table")]
		private static void ShowBodyTypeWindow(){
			BodyTypeWindow.ShowBodyTypeWindow ();
		}

		[MenuItem(_menuName+"/Body Subtypes Table")]
		private static void ShowBodySubtypeWindow(){
			BodySubtypeWindow.ShowBodySubtypeWindow ();
		}

		[MenuItem(_menuName+"/Body Structure Table")]
		private static void ShowStructureWindow(){
			BodyStructureWindow.ShowBodyStructureWindow ();
		}

		[MenuItem(_menuName+"/Body Models Table")]
		private static void ShowModelsWindow(){
			BodyModelWindow.ShowBodyModelWindow();
		}

		[MenuItem(_menuName+"/Skins Table")]
		private static void ShowSkinsWindow(){
			SkinsWindow.ShowSkinsWindow ();
		}

		[MenuItem(_menuName+"/Clothes Table")]
		private static void ShowClothesWindow(){
			ClothesWindow.ShowClothesWindow ();
		}

		[MenuItem(_menuName+"/Props Table")]
		private static void ShowPropsWindow(){
			PropsWindow.ShowPropsWindow ();
		}

		[MenuItem(_menuName+"/Eyes")]
		private static void ShowEyesWindow(){
			EyesWindow.ShowEyesWindow();
		}

		[MenuItem(_menuName+"/Eyes Skins")]
		private static void ShowEyeSkinsWindow(){
			EyesSkinWindow.ShowEyesSkinWindow ();
		}

		[MenuItem (_menuName+"/Configuration")]
		private static void Config(){
			ConfigWindow.ShowConfigWindow ();
		}

	}
}
