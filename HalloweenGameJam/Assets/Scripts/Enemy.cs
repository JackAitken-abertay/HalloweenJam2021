using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] GameObject playerObject;
    PlayerController player;

    public float defaultSpeed = 8.0f;
    public float boostSpeed = 15.0f;
    public float currentSpeed;
    public float speedUpDistance = 75.0f;
    float constantYPos;
    private float soundTimer;

    // Start is called before the first frame update
    void Start()
    {
        //Get reference to player
        player = playerObject.GetComponent<PlayerController>();
        currentSpeed = defaultSpeed;
        controller = GetComponent<CharacterController>();
        constantYPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = transform.forward * currentSpeed;
        controller.Move(movement * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distanceToPlayer > speedUpDistance)
        {
            currentSpeed = boostSpeed;
        }

        if (distanceToPlayer < speedUpDistance)
        {
            currentSpeed = defaultSpeed;
        }

        soundTimer += Time.deltaTime;

        if(soundTimer > 10.0f)
        {
            GetComponent<AudioSource>().Play();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject collider = hit.gameObject;

        if (collider.tag == "Curved")
        {
            Debug.Log("Enemy Collided with curved road");
            transform.Rotate(0.0f, 90.0f, 0.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Collectable")
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Obstacle")
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Curved")
        {
            transform.Rotate(0.0f, 90.0f, 0.0f);
        }
    }
}
