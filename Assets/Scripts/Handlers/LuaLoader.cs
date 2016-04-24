using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Linq;

public class LuaLoader : MonoBehaviour
{

	Dictionary<string,string> scripts;

	public void OnStart ()
	{

		scripts = new Dictionary<string, string> ();

		object[] result = Resources.LoadAll ("Lua", typeof(TextAsset));

		foreach (TextAsset ta in result.OfType<TextAsset>()) {
			scripts.Add (ta.name, ta.text);
			Debug.Log (ta.name);
			Script.RunString (ta.text);
		}


	}

	public string GetScript (string ScriptName)
	{
		string output;

		scripts.TryGetValue (ScriptName, out output);
	
		return output;
	}

}