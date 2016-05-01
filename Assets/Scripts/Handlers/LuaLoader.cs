using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Linq;

public class LuaLoader : MonoBehaviour
{

	public Dictionary<string,string> scripts;
	//	Script scriptOfData;

	public void Start ()
	{
		//Debug.Log ("LuaLoader.Start()");
		//scriptOfData = new Script ();
		scripts = new Dictionary<string, string> ();

		object[] result = Resources.LoadAll ("MoonSharp/Scripts", typeof(TextAsset));
		//Debug.Log (result.Length + " Lua TextAssets found");
		//Debug.Log ("Time: " + Time.unscaledTime);
		foreach (TextAsset ta in result.OfType<TextAsset>()) {
			scripts.Add (ta.name, ta.text);
			//	Debug.Log ("LoaLoader loaded script: " + ta.name);

			DynValue mfn = Script.RunString (ta.text);
			//	Debug.Log (mfn.Table [1]);
			switch (ta.name) {

			case "data.names.first.male":
				Person.DataNamesFirstMale = mfn.ToObject<string[]> ();
				break;
			case "data.names.first.female":
				Person.DataNamesFirstFemale = mfn.ToObject<string[]> ();
				break;
			case "data.names.last":
				Person.DataNamesLast = mfn.ToObject<string[]> ();
				break;
			case "data.world.sizes":
				World.WorldSizes = mfn.ToObject<Dictionary<int,int>> ();
				break;
			}
		}
		//Debug.Log ("Time: " + Time.unscaledTime);
//		Debug.Log ("World.WorldSizes: " + World.WorldSizes.ToString ());
	}

	public string GetScript (string ScriptName)
	{
		string output = "";

		scripts.TryGetValue (ScriptName, out output);
	
		return output;
	}

}