using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLog : MonoBehaviour {

    public List<GameEvent> Log;


    public void Add(GameEvent newEvent)
    {
        Log.Add(newEvent);
    }


}
