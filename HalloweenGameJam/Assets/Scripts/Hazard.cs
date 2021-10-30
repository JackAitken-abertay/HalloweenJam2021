using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum  HazardType
{
    SLOW = 0,
    KILL = 1
}
public class Hazard : MonoBehaviour
{
    public bool isMovingHazard = false;
    public float movementSpeed = 3.0f;
    public Vector3 maxDistance = Vector3.zero;
    Vector3 origin;
    Vector3 newPosition;
    public HazardType type = HazardType.SLOW;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        newPosition = origin;
    }

    // Update is called once per frame
    void Update()
    {
        newPosition = origin + (maxDistance * Mathf.Sin(Time.time * movementSpeed));
        transform.position = newPosition;
    }
}
