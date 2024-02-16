using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectPool<T> where T : MonoBehaviour
{
    public IObservable<T> OnActive => _onActive;
    public IObservable<T> OnInactive => _onInactive;
    
    // なるべく用語を使わない変数名にして使うときに入りやすいようにしてみた。（リリースやキャッチ）
    private Subject<T> _onActive = new();
    private Subject<T> _onInactive = new();

    private T _prefab;
    private Queue<T> _pool;
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="capacity">オブジェクトの最大数</param>
    /// <param name="prefab">使用するPrefab</param>
    public ObjectPool(int capacity, T prefab)
    {
        _pool = new Queue<T>(capacity);
        _prefab = prefab;
        
        Initialize(capacity);
    }

    /// <summary>
    /// プールから取り出してオブジェクトを表示する。
    /// </summary>
    /// <returns>取り出されたオブジェクト</returns>
    public T ActiveObject()
    {
        if (_pool.TryDequeue(out var releaseObj))
        {
            _onActive.OnNext(releaseObj);
            releaseObj.gameObject.SetActive(true);
            
            return releaseObj;
        }

        return null;
    }
    /// <summary>
    /// オブジェクトを非表示にしてプールする。
    /// </summary>
    /// <param name="obj">その対象</param>
    public void InactiveObject(T obj)
    {
        _onInactive.OnNext(obj);
        obj.gameObject.SetActive(false);
        
        _pool.Enqueue(obj);
    }

    private void Initialize(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            var obj = Object.Instantiate(_prefab);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}
