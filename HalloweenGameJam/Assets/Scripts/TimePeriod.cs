using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Period
{
    FUTURE, PRESENT, PAST
}

public class TimePeriod : MonoBehaviour
{
    private Period period;

    // Start is called before the first frame update
    void Start()
    {
        //Initially set up the time period as the future
        period = Period.FUTURE; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Period GetTimePeriod()
    {
        return period;
    }

    public void SetTimePeriod(Period periodPassedIn)
    {
        period = periodPassedIn;
    }
}
