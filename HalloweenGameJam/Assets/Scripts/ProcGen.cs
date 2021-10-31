using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcGen : MonoBehaviour
{
    SectionStruct[][] StraightSections;
    SectionStruct[][] CurvedSections;

    GameObject[][] SideSections;

    public GameObject[] SideSectionsIn;
    public int[] ObjectsPerSideCluster;

    Vector2Int[] SavedSideChunks = new Vector2Int[2]
    {
        new Vector2Int(-999,-999),
        new Vector2Int(-999,-999),
    };

    static Vector2Int[] SavedSideVecs = new Vector2Int[2]
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
    };

    static readonly Vector2Int[][] SavedSideVecsAll = new Vector2Int[2][]
    {
        new Vector2Int[]
        {
            new Vector2Int(1,0),
            new Vector2Int(-1,0),
        },
        new Vector2Int[]
        {
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
        },
    };

    public static ChunkData[] LoadedSections;

    static readonly SectionStruct NullStruct = new SectionStruct(null, 0);

    public int NumOfEras;
    public GameObject[] AllSections;
    public int[] SectionsPerEra;
    public int NumOfStraightSections;
    public int NumOfCurvedSections;
    public ushort[] SectionsAvaliableSideTab;
    public int PlayerChunkIndex = 1;
    Vector2Int PlayerChunkPos;

    int StraightSectionsPerEra;

    public bool TestChunks = false;
    public float TestChunkTimer = 0.0f;
    public GameObject TestPlayer;
    public Transform PlayerTransform;

    public float ChunkSize = 5.0f;

    static public byte CurrentEra = 0;

    float TriggerPos;
    bool TriggerYAxis;
    bool TriggerCheckMoreThan;

    Vector2Int CurrentChunk = new Vector2Int(0, 0);
    public static Vector2Int CurrentDir = new Vector2Int(0, -1);
    Vector2Int CurrentLastLoadedChunk = new Vector2Int(0, 0);

    public int AmountOfChunksToLoad = 5;
    int ChunksSinceLastTurn;

    int CurrentTurnChance = 100;

    HazardManager hazardManager;
    PowerupManager powerupManager;
    TimePeriod t;
    PlayerController playerController;
    Enemy enemy;

    private void Awake()
    {
        hazardManager = GameObject.FindObjectOfType<HazardManager>().GetComponent<HazardManager>();
        powerupManager = GameObject.FindObjectOfType<PowerupManager>().GetComponent<PowerupManager>();
        t = GameObject.FindObjectOfType<TimePeriod>().GetComponent<TimePeriod>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        enemy = GameObject.Find("Ugpin").GetComponent<Enemy>();
    }

    private void Start()
    {
        ChunksSinceLastTurn = AmountOfChunksToLoad;

        StraightSectionsPerEra = Mathf.RoundToInt((float)SectionsPerEra.Length / 2.0f);

        SideSections = new GameObject[ObjectsPerSideCluster.Length][];

        Enemy.ChunkSize = ChunkSize;

        if (TestPlayer != null)
        {
            PlayerTransform = TestPlayer.transform;
        }

        float PlayerStartPos = CurrentDir.y * PlayerChunkIndex;
        float EnemyStartPos = CurrentDir.y * Enemy.EnemiesCSectionIndex;

        PlayerChunkIndex = AmountOfChunksToLoad - PlayerChunkIndex - 1;
        Enemy.EnemiesCSectionIndex = AmountOfChunksToLoad - Enemy.EnemiesCSectionIndex;

        PlayerTransform.position = new Vector3(0.0f, 1.0f, PlayerStartPos * ChunkSize);
        Enemy.EnemyRef.transform.position = new Vector3(0.0f, 1.0f, EnemyStartPos * ChunkSize);

        int SideSectionIndex = 0;

        for (int i = 0; i < ObjectsPerSideCluster.Length; i++)
        {
            SideSections[i] = new GameObject[ObjectsPerSideCluster[i]];
            GameObject[] CurrentSideCluster = SideSections[i];

            for (int j = 0; j < CurrentSideCluster.Length; j++)
            {
                CurrentSideCluster[j] = SideSectionsIn[SideSectionIndex];
                SideSectionIndex++;
            }
        }

        LoadedSections = new ChunkData[AmountOfChunksToLoad];

        int SectionIndex = 0;

        StraightSections = new SectionStruct[NumOfEras][];
        CurvedSections = new SectionStruct[NumOfEras][];

        for (int i = 0; i < SectionsPerEra.Length; i++)
        {
            SectionStruct[][] SectionArray = i < StraightSectionsPerEra ? StraightSections : CurvedSections;
            int Era = i % NumOfEras;

            SectionArray[Era] = new SectionStruct[SectionsPerEra[i]];

            for (int j = 0; j < SectionArray[Era].Length; j++)
            {
                SectionArray[Era][j] = new SectionStruct(AllSections[SectionIndex], SectionsAvaliableSideTab[SectionIndex]);
                SectionIndex++;
            }
        }

        for (int i = 0; i < StraightSections.Length; i++)
        {

            for (int j = 0; j < StraightSections[i].Length; j++)
            {
              
            }
        }

        for (int i = 0; i < CurvedSections.Length; i++)
        {

            for (int j = 0; j < StraightSections[i].Length; j++)
            {
                
            }
        }

        for (int i = 0; i < AmountOfChunksToLoad; i++)
        {
            LoadNewChunk();
        }
    }

    private void Update()
    {
        if (TestChunks)
        {
            if (TestChunkTimer < 0.0f)
            {
                TestChunkTimer = 0.5f;
                LoadNewChunk();
            }
            else
            {
                TestChunkTimer -= Time.deltaTime;
            }
        }

        triggerCheck();
    }

    void triggerCheck()
    {
        if (TriggerYAxis)
        {
            if (TriggerCheckMoreThan)
            {
                if (PlayerTransform.position.z > TriggerPos)
                {
                    LoadNewChunk();
                    Enemy.EnemiesCSectionIndex++;
                }
            }
            else
            {
                if (PlayerTransform.position.z < TriggerPos)
                {
                    LoadNewChunk();
                    Enemy.EnemiesCSectionIndex++;
                }
            }
        }
        else
        {
            if (TriggerCheckMoreThan)
            {
                if (PlayerTransform.position.x > TriggerPos)
                {
                    LoadNewChunk();
                    Enemy.EnemiesCSectionIndex++;
                }
            }
            else
            {
                if (PlayerTransform.position.x < TriggerPos)
                {
                    LoadNewChunk();
                    Enemy.EnemiesCSectionIndex++;
                }
            }
        }
    }

    void LoadNewChunk()
    {

        SectionStruct ChunkToSpawn = NullStruct;
        bool DidCurvedSection = false;
        Vector2Int NewCurrentDir = CurrentDir;
        Quaternion RoadRot = Quaternion.identity;

        //Do turn
        if (ChunksSinceLastTurn == 0)
        {
            if (Random.Range(0, CurrentTurnChance) == 0)
            {
                ChunksSinceLastTurn = AmountOfChunksToLoad;
                CurrentTurnChance = 100;

                ChunkToSpawn = CurvedSections[CurrentEra][Random.Range(0, CurvedSections[CurrentEra].Length)];

                if (CurrentDir.x != 0)
                {
                    NewCurrentDir = Random.Range(0, 2) == 0 ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
                    SavedSideVecs = SavedSideVecsAll[0];
                }
                else
                {
                    NewCurrentDir = Random.Range(0, 2) == 0 ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                    SavedSideVecs = SavedSideVecsAll[1];
                }

                DidCurvedSection = true;
            }
            else
            {
                CurrentTurnChance -= 10;
                if (CurrentTurnChance < 0)
                {
                    CurrentTurnChance = 0;
                }
            }
        }
        else
        {
            ChunksSinceLastTurn -= 1;
        }

        CurrentChunk += CurrentDir;

        if (LoadedSections[AmountOfChunksToLoad - 1] != null)
        {
            GameObject[] DestroyCluster = LoadedSections[AmountOfChunksToLoad - 1].SectionsInChunk;

            for (int i = 0; i < DestroyCluster.Length; i++)
            {
                Destroy(DestroyCluster[i]);
            }
        }

        for (int i = AmountOfChunksToLoad - 1; i > 0; i--)
        {
            LoadedSections[i] = LoadedSections[i - 1];
        }

        Vector3 NewChunkSpawnPos = new Vector3(CurrentChunk.x * ChunkSize, 0.0f, CurrentChunk.y * ChunkSize);

        GameObject[] CSpawnedSections;

        if (!DidCurvedSection)
        {
            CSpawnedSections = new GameObject[3];

            ChunkToSpawn = StraightSections[CurrentEra][Random.Range(0, StraightSections[CurrentEra].Length)];
            RoadRot = Quaternion.Euler(-90.0f, CurrentDir.x != 0 ? 0.0f : 90.0f, 0.0f);

            Debug.Log("THIS: " + ChunkToSpawn.OriginalIndex);
            Debug.Log("THIS2: " + SideSections[ChunkToSpawn.OriginalIndex][0].name);

            for (int i = 1; i < 3; i++)
            {
                GameObject[] AvalSides = SideSections[ChunkToSpawn.OriginalIndex];
                CSpawnedSections[i] = Instantiate(AvalSides[Random.Range(0, AvalSides.Length)]);
            }

            bool DontMake = false;

            for (int i = 0; i < SavedSideChunks.Length; i++)
            {
                if (SavedSideChunks[i] == SavedSideVecs[0] + CurrentChunk)
                {
                    DontMake = true;
                    break;
                }
            }

            if (!DontMake)
            {
                CSpawnedSections[1].transform.position = new Vector3(NewChunkSpawnPos.x + SavedSideVecs[0].x * ChunkSize, 0.0f, NewChunkSpawnPos.z + SavedSideVecs[0].y * ChunkSize);
                CSpawnedSections[1].SetActive(true);
            }

            DontMake = false;

            for (int i = 0; i < SavedSideChunks.Length; i++)
            {
                if (SavedSideChunks[i] == SavedSideVecs[1] + CurrentChunk)
                {
                    DontMake = true;
                    break;
                }
            }

            if (!DontMake)
            {
                CSpawnedSections[2].transform.position = new Vector3(NewChunkSpawnPos.x + SavedSideVecs[1].x * ChunkSize, 0.0f, NewChunkSpawnPos.z + SavedSideVecs[1].y * ChunkSize);
                CSpawnedSections[2].SetActive(true);
            }
        }
        else
        {
            CSpawnedSections = new GameObject[4];

            for (int i = 1; i < 4; i++)
            {
                GameObject[] AvalSides = SideSections[ChunkToSpawn.OriginalIndex];
                CSpawnedSections[i] = Instantiate(AvalSides[Random.Range(0, AvalSides.Length)]);
            }

            if ((CurrentDir == new Vector2Int(0, 1) && NewCurrentDir == new Vector2Int(1, 0)) || (CurrentDir == new Vector2Int(-1, 0) && NewCurrentDir == new Vector2Int(0, -1)))
            {
                CSpawnedSections[1].transform.position = NewChunkSpawnPos + new Vector3(0.0f, 0.0f, ChunkSize);
                CSpawnedSections[2].transform.position = NewChunkSpawnPos + new Vector3(-ChunkSize, 0.0f, ChunkSize);
                CSpawnedSections[3].transform.position = NewChunkSpawnPos + new Vector3(-ChunkSize, 0.0f, 0.0f);

                RoadRot = Quaternion.Euler(-90.0f, 270.0f, 0.0f);
                playerController.roadDirection = enemy.roadDirection = NewCurrentDir;
            }
            else if ((CurrentDir == new Vector2Int(0, -1) && NewCurrentDir == new Vector2Int(1, 0)) || (CurrentDir == new Vector2Int(-1, 0) && NewCurrentDir == new Vector2Int(0, 1)))
            {
                CSpawnedSections[1].transform.position = NewChunkSpawnPos + new Vector3(0.0f, 0.0f, -ChunkSize);
                CSpawnedSections[2].transform.position = NewChunkSpawnPos + new Vector3(-ChunkSize, 0.0f, -ChunkSize);
                CSpawnedSections[3].transform.position = NewChunkSpawnPos + new Vector3(-ChunkSize, 0.0f, 0.0f);

                RoadRot = Quaternion.Euler(-90.0f, 180.0f, 0.0f);
                playerController.roadDirection = enemy.roadDirection = NewCurrentDir;
            }
            else if ((CurrentDir == new Vector2Int(0, -1) && NewCurrentDir == new Vector2Int(-1, 0)) || (CurrentDir == new Vector2Int(1, 0) && NewCurrentDir == new Vector2Int(0, 1)))
            {
                CSpawnedSections[1].transform.position = NewChunkSpawnPos + new Vector3(0.0f, 0.0f, -ChunkSize);
                CSpawnedSections[2].transform.position = NewChunkSpawnPos + new Vector3(ChunkSize, 0.0f, -ChunkSize);
                CSpawnedSections[3].transform.position = NewChunkSpawnPos + new Vector3(ChunkSize, 0.0f, 0.0f);

                RoadRot = Quaternion.Euler(-90.0f, 90.0f, 0.0f);
                playerController.roadDirection = enemy.roadDirection = NewCurrentDir;
            }
            else if ((CurrentDir == new Vector2Int(0, 1) && NewCurrentDir == new Vector2Int(-1, 0)) || (CurrentDir == new Vector2Int(1, 0) && NewCurrentDir == new Vector2Int(0, -1)))
            {
                CSpawnedSections[1].transform.position = NewChunkSpawnPos + new Vector3(0.0f, 0.0f, ChunkSize);
                CSpawnedSections[2].transform.position = NewChunkSpawnPos + new Vector3(ChunkSize, 0.0f, ChunkSize);
                CSpawnedSections[3].transform.position = NewChunkSpawnPos + new Vector3(ChunkSize, 0.0f, 0.0f);

                RoadRot = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                playerController.roadDirection = enemy.roadDirection = NewCurrentDir;
            }

            CSpawnedSections[1].SetActive(true);
            CSpawnedSections[2].SetActive(true);
            CSpawnedSections[3].SetActive(true);
        }

        ChunkToSpawn.Obj.SetActive(true);

        GameObject NewChunk = Instantiate(ChunkToSpawn.Obj, NewChunkSpawnPos, RoadRot, null);
        CSpawnedSections[0] = NewChunk;
        LoadedSections[0] = new ChunkData(CSpawnedSections, CurrentChunk);

        hazardManager.spawnHazards(NewChunkSpawnPos, 1);
        powerupManager.spawnPowerups(NewChunkSpawnPos, 1);

        ChunkToSpawn.Obj.SetActive(false);

        if (LoadedSections[PlayerChunkIndex] != null)
        {
            PlayerChunkPos = LoadedSections[PlayerChunkIndex].ChunkPos;
            Vector2Int NextChunk = LoadedSections[PlayerChunkIndex - 1].ChunkPos;

            Vector2Int Dir = NextChunk - PlayerChunkPos;

            TriggerYAxis = Dir.x == 0;
            TriggerPos = TriggerYAxis ? (PlayerChunkPos.y + (Dir.y * 0.5f)) * ChunkSize : (PlayerChunkPos.x + (Dir.x * 0.5f)) * ChunkSize;
            TriggerCheckMoreThan = TriggerYAxis ? (Dir.y == 1) : (Dir.x == 1);
        }

        CurrentDir = NewCurrentDir;
    }
}
