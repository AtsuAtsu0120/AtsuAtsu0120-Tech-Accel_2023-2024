using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AccelUtility
{
    /// <summary>
    /// 対象が自分から見て前方扇状に存在するかどうか
    /// </summary>
    /// <param name="hitObject">対象</param>
    /// <param name="self">自分</param>
    /// <param name="maxAngle">最大角</param>
    /// <param name="minAngle">最小角</param>
    /// <returns>true: 存在する。 false: 存在しない。</returns>
    public static bool IsHitInAngle(in Transform hitObject, in Transform self, in float maxAngle, in float minAngle)
    {
        var direction = hitObject.position - self.position;
        var angle = Vector3.Dot(self.forward, direction.normalized);
        return angle > minAngle && angle < maxAngle;
    }
}
