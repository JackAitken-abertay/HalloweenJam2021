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

    public int score;

    //Variables to control powerups
    public float speedTimer;
    public bool hasSpeed;
    public bool hasShield;
    public bool hasLetter1;
    public bool hasLetter2;
    public bool hasLetter3;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Please Work");
        controller = GetComponent<CharacterController>();

        //Initialising variables to do with powerups
        speedTimer = 0.0f;
        hasSpeed = false;
        hasShield = false;
        hasLetter1 = false;
        hasLetter2 = false;
        hasLetter3 = false;

        score = 0;
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

        Vector3 playerMovement = inputVector * moveSpeed;

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

        //Start a timer for their speed if the player has it
        if(hasSpeed && speedTimer > 0.0f)
        {
            speedTimer -= Time.deltaTime;
        }

        //If the player has all the letter powerups then add a lot to their score
        if(hasLetter1 && hasLetter2 && hasLetter3)
        {
            score += 2000;
            hasLetter1 = false;
            hasLetter2 = false;
            hasLetter3 = false;
        }

        timePeriod = GameObject.Find("TimePeriodManager").GetComponent<TimePeriod>();
        screenFade = GameObject.Find("Screen Fade").GetComponent<ScreenFade>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Speed": moveSpeed = 25.0f; hasSpeed = true; speedTimer = 5.0f; break;
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
            case "monster": //Change the scene to the game over
                break;
            case "letter1":  hasLetter1 = true;
                break;
            case "letter2": hasLetter2 = true; break;
            case "letter3": hasLetter3 = true; break;
            case "Candy": score += 10; break;
        }
    }
}
