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

    public float damageTimer;
    float rotationTimer = 0.0f;

    //Current direction that procedural generation is doing
    public float roadRotation = 0.0f;
    public Vector2Int roadDirection = new Vector2Int(0,-1);

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        timePeriod = GameObject.Find("TimePeriodManager").GetComponent<TimePeriod>();
        screenFade = GameObject.Find("Screen Fade").GetComponent<ScreenFade>();
        sceneChange = GameObject.Find("SceneManager").GetComponent<SceneChange>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Please Work");

        //Initialising variables to do with powerups
        speedTimer = 0.0f;
        hasShield = false;

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
            speedTimer -= Time.deltaTime;
        }
        else
        {
            if(damageTimer <= 0.0f)
            {
                moveSpeed = 15.0f;
            }
        }

        //Have a timer for when your speed goes back to normal
        if(damageTimer > 0.0f)
        {
            damageTimer -= Time.deltaTime;
        }
        else
        {
            if (speedTimer <= 0.0f)
            {
                moveSpeed = 15.0f;
            }
        }

        rotationTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
          switch (other.tag)
        {
            //Can change these over to an enum if needed
            case "Speed": speedTimer = 5.0f; moveSpeed = 25.0f; break;
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
                fadeToWhite();
                screenFade.FadeIn();
                break;
            case "monster": sceneChange.GameOver(); break;
            case "Candy": score += 10; break;
            case "Pumpkin": score += 30; break;
            case "Destroyable":
                Hazard h = other.GetComponent<Hazard>();

                if (h.type == HazardType.SLOW)
                {
                    takeDamage();
                }
                break;
        }

        Destroy(other.gameObject);
        Debug.Log("Powerup Destroyed");
    }

    IEnumerator fadeToWhite()
    {
        screenFade.FadeToWhite();
        yield return new WaitForSeconds(2.0f);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject collider = hit.gameObject;

        if (collider.tag == "Curved")
        {
            if (rotationTimer >= 5.0f)
            {
                Debug.Log("Collided with curved road");
                if (roadDirection == new Vector2Int(1, 0))
                {
                    //Turning to the right
                    roadRotation = -90.0f;
                }
                if (roadDirection == new Vector2Int(-1, 0))
                {
                    roadRotation = 90.0f;
                }
                if (roadDirection == new Vector2Int(0, -1))
                {
                    roadRotation = 0.0f;
                }
                if (roadDirection == new Vector2Int(0, 1))
                {
                    roadRotation = 180.0f;
                }

                Quaternion q = transform.rotation;
                q = Quaternion.Euler(q.eulerAngles.x, roadRotation, q.eulerAngles.z);
                transform.rotation = q;
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
