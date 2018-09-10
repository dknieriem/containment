using UnityEngine;
using System.Collections;

public partial class Person
{
	
	public enum CharacterLocation
	{
		Outside = 0,
		UnclearedBuilding,
		ClearedBuilding,
		Safehouse}
	;

	public enum Role
	{
		None = 0,
		Patient,
		Doctor,
		Builder,
		Guard,
		Scout,
		Looter}
	;

	public static string[] RoleNames = { "None", "Patient", "Doctor", "Builder", "Guard", "Scout", "Looter" };

	public enum CharacterState
	{
		Dead = 0,
		Resting,
		//required sleep
		Recovering,
		//healing from wounds
		Idle,
		//not assigned a job
		Repairing,
		//repairing or building defenses
		Defending,
		//defending a safe house
		Hunting,
		//actively hunting zed in the sector
		Scouting,
		//scouting buildings in the sector, discovering safety values and lootable contents
		Looting}
		//looting buildings in the sector
	;

	public enum Skill
	{
		MeleeStrength = 0,
		FirearmStrength,
		AmmoHeld,
		Doctor,
		Agility,
		Construction,
		Stamina,
		Leadership}
	;

	public static string[] SkillNames = {
		"Melee",
		"Firearm",
		"Ammo",
		"Doctor",
		"Agility",
		"Construction",
		"Stamina",
		"Leadership"
	};

	public enum Stats
	{
		Health = 0,
		Stamina,
		//current stamina. when < 10%, character should seek rest. at 0, will pass out.
		MentalStability,
		KillRate,
		//average number of Zeds per hour killed, modified by melee weapon strength and stamina.
		BuildRate,
		//rate of building and repairing defenses
		InjuryRate}
		//rate of this person's health loss
	;

	public static string[] StatNames = {
		"Health",
		"Stamina",
		"Mental Stability",
		"Kill Rate",
		"Build Rate",
		"Injury Rate"
	};
	
}