#if UNITY_EDITOR

using System;
using UnityEngine;

public class NPCRoundMarker : MonoBehaviour
{
    public NPCRoundAsset Asset { get; set; }
    public int ArrayIndex { get; set; }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var offsetPosition = transform.position;
        offsetPosition.y += 1;
        Gizmos.DrawLine(transform.position, offsetPosition);

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}

#endif
