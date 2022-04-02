using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.U2D;

public class ModifyVehicleDataBaseEditorWindow : EditorWindow
{
    private List<string> _icons;
    private VehicleDataBaseStorage _vehicleDataBase;
    private const string DATA_PATH = "StreamingAssets/VehicleDataBase.json";
    private GUIStyle textStyle;
    private Vector2 _scrollValue;
    private Color _selectionColor = new Color(0.49f, 0.49f, 0.49f, 0.25f);
    private Color BackGroundColor => EditorGUIUtility.isProSkin ? new Color(56, 56, 56, 255) : new Color(194, 194, 194, 255);
    private int _selectedID = 0;
    public int SelectedID 
    { 
        get => _selectedID;
        set
        {
            if (_selectedID != value)
            { 
                _selectedID = value;
                OnSelectedRecordChange();
            }
        }
    }
    private VehicleDataBaseRecord SelectedRecord => SelectedID != 0 ? _vehicleDataBase.Get(SelectedID) : null;
    private bool IsSomeThingSelected => SelectedID != 0;
    private Texture2D _selectionTexture;
    private const int CAPTION_SPACE = 10;
    private const int MENU_HEIGHT = 30;
    private const int FIELD_HEIGHT = 15;
    private const int LABEL_FIELD_WIDTH = 120;
    private const int TEXT_FIELD_WIDTH = 116;
    private const int ID_WIDTH = 30;
    private const int MENU_BUTTON_WIDTH = 60;
    private const int MENU_SPACE = 30;
    //To modify data
    private int _toModID;
    private string _toModName;
    private string _toModIcon;
    private float _toModMass;
    private int _toModCapacity;
    private float _toModMaxVelocity;
    private VehicleType _toModType;
    private ColorEnum _toModColor;
    private float _toModDisplaceMent;
    private Vector3Int _tomodSize;
    private float _toModLiftingForce;
    private static ModifyVehicleDataBaseEditorWindow _winInstance;
    private static ModifyVehicleDataBaseEditorWindow Instance
    {
        get
        {
            if (_winInstance == null)
            {
                _winInstance = EditorWindow.GetWindow(typeof(ModifyVehicleDataBaseEditorWindow)) as ModifyVehicleDataBaseEditorWindow;
            }
            return _winInstance;
        }
    }

    [MenuItem("Window/Vehicle Data Base Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        _winInstance = EditorWindow.GetWindow<ModifyVehicleDataBaseEditorWindow>("Vehicle Data Base Editor");
    }
    private void OnEnable()
    {
        textStyle = new GUIStyle();
        if (EditorGUIUtility.isProSkin)
        {
            textStyle.normal.textColor = Color.white;
        }
        else
        {
            textStyle.normal.textColor = Color.black;
        }
        textStyle.alignment = TextAnchor.MiddleCenter;

        _scrollValue = Vector2.zero;
        _selectionTexture = GenerateTexture(20, 20, _selectionColor);

        LoadIcons();
        LoadData();
    }

    private void LoadIcons()
    {
        SpriteAtlas spriteAtlas = Resources.Load<SpriteAtlas>("Icons Sprite Atlas");
        Sprite[] sprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(sprites);
        _icons = new List<string>();
        for (int i = 0; i < sprites.Length; i++)
        {
            _icons.Add(sprites[i].name.Substring(0, sprites[i].name.Length - 7));
        }
        _toModIcon = _icons[0];
    }

    private void LoadData()
    {
        string path = Path.Combine(Application.dataPath, DATA_PATH);
        string jsonString = File.ReadAllText(path);
        DataBaseSaveContext dbContext = JsonUtility.FromJson<DataBaseSaveContext>(jsonString);
        if (dbContext != null)
        {
            _vehicleDataBase = DataBaseSaveContext.GenerateDataBase(dbContext);
        }
    }
    private void SaveData()
    {
        string path = Path.Combine(Application.dataPath, DATA_PATH);
        var dbContext = new DataBaseSaveContext(_vehicleDataBase);
        string jsonString = JsonUtility.ToJson(dbContext);
        File.WriteAllText(path, jsonString);
    }
    private void OnGUI()
    {
        if (_vehicleDataBase == null)
        {
            GUILayout.Label("Failed to load Data");
            return;
        }
        Rect windowRect = Instance.position;
        DisplayMenuButtons(new Rect(0, 0, windowRect.width, MENU_HEIGHT));
        Rect dataTableRect = new Rect(0, MENU_HEIGHT, windowRect.width, windowRect.height);
        DisplayDataTable(dataTableRect);

        if (Event.current != null && Event.current.type == EventType.KeyUp)
        {
            List<int> ids = _vehicleDataBase.GetIDs();
            if (Event.current.keyCode == KeyCode.UpArrow)
            {
                var possibleIndex = Mathf.Max(0, ids.IndexOf(SelectedID) - 1);
                SelectedID = ids[possibleIndex];
            }
            if (Event.current.keyCode == KeyCode.DownArrow)
            {
                var possibleIndex = Mathf.Min(ids.Count - 1, ids.IndexOf(SelectedID) + 1);
                SelectedID = ids[possibleIndex];
            }
            Event.current.Use();
        }
        if (dataTableRect.Contains(Event.current.mousePosition))
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
            {
                float displacement = Event.current.mousePosition.y - MENU_HEIGHT - CAPTION_SPACE - FIELD_HEIGHT * 2;
                List<int> ids = _vehicleDataBase.GetIDs();
                int possibleIndex = (int)(displacement / FIELD_HEIGHT);
                SelectedID = possibleIndex < ids.Count ? ids[possibleIndex] : 0;
                Event.current.Use();
            }
        }
    }
    private void DisplayMenuButtons(Rect rect)
    {
        GUILayout.BeginArea(rect);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save", GUILayout.Width(MENU_BUTTON_WIDTH)))
        {
            //Save database to json
            SaveData();
        }
        if (GUILayout.Button("Undo", GUILayout.Width(MENU_BUTTON_WIDTH)))
        {
            //Load
            LoadData();
        }
        GUILayout.Space(MENU_SPACE);
        GUI.enabled = !IsSomeThingSelected;
        if (GUILayout.Button("Create", GUILayout.Width(MENU_BUTTON_WIDTH)))
        {
            AddRecord();
            ResetToModVariables();
            ResetSelection();
        }
        GUI.enabled = IsSomeThingSelected;
        if (GUILayout.Button("Update", GUILayout.Width(MENU_BUTTON_WIDTH)))
        {
            UpdateRecord();
            ResetToModVariables();
            ResetSelection();
        }
        if (GUILayout.Button("Delete", GUILayout.Width(MENU_BUTTON_WIDTH)))
        {
            DeleteRecord();
            ResetToModVariables();
            ResetSelection();
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    private void DisplayDataTable(Rect rect)
    {
        GUILayout.BeginArea(rect);
        GUILayout.BeginVertical();
        DisplayCaption();
        DisplayCreatUpdateLayout();
        GUILayout.Space(CAPTION_SPACE);
        _scrollValue = GUILayout.BeginScrollView(_scrollValue);
        foreach (VehicleDataBaseRecord record in _vehicleDataBase.GetAll())
        {
            if (record.ID == SelectedID)
            {
                GUIStyle selectedStyle = new GUIStyle();
                selectedStyle.normal.background = _selectionTexture;
                GUILayout.BeginHorizontal(selectedStyle);
            }
            else
            {
                GUILayout.BeginHorizontal();
            }
            GUILayout.Label(record.ID.ToString(), textStyle, GUILayout.Width(ID_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            GUILayout.Label(record.Name, textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            GUILayout.Label(record.IconName, textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            GUILayout.Label(record.Mass.ToString(), textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            GUILayout.Label(record.Capacity.ToString(), textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            GUILayout.Label(record.MaxVelocity.ToString(), textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            if (record is ShipDataBaseRecord)
            {
                GUILayout.Label((record as ShipDataBaseRecord).Displacement.ToString(), textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            }
            if (record is CarDataBaseRecord)
            {
                GUILayout.Label((record as CarDataBaseRecord).Color.ToString(), textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            }
            if (record is BikeDataBaseRecord)
            {
                GUILayout.Label((record as BikeDataBaseRecord).Size.ToString(), textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            }
            if (record is PlaneDataBaseRecord)
            {
                GUILayout.Label((record as PlaneDataBaseRecord).LiftingForce.ToString(), textStyle, GUILayout.Width(LABEL_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
            }
            GUILayout.EndHorizontal();
        }       
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    private void DisplayCaption()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("ID", textStyle, GUILayout.Width(ID_WIDTH));
        GUILayout.Label("Имя", textStyle, GUILayout.Width(LABEL_FIELD_WIDTH));
        GUILayout.Label("Icon", textStyle, GUILayout.Width(LABEL_FIELD_WIDTH));
        GUILayout.Label("Масса", textStyle, GUILayout.Width(LABEL_FIELD_WIDTH));
        GUILayout.Label("Вместительность", textStyle, GUILayout.Width(LABEL_FIELD_WIDTH));
        GUILayout.Label("Макс Скорость", textStyle, GUILayout.Width(LABEL_FIELD_WIDTH));
        GUILayout.Label("Уникальное св-во", textStyle, GUILayout.Width(LABEL_FIELD_WIDTH));
        GUILayout.EndHorizontal();
    }
    private void DisplayCreatUpdateLayout()
    {
        GUILayout.BeginHorizontal();
        if (!IsSomeThingSelected)
        {
            _toModID = EditorGUILayout.IntField(GetSmallestFreeID(), GUILayout.Width(ID_WIDTH), GUILayout.Height(FIELD_HEIGHT));
        }
        else
        {
            _toModID = EditorGUILayout.IntField(SelectedRecord.ID, GUILayout.Width(ID_WIDTH), GUILayout.Height(FIELD_HEIGHT));
        }
        _toModName = GUILayout.TextField(DefaultIfEmpty(_toModName),  GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
        int iconindex = EditorGUILayout.Popup(_icons.IndexOf(_toModIcon), _icons.ToArray(), GUILayout.Width(116), GUILayout.Height(FIELD_HEIGHT));
        _toModIcon = _icons[iconindex];
        _toModMass = EditorGUILayout.FloatField(Mathf.Abs(_toModMass),  GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
        _toModCapacity = EditorGUILayout.IntField(Mathf.Abs(_toModCapacity),  GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
        _toModMaxVelocity = EditorGUILayout.FloatField(Mathf.Abs(_toModMaxVelocity),  GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
        switch (_toModType)
        {
            case VehicleType.Car:
                _toModColor = (ColorEnum)EditorGUILayout.EnumPopup(_toModColor, GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
                break;
            case VehicleType.Ship:
                _toModDisplaceMent = EditorGUILayout.FloatField(Mathf.Abs(_toModDisplaceMent), GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
                break;
            case VehicleType.Plane:
                _toModLiftingForce = EditorGUILayout.FloatField(Mathf.Abs(_toModLiftingForce), GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
                break;
            case VehicleType.Bike:
                _tomodSize = EditorGUILayout.Vector3IntField("", Validate(_tomodSize), GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
                break;
            default:
                GUILayout.Label("", GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
                break;
        }
        if (!IsSomeThingSelected)
        {
            _toModType = (VehicleType)EditorGUILayout.EnumPopup(_toModType, GUILayout.Width(TEXT_FIELD_WIDTH), GUILayout.Height(FIELD_HEIGHT));
        }
        GUILayout.EndHorizontal();
    }
    private Texture2D GenerateTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }
    private int GetSmallestFreeID()
    {
        //Copy of the list otherwise ids of database will be sorted
        List<int> ids = new List<int>(_vehicleDataBase.GetIDs());
        ids.Sort();
        int largest = ids[ids.Count - 1];
        for (int i = 1; i < largest; i++)
        {
            if (!ids.Contains(i))
            {
                return i;
            }
        }
        return largest + 1;
    }
    private Vector3Int Validate(Vector3Int value) => new Vector3Int(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
    private string DefaultIfEmpty(string value) => value != "" && value != null ? value : "Default";
    private void AddRecord()
    {
        VehicleDataBaseRecord record = _toModType switch
        {
            VehicleType.Car => new CarDataBaseRecord(_toModID, _toModName, _toModIcon, _toModMass, _toModCapacity, _toModMaxVelocity, _toModColor),
            VehicleType.Ship => new ShipDataBaseRecord(_toModID, _toModName, _toModIcon, _toModMass, _toModCapacity, _toModMaxVelocity, _toModDisplaceMent),
            VehicleType.Bike => new BikeDataBaseRecord(_toModID, _toModName, _toModIcon, _toModMass, _toModCapacity, _toModMaxVelocity, _tomodSize),
            VehicleType.Plane => new PlaneDataBaseRecord(_toModID, _toModName, _toModIcon, _toModMass, _toModCapacity, _toModMaxVelocity, _toModLiftingForce),
            _ => null
        };
        _vehicleDataBase.Add(_toModID, record);
    }
    private void DeleteRecord()
    {
        if (_vehicleDataBase.GetIDs().Contains(SelectedID))
        {
            _vehicleDataBase.Delete(SelectedID);
        }
    }
    private void UpdateRecord()
    {
        VehicleDataBaseRecord record = _toModType switch
        {
            VehicleType.Car => new CarDataBaseRecord(_toModID, _toModName, _toModIcon, _toModMass, _toModCapacity, _toModMaxVelocity, _toModColor),
            VehicleType.Ship => new ShipDataBaseRecord(_toModID, _toModName, _toModIcon, _toModMass, _toModCapacity, _toModMaxVelocity, _toModDisplaceMent),
            VehicleType.Bike => new BikeDataBaseRecord(_toModID, _toModName, _toModIcon, _toModMass, _toModCapacity, _toModMaxVelocity, _tomodSize),
            VehicleType.Plane => new PlaneDataBaseRecord(_toModID, _toModName, _toModIcon, _toModMass, _toModCapacity, _toModMaxVelocity, _toModLiftingForce),
            _ => null
        };
        _vehicleDataBase.UpdateRecord(_toModID, record);
    }
    private void ResetToModVariables()
    {
        _toModName = string.Empty;
        _toModIcon = _icons[0];
        _toModMass = 0;
        _toModCapacity = 0;
        _toModMaxVelocity = 0;
        //_toModType = ;
        _toModColor = ColorEnum.White;
        _toModDisplaceMent = 0;
        _tomodSize = Vector3Int.zero;
        _toModLiftingForce = 0;
    }
    private void ResetSelection() => SelectedID = 0;
    private void OnSelectedRecordChange()
    {
        if (SelectedRecord == null)
        {
            ResetToModVariables();
            return;
        }
        _toModName = SelectedRecord.Name;
        _toModIcon = SelectedRecord.IconName;
        _toModMass = SelectedRecord.Mass;
        _toModCapacity = SelectedRecord.Capacity;
        _toModMaxVelocity = SelectedRecord.MaxVelocity;
        if (SelectedRecord is ShipDataBaseRecord)
        {
            _toModType = VehicleType.Ship;
            _toModDisplaceMent = (SelectedRecord as ShipDataBaseRecord).Displacement;
        }
        if (SelectedRecord is CarDataBaseRecord)
        {
            _toModType = VehicleType.Car;
            _toModColor = (SelectedRecord as CarDataBaseRecord).Color;
        }
        if (SelectedRecord is BikeDataBaseRecord)
        {
            _toModType = VehicleType.Bike;
            _tomodSize = (SelectedRecord as BikeDataBaseRecord).Size;
        }
        if (SelectedRecord is PlaneDataBaseRecord)
        {
            _toModType = VehicleType.Plane;
            _toModLiftingForce = (SelectedRecord as PlaneDataBaseRecord).LiftingForce;
        }
    }
}
