using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    public float moveSpeed = 18.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 9.8f + (9.8f/2.0f);
    public float verticalSpeed = 0.0f;

    //Will delete if game manager is made
    TimePeriod timePeriod;
    ScreenFade screenFade;
    SceneChange sceneChange;

    //Initial variable to control score
    public int score;

    //Variables to control powerups
    public float speedTimer;
    public bool hasSpeed;
    public bool hasShield;
    public bool hasLetter1;
    public bool hasLetter2;
    public bool hasLetter3;

    public float damageTimer;
    float rotationTimer = 0.0f;

    //Current direction that procedural generation is doing
    public float roadRotation = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Please Work");
        controller = GetComponent<CharacterController>();

        //Initialising variables to do with powerups
        speedTimer = 0.0f;
        hasShield = false;
        hasLetter1 = false;
        hasLetter2 = false;
        hasLetter3 = false;

        score = 0;

        timePeriod = GameObject.Find("TimePeriodManager").GetComponent<TimePeriod>();
        screenFade = GameObject.Find("Screen Fade").GetComponent<ScreenFade>();
        sceneChange = GameObject.Find("SceneManager").GetComponent<SceneChange>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get Axes input
        //Horizontal input controls x position
        //Vertical Input controls vertical
        Vector3 inputVector;
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = 0.0f;
        inputVector.z = Input.GetAxis("Vertical");

        Vector3 right = new Vector3(transform.right.x, 0.0f, transform.right.z).normalized;
        Vector3 forward = new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized;

        Vector3 playerMovement = ((inputVector.x * right) + (inputVector.z * forward)) * moveSpeed;

        if (controller.isGrounded)
        {
            //Vertical speed set to 0 when on the ground
            verticalSpeed = 0.0f;
            //Player can only jump when on the ground
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalSpeed = jumpSpeed;
            }
        }
        //Reduce players vertical movement speed by gravity over time when not grounded
        verticalSpeed -= gravity * Time.deltaTime;
        playerMovement.y = verticalSpeed;
        //Apply movemnet to controller
        controller.Move(playerMovement * Time.deltaTime);

        //Start a timer for changing the speed of the player if they picked up the powerup
        if(speedTimer > 0.0f)
        {
            moveSpeed = 25.0f;
            speedTimer -= Time.deltaTime;
        }
        else
        {
            moveSpeed = 15.0f;
        }

        //If the player has all the letter powerups then add a lot to their score
        if(hasLetter1 && hasLetter2 && hasLetter3)
        {
            score += 2000;
            hasLetter1 = false;
            hasLetter2 = false;
            hasLetter3 = false;
        }

        //Have a timer for when your speed goes back to normal
        if(damageTimer > 0.0f)
        {
            damageTimer -= Time.deltaTime;
        }
        else
        {
            moveSpeed = 15.0f;
        }

        rotationTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            //Can change these over to an enum if needed
            case "Speed": speedTimer = 5.0f; break;
            case "Shield": hasShield = true; break;
            case "Time": 
                if(timePeriod.GetTimePeriod() == Period.FUTURE)
                {
                    timePeriod.SetTimePeriod(Period.PRESENT);
                }
                else if (timePeriod.GetTimePeriod() == Period.PRESENT)
                {
                    timePeriod.SetTimePeriod(Period.PAST);
                }
                screenFade.FadeToWhite();
                screenFade.FadeIn();
                break;
            case "monster": sceneChange.GameOver(); break;
            case "letter1":  hasLetter1 = true; break;
            case "letter2": hasLetter2 = true; break;
            case "letter3": hasLetter3 = true; break;
            case "Candy": score += 10; break;
            case "Hazard":
                Hazard h = other.GetComponent<Hazard>();

                if (h.type == HazardType.SLOW)
                {
                    takeDamage();
                }
                break;
                
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject collider = hit.gameObject;

        if (collider.tag == "Curved")
        {
            if (rotationTimer >= 5.0f)
            {
                Debug.Log("Collided with curved road");
                Quaternion q = transform.rotation;
                Quaternion newQ = Quaternion.Euler(q.eulerAngles.x, roadRotation, q.eulerAngles.z);
                transform.rotation = newQ;
                rotationTimer = 0.0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Get the object we are colliding with
        GameObject collider = collision.gameObject;
        
        //if(transform.position.y > collider.transform.position.y)
        //{
        //    //Set the players transform to follow whatever you are colliding with, except on the y-axis
        //    transform.position = new Vector3(collider.transform.position.x, transform.position.y, collider.transform.position.z);
        //}
        //else
        //{
        //    //Take damage/slow down
        //    takeDamage();
        //}

    }

    //basic function for calculating what happens when colliding with a hazard
    //Speed is lowered to just below the monsters so that you do get punished for running into something
    void takeDamage()
    {
        if(!hasShield)
        {
            moveSpeed = 7.0f;
            damageTimer = 2.5f;
        }
        else
        {
            hasShield = false;
        }
    }

}
