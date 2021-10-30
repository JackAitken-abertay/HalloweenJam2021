using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [SerializeField] protected GameObject[] futurePowerups;
    [SerializeField] protected GameObject[] presentPowerups;
    [SerializeField] protected GameObject[] pastPowerups;

    //Variables for the max and min positions to spawn an object on.
    //Want to keep as variables so that if we use turns they arent spawning outside the turns
    private float spawnMin;
    private float spawnMax;

    float timer;

    //Instance of the class needed for altering the time period
    TimePeriod timePeriod;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        //Will need access to a time period manager - do that here
        timePeriod = GameObject.Find("TimePeriodManager").GetComponent<TimePeriod>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 3.0f)
        {
            spawnPowerups(new Vector3(0, 0, 0), 2);
            timer = 0;
        }
    }

    //Function for spawning the hazards
    public void spawnPowerups(Vector3 objectPosition, int howManyTimes)
    {
        //Position will be object position in the final product
        spawnMax = objectPosition.x + 5.0f;
        spawnMin = objectPosition.x - 5.0f;

        for (int i = 0; i < howManyTimes; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(spawnMin, spawnMax), objectPosition.y, Random.Range(spawnMin, spawnMax));
            
            //Switch statement used to spawn the appropriate objects
            switch (timePeriod.GetTimePeriod())
            {
                case Period.FUTURE: Instantiate(futurePowerups[Random.Range(0, 3)], spawnPosition, Quaternion.identity); break;
                case Period.PRESENT: Instantiate(presentPowerups[Random.Range(0, 3)], spawnPosition, Quaternion.identity); break;
                case Period.PAST: Instantiate(pastPowerups[Random.Range(0, 3)], spawnPosition, Quaternion.identity); break;
            }
        }
    }
}
