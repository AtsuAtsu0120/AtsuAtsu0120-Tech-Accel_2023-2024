using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using UniRx;
using UnityEngine.UI;

public class InGameUIManager : UIManagerBase
{
    [Header("ゲームオブジェクト")]
    [SerializeField] private RectTransform targetsParent;
    [SerializeField] private Slider _confirmSlider;
    [SerializeField] private Image _fill;
 
    [Header("Prefab")] 
    [SerializeField] private TextMeshProUGUI targetTextPrefab;
    
    private InGameManager _inGameManager;
    private static readonly int SliderValue = Shader.PropertyToID("_SliderValue");

    [Inject]
    public void Constructor(InGameManager inGameManager)
    {
        _inGameManager = inGameManager;

        // ConfirmSliderの値をイベントで変更
        _inGameManager.ConfirmPercent.Subscribe(value =>
        {
            _confirmSlider.value = value;
            _fill.material.SetFloat(SliderValue, value / InGameManager.MaxConfirm);
        }).AddTo(this);
        
        foreach (var target in inGameManager.Stage.Targets)
        {
            // テキストを生成
            var obj = Instantiate(targetTextPrefab, targetsParent);
            var text = obj.GetComponent<TextMeshProUGUI>(); 
            text.text = $"- {target.TargetName}";
            
            // イベントを登録
            target.IsAchieved.Where(isAchieved => isAchieved)
                .Subscribe(_ => text.fontStyle = FontStyles.Strikethrough).AddTo(target);
        }
    }
}
