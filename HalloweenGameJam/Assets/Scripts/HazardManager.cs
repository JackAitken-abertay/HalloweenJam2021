using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
    //List of hazards to potentially spawn
    [SerializeField] protected GameObject[] futureHazards;
    [SerializeField] protected GameObject[] presentHazards;
    [SerializeField] protected GameObject[] pastHazards;

    //Variables for the max and min positions to spawn an object on.
    //Want to keep as variables so that if we use turns they arent spawning outside the turns
    private float spawnMin;
    private float spawnMax;

    //TO DELETE
    float spawnTimer;

    //Instance of the class needed for altering the time period
    TimePeriod timePeriod;

    private void Awake()
    {
        timePeriod = GameObject.FindObjectOfType<TimePeriod>().GetComponent<TimePeriod>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Will need access to a time period manager - do that here
        //timePeriod = GameObject.FindObjectOfType<TimePeriod>().GetComponent<TimePeriod>();

        //May also need access to the proc gen class Michael has - also do that here
    }

    // Update is called once per frame
    void Update()
    {
        //Set as a timer for now but will update to be called whenever I need it
       //spawnTimer += Time.deltaTime;

        //if(spawnTimer >= 5.0f)
        //{
        //    spawnHazards(new Vector3(0, 0, 0), 3);
        //    spawnTimer = 0.0f;
        //}

        if(Input.GetKeyDown(KeyCode.A))
        {
            timePeriod.SetTimePeriod(Period.PAST);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            timePeriod.SetTimePeriod(Period.PRESENT);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            timePeriod.SetTimePeriod(Period.FUTURE);
        }
    }

    //Function for spawning the hazards
    public void spawnHazards(Vector3 objectPosition, int howManyTimes)
    {
        Vector3 spawnPosition;
        
        //Assigning values for a max and min spawn area
        spawnMax = objectPosition.x + 5.0f;
        spawnMin = objectPosition.x - 5.0f;

        for (int i = 0; i < howManyTimes; i++)
        {
            spawnPosition = new Vector3(Random.Range(spawnMin, spawnMax), objectPosition.y, Random.Range(spawnMin, spawnMax));

            //Switch statement used to spawn the appropriate objects
            switch (timePeriod.GetTimePeriod())
            {
                case Period.FUTURE: Instantiate(futureHazards[Random.Range(0, 3)], spawnPosition, Quaternion.identity); break;
                case Period.PRESENT: Instantiate(presentHazards[Random.Range(0, 3)], spawnPosition, Quaternion.identity); break;
                case Period.PAST: Instantiate(pastHazards[Random.Range(0, 3)], spawnPosition, Quaternion.identity); break;
            }
        }
    }

    public void spawnHazardsArray(Vector3[] objectPositions, int howManyTimes)
    {
        for(int i = 0; i < howManyTimes; i++)
        {
            for (int j = 0; j < objectPositions.Length; j++)
            {
                Instantiate(futureHazards[0], objectPositions[j], Quaternion.identity);
            }
        }
    }
}
