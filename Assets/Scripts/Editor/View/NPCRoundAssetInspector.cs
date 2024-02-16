using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

[CustomEditor(typeof(NPCRoundAsset))]
public class NPCRoundAssetInspector : Editor
{
    private bool _isEditMode;
    private bool _isShowGizmos;
    private NPCRoundAsset _asset;

    private ListView _positionListView;
    private Button _modeChangeButton;
    private Button _showGizmosButton;

    private GameObject _gizmoPrefab;
    private List<GameObject> _gizmosList;
    #region UnityMethods

    public void OnEnable()
    {
        _asset = (NPCRoundAsset)target;
        _gizmoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/Editor/Gizmo.prefab");
        _gizmosList = new();
    }
    public override VisualElement CreateInspectorGUI()
    {
        var rootElement = new VisualElement();
        
        // List一覧を表示
        var label = new Label("Position List");
        rootElement.Add(label);
        // ListViewの設定
        _positionListView = new ListView(_asset.PassPositionList, 20, MakePositionUI, BindPositionUI);
        _positionListView.selectionType = SelectionType.Multiple;
        
        rootElement.Add(_positionListView);
        
        // ボタンを追加
        _modeChangeButton = new Button();
        _modeChangeButton.text = "Set Round Position";
        _modeChangeButton.clicked += ChangeMode;
        ChangeBorderColor(_modeChangeButton, _isEditMode);

        _showGizmosButton = new Button();
        _showGizmosButton.text = "Show Round Position";
        _showGizmosButton.clicked += ChageGizmosActive;
        ChangeBorderColor(_showGizmosButton, _isShowGizmos);
        _showGizmosButton.SetEnabled(!_isShowGizmos);

        
        rootElement.Add(_modeChangeButton);
        rootElement.Add(_showGizmosButton);
        
        return rootElement;
    }

    public void OnSceneViewClick(SceneView sceneView)
    {
        // イベントを変数として保存
        var e = Event.current;
    
        // マウスの左クリックのときtrue
        if (e.type == EventType.MouseDown && e.button == 0) {
            // 座標のズレが起きないようにマウスのポジションを取得
            var mousePosition = new Vector3(e.mousePosition.x, 
                Camera.current.pixelHeight - e.mousePosition.y,
                0);
            
            // レイキャストを飛ばす
            if (Physics.Raycast(Camera.current.ScreenPointToRay(mousePosition), out var hit))
            {
                _asset.PassPositionList.Add(hit.point);
                CreatePositionGizmos(hit.point);
            }
        }
        _positionListView.RefreshItems();
    }

    #endregion

    private void ChangeMode()
    {
        if (_isEditMode)
        {
            SceneView.duringSceneGui -= OnSceneViewClick;
            SceneVisibilityManager.instance.EnableAllPicking();
            
            HidePositionGizmos();
            _isShowGizmos = false;
            _showGizmosButton.SetEnabled(true);
        }
        else
        {
            SceneView.duringSceneGui += OnSceneViewClick;
            SceneVisibilityManager.instance.DisableAllPicking();
            
            ShowPositionGizmos();
            _isShowGizmos = true;
            _showGizmosButton.SetEnabled(false);
        }

        _isEditMode = !_isEditMode;
        ChangeBorderColor(_modeChangeButton, _isEditMode);
        ChangeBorderColor(_showGizmosButton, _isEditMode);
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
    }

    private void ChageGizmosActive()
    {
        if (_isShowGizmos)
        {
            HidePositionGizmos();
        }
        else
        {
            ShowPositionGizmos();
            
        }

        _isShowGizmos = !_isShowGizmos;
        ChangeBorderColor(_showGizmosButton, _isShowGizmos);
    }

    private VisualElement MakePositionUI()
    {
        var filed = new Vector3Field();
        filed.SetEnabled(false);
        return filed;
    }

    private void BindPositionUI(VisualElement positionElement, int i)
    {
        var positionUI = positionElement as Vector3Field;
        positionUI.value = _asset.PassPositionList[i];
    }

    private void ShowPositionGizmos()
    {
        foreach (var position in _asset.PassPositionList)
        {
            CreatePositionGizmos(position);
        } 
    }

    private void CreatePositionGizmos(Vector3 position)
    {
        var obj = (GameObject)PrefabUtility.InstantiatePrefab(_gizmoPrefab);
        obj.transform.position = position;
        var component = obj.GetComponent<NPCRoundMarker>();
        component.ArrayIndex = _gizmosList.Count;
        component.Asset = _asset;

        Selection.activeObject = _asset;
        _gizmosList.Add(obj);
    }

    private void HidePositionGizmos()
    {
        foreach (var obj in _gizmosList)
        {
            DestroyImmediate(obj);
        }
        _gizmosList.Clear();
        
        SaveAsset();
    }

    private void ChangeBorderColor(VisualElement element, bool isActive)
    {
        var color = isActive ? Color.green : new Color(0, 0, 0, 0);
        element.style.borderBottomColor = color;
        element.style.borderTopColor = color;
        element.style.borderRightColor = color;
        element.style.borderLeftColor = color;
    }

    private void SaveAsset()
    {
        EditorUtility.SetDirty(_asset);
        AssetDatabase.SaveAssets();
    }
}
