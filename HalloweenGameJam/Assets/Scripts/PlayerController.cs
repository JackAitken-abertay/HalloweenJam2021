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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Please Work");
        controller = GetComponent<CharacterController>();
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

    }
}
