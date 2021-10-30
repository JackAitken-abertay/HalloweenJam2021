using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public GameObject[] SectionsInChunk;
    public Vector2Int ChunkPos;

    public ChunkData(GameObject[] SectionsInChunkIn, Vector2Int ChunkPosIn)
    {
        SectionsInChunk = SectionsInChunkIn;
        ChunkPos = ChunkPosIn;
    }
}
