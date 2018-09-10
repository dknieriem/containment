using UnityEngine;
using System.Collections;

public interface PersonMission
{
Sector MissionTargetSector;
Person ThePerson;

void AssignMission(Person person);

void DoUpdate();

void DoLogic();
}