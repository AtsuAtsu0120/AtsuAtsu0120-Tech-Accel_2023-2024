using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class NPCRoundAsset : ScriptableObject
{
    public List<Vector3> PassPositionList => _passPositionList;
    [SerializeField] private List<Vector3> _passPositionList;

    /// <summary>
    /// そのインデックスで、配列が受け取れるか調べる。
    /// </summary>
    /// <param name="index">index</param>
    /// <returns>true: 受け取れる。 false: 受け取れない。</returns>
    public bool GetableGoal(in int index)
    {
        return index >= 0 && index < _passPositionList.Count;
    }
    /// <summary>
    /// インデックスで目的地（経由地）を検索する。
    /// </summary>
    /// <param name="index">index</param>
    /// <returns>目的地（経由地）</returns>
    public Vector3 GetGoal(in int index)
    {
        return _passPositionList[index];
    }
    
    /// <summary>
    /// ランダムに目的地（経由地）を取得する。
    /// </summary>
    /// <returns>目的地（経由地）</returns>
    public Vector3 GetRandomGoal()
    {
        var randomNum = Random.Range(0, _passPositionList.Count);

        return _passPositionList[randomNum];
    }
    /// <summary>
    /// ポジションをシャッフルする。
    /// </summary>
    public void ShufflePassPositionList()
    {
        _passPositionList = PassPositionList.OrderBy(x => Guid.NewGuid()).ToList();
    }
}   
