using System;
using System.Collections.Generic;
using System.Linq;
using InGame;
using Unity.Burst;
using UnityEngine;
using UniRx;

public class IndicatorManager : UIManagerBase
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Camera _camera;
    [SerializeField] private Indicator _indicatorPrefab;
    [SerializeField] private Transform _indicatorParent;
    private ObjectPool<Indicator> _indicatorPool;
    private List<Indicator> _activeIndicators = new(4);
    private float vigilanceThreshold = 3.0f;
    
    private const int NPCCount = 4;

    private void Start()
    {
        _indicatorPool = new(NPCCount, _indicatorPrefab);

        _indicatorPool.OnActive.Subscribe(indicator =>
        {
            // Indicatorの位置を初期化する
            Transform indicatorTransform = indicator.transform;
            indicatorTransform.SetParent(_indicatorParent);
            indicatorTransform.localPosition = new Vector3(0, 0, 0);
            
            // Indicatorの色をリセットする
            indicator.SliderValue = Indicator.DefaultSliderValue;

            indicator.transform.localPosition = indicatorTransform.localPosition;
        }).AddTo(this);
    }

    private void Update()
    {
        foreach (var indicator in _activeIndicators)
        {
            SetIndicatorProperty(indicator);
        }   
    }

    /// <summary>
    /// どこからみられているのかを追加する。
    /// </summary>
    /// <param name="npc">追いかけるNPC</param>
    public void AddIndicator(NPC npc)
    {
        var indicator = _indicatorPool.ActiveObject();
        indicator.Npc = npc;
        
        _activeIndicators.Add(indicator);
    }

    /// <summary>
    /// インディケーターを削除
    /// </summary>
    /// <param name="npc">追いかけられてたNPC</param>
    public void RemoveIndicator(NPC npc)
    {
        var indicator = _activeIndicators.FirstOrDefault(indicator => indicator.Npc == npc);
        if (indicator is null)
        {
            return;
        }
        _indicatorPool.InactiveObject(indicator);

        _activeIndicators.Remove(indicator);
    }
    
    /// <summary>
    /// NPCに対してIndicatorがアクティブになっているか
    /// </summary>
    /// <param name="npc">対象となるNPC</param>
    /// <returns></returns>
    public bool IsIndicatorActive(NPC npc)
    {
        var result = _activeIndicators.Any(x => x.Npc == npc);
        
        return result;
    }

    private void SetIndicatorProperty(Indicator indicator)
    {
        SetIndicatorSlider(indicator);
        SetIndicatorDirection(indicator);
    }
    private void SetIndicatorSlider(Indicator indicator)
    {
        var value = indicator.Npc.VigilancePercent / vigilanceThreshold;
        indicator.SliderValue = value;
    }
    [BurstCompile]
    private void SetIndicatorDirection(Indicator indicator)
    {
        var rot = Quaternion
            .LookRotation(indicator.Npc.transform.position - _player.transform.position);
        var angle = (_camera.transform.eulerAngles - rot.eulerAngles).y;
        indicator.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
