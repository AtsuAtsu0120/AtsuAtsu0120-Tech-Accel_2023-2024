using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public NPC Npc { get; set; }

    public static float DefaultSliderValue { get; } = -0.1f;

    public float SliderValue
    {
        get => _sliderValue.Value;
        set
        {
            if (value < 1.0f)
            {
                _sliderValue.Value = value;
            }
        }
    }
    
    [SerializeField] private Image _image;
    private static readonly int SliderValuePropertyID = Shader.PropertyToID("_SliderValue");
    private ReactiveProperty<float> _sliderValue = new();

    private void Start()
    {
        var material = new Material(_image.material);
        material.SetFloat(SliderValuePropertyID, DefaultSliderValue);

        _sliderValue.Subscribe(value =>
        {
            material.SetFloat(SliderValuePropertyID, value);
        });

        _image.material = material;
    }
#if UNITY_EDITOR

    private void Reset()
    {
        _image = GetComponentInChildren<Image>();
    }

    #endif
}
