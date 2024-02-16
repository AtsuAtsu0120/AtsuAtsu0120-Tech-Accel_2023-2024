using TMPro;
using UnityEngine;

public class InteractUIManager : UIManagerBase
{
    public string InteractDescription
    {
        get => _text.text;
        set => _text.SetText(value);
    }
    [SerializeField]private TextMeshProUGUI _text;
}
