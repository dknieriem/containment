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

            Script mfn = new Script();
            mfn.DoString(ta.text);
            DynValue nameFn = mfn.Globals.Get("Name");
            DynValue name = mfn.Call(nameFn);

            DynValue dataFn = mfn.Globals.Get("ReturnData");
			Debug.Log (name.String);
			switch (name.String){//(ta.name) {

			case "data.names.first.male":
                Person.DataNamesFirstMale = mfn.Call(dataFn).ToObject<string[]>();//ToObject<string[]> ();
				break;
			case "data.names.first.female":
				Person.DataNamesFirstFemale = mfn.Call(dataFn).ToObject<string[]>(); //mfn.ToObject<string[]> ();
				break;
			case "data.names.last":
				Person.DataNamesLast = mfn.Call(dataFn).ToObject<string[]>(); //mfn.ToObject<string[]> ();
                break;
			case "data.world.sizes":
                    World.WorldSizes = mfn.Call(dataFn).ToObject<Dictionary<int, int>>(); //mfn.ToObject<Dictionary<int,int>> ();
                break;
             case "Relationship.modifierPrototypes":
                Relationship.AddModifierPrototype(mfn.Call(dataFn).ToObject<RelationshipModifier>());
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