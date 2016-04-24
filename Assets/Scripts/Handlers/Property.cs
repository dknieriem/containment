using UnityEngine;
using System.Collections;

public abstract class Property
{

	public string Name;

	public abstract void SetValue (System.Object newValue);

	public abstract System.Object GetValue ();

	public abstract System.Type GetPropType ();

	public abstract string GetPropTypeString ();
}

public class StringProperty : Property
{
	string Value;

	public StringProperty (string name, string defaultValue)
	{
		Name = name;
		Value = defaultValue;
	}

	public StringProperty (string name)
	{
		Name = name;
		Value = "";
	}

	public override void SetValue (System.Object newValue)
	{
		if (newValue.GetType () != typeof(string))
			Value = "";
		else
			Value = (string)newValue;

		Debug.Log (this.Name + ": " + Value);
	}

	public override System.Object GetValue ()
	{
		return Value as System.Object;
	}

	public override System.Type GetPropType ()
	{
		return typeof(string);
	}

	public override string GetPropTypeString ()
	{
		return "System.String";
	}
}

public class IntegerProperty : Property
{
	int Value;
	int minValue, maxValue;
	bool ValueRestricted;

	public IntegerProperty (string name)
	{
		Name = name;
		ValueRestricted = false;
		Value = 0;
		minValue = 0;
		maxValue = 0;
	}

	public IntegerProperty (string name, int DefaultValue)
	{
		Name = name;
		ValueRestricted = false;
		Value = DefaultValue;
		minValue = 0;
		maxValue = 0;
	}

	public IntegerProperty (string name, int DefaultValue, int MinValue, int MaxValue)
	{
		Name = name;
		ValueRestricted = true;
		Value = DefaultValue;
		minValue = MinValue;
		maxValue = MaxValue;
	}

	public IntegerProperty (string name, int MinValue, int MaxValue)
	{
		Name = name;
		ValueRestricted = true;
		Value = 0;
		minValue = MinValue;
		maxValue = MaxValue;
	}

	public override void SetValue (System.Object newValue)
	{
		if (newValue.GetType () != typeof(int)) {
			Value = 0;
			return;
		}
		int newInt = (int)newValue;
		if (ValueRestricted) {
			if (newInt >= minValue && newInt <= maxValue)
				Value = newInt;
		} else {
			Value = newInt;
		}

		Debug.Log (this.Name + ": " + Value);
	}

	public override System.Object GetValue ()
	{
		return Value as System.Object;
	}

	public override System.Type GetPropType ()
	{
		return typeof(int);
	}

	public override string GetPropTypeString ()
	{
		return "System.Int32";
	}
}

public class BoolProperty : Property
{
	bool Value;

	public BoolProperty (string name)
	{
		Name = name;
	}

	public override void SetValue (System.Object newValue)
	{
		if (newValue.GetType () != typeof(bool))
			Value = false;
		else
			Value = (bool)newValue;

		Debug.Log (this.Name + ": " + Value);
	}

	public override System.Object GetValue ()
	{
		return Value as System.Object;
	}

	public override System.Type GetPropType ()
	{
		return typeof(bool);
	}

	public override string GetPropTypeString ()
	{
		return "System.Boolean";
	}
}

public class EnumProperty : Property
{
	public string[] possibleValues;
	int enumValue;
	public int min = 0;
	public int max;

	public EnumProperty (string[] possibles, int defaultValue)
	{
		possibleValues = possibles;
		enumValue = defaultValue;
		max = possibles.Length - 1;
	}

	public override void SetValue (System.Object newValue)
	{

	}

	public override System.Object GetValue ()
	{
		return enumValue as System.Object;
	}

	public override System.Type GetPropType ()
	{
		return typeof(System.Enum);
	}

	public override string GetPropTypeString ()
	{
		return "System.Enum";
	}
}