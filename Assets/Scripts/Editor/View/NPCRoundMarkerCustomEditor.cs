using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCRoundMarker))]
public class NPCRoundMarkerCustomEditor : Editor
{
    public void OnDestroy()
    {
        var marker = (NPCRoundMarker)target;
        marker.Asset.PassPositionList.RemoveAt(marker.ArrayIndex);
    }
}
