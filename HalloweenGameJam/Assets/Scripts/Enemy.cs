using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject playerObject;
    GameObject EnemyIn;

    public float defaultSpeed = 8.0f;
    public float boostSpeed = 15.0f;
    public float currentSpeed;
    public float speedUpDistance = 20.0f;
    float constantYPos;
    private float soundTimer;
    public float roadRotation = 0.0f;
    public static GameObject EnemyRef;

    public static int EnemiesCSectionIndex = 2;
    public Vector2Int CDir = new Vector2Int(0, -1);

    public Vector2Int roadDirection = new Vector2Int(0, -1);

    public static float ChunkSize;

    Vector3 TargetPos;

    Vector2Int NextChunk;
    Vector2Int CurrentChunk;

    HashSet<string> DestroyStrings = new HashSet<string>()
    {
        "Time",
        "Speed",
        "Shield",
        "Pumpkin",
        "Candy",
        "Destroyable",
    };

    void SetNextChunk()
    {
        NextChunk = ProcGen.LoadedSections[EnemiesCSectionIndex - 1].ChunkPos;

        CDir = NextChunk - CurrentChunk;

        TargetPos = new Vector3(NextChunk.x * ChunkSize, 1.0f, NextChunk.y * ChunkSize);
    }

    public void PassedTrigger()
    {
       
        CurrentChunk += CDir;
        EnemiesCSectionIndex--;

        SetNextChunk();

        this.transform.rotation = CDir.x != 0 ? (CDir.x == 1 ? Quaternion.Euler(0.0f, 90.0f, 0.0f) : Quaternion.Euler(0.0f, -90.0f, 0.0f)) : (CDir.y == 1 ? Quaternion.Euler(0.0f, 0.0f, 0.0f) : Quaternion.Euler(0.0f, 180.0f, 0.0f));
    }

    private void Start()
    {
        currentSpeed = defaultSpeed;
        SetNextChunk();
    }

    private void Awake()
    {
        EnemyRef = this.gameObject;
        CurrentChunk = ProcGen.CurrentDir * EnemiesCSectionIndex;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        if (distanceToPlayer > speedUpDistance)
        {
            currentSpeed = boostSpeed;
        }
        else
        {
            currentSpeed = defaultSpeed;
        }

        soundTimer += Time.deltaTime;

        if (soundTimer > 10.0f)
        {
            soundTimer = 0.0f;
            GetComponent<AudioSource>().Play();
        }

        float ActMovement = Time.deltaTime * currentSpeed;

        if (CDir.magnitude != 1)
        {
            Debug.Log(CDir);
            Debug.Break();
        }
        this.transform.position += new Vector3(ActMovement * CDir.x, 0.0f, ActMovement * CDir.y);

        bool DoSnap = false;

        float CheckDist = 0.09f;

        if (ActMovement > 0.3f)
        {
            CheckDist = ActMovement * ActMovement;
        }
        if ((this.transform.position - TargetPos).sqrMagnitude <= CheckDist)
        {
            DoSnap = true;
        }

        if (DoSnap)
        {
            PassedTrigger();
        }

        Collider[] HitObjs = Physics.OverlapBox(this.transform.position, new Vector3(ChunkSize, 50.0f, 5f), this.transform.rotation);

        for (int i = 0; i < HitObjs.Length; i++)
        {
            if (DestroyStrings.Contains(HitObjs[i].gameObject.tag))
            {
                Destroy(HitObjs[i].gameObject);
            }
            else if (HitObjs[i].gameObject.tag == "Player")
            {
                playerObject.GetComponent<PlayerController>().GameOver();
            }
        }
    }
}
