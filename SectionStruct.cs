using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SectionStruct
{
    public GameObject Obj;
    public ushort OriginalIndex;

    public SectionStruct(GameObject ObjIn, ushort OriginalIndexIn)
    {
        Obj = ObjIn;
        OriginalIndex = OriginalIndexIn;
    }
}
