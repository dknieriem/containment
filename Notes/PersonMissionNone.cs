using UnityEngine;
using System.Collections;

public class PersonMissionNone : PersonMission
{

Sector MissionTargetSector;
Person ThePerson;

void AssignMission(Person person){
ThePerson = person;
ThePerson.CurrentState = Person.CharacterState.Idle;
//TODO: ThePerson.ActionQueue = ActionQueue.Empty
}

void DoUpdate(){


}

void DoLogic(){
//TODO: If Zeds around, try to hide in safe house or cleared building or uncleared building, or run to safehouse sector.Last resort = fight?
}
}