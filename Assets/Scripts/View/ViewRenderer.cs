using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.U2D;

public class ViewRenderer : MonoBehaviour
{
    [SerializeField]
    private PanelFiller _panelFillerPrefab;
    private List<PanelFiller> _panelFillers;
    private List<DataViewContext> _contexts;
    private int _index;
    [SerializeField]
    private Transform _canvasTransform;
    private int _panelsOnScreen;
    [SerializeField]
    Controller _controller;
    [SerializeField]
    Scrollbar _scrollbar;
    [SerializeField]
    SpriteAtlas _atlas;

    private int Amount => _contexts.Count;
    public int Index 
    { 
        get => _index;
        set
        {
            if (_index != value)
            {
                _index = value;
            }
        }
    }

    void OnEnable()
    {
        _index = 0;
    }
    public void RenderRequest(List<DataViewContext> contexts)
    {
        _contexts = contexts;
        _index = 0;
        GeneratePanels();
        RenderAll();
    }
    private void Render(DataViewContext context, PanelFiller panel)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach ((string,string) stat in context.Stats)
        {
            stringBuilder.AppendFormat("{0} — {1}\n", stat.Item1, stat.Item2);
        }
        string description = stringBuilder.ToString();
        var icon = _atlas.GetSprite(context.IconName);
        panel.Fill(context.Name, description, icon);
    }
    private void RenderAll()
    {
        for (int i = 0; i < _panelsOnScreen; i++)
        {
            Render(_contexts[i+Index], _panelFillers[i]);
        }
        if (Amount > _panelsOnScreen)
        {
            _scrollbar.gameObject.SetActive(true);
        }
        else
        {
            _scrollbar.gameObject.SetActive(false);
        }
    }
    private void GeneratePanel(int x, int y)
    {
        PanelFiller panel = Instantiate(_panelFillerPrefab, _canvasTransform.transform);
        RectTransform rect = panel.transform.GetComponent<RectTransform>();
        var pos = rect.anchoredPosition;
        
        rect.anchoredPosition = new Vector2(x, y);
        _panelFillers.Add(panel);
    }
    private void GeneratePanels()
    {
        _panelFillers = new List<PanelFiller>();
        int width = Screen.width;
        int height = Screen.height;
        int dim = (int)_panelFillerPrefab.GetComponent<RectTransform>().sizeDelta.x;
        int columns = width / dim;
        _panelsOnScreen = Mathf.Min(columns, Amount);
        int rows = 1;
        int indent = (width - _panelsOnScreen * dim) / (_panelsOnScreen + 1);
        _scrollbar.size = (float)_panelsOnScreen / (float)Amount;
        _scrollbar.value = 0f;
        _scrollbar.numberOfSteps = Amount - _panelsOnScreen + 1;
        _scrollbar.onValueChanged.AddListener(OnScrollBarValueChanged);
        for (var i = 0; i < _panelsOnScreen; i++)
        {
            GeneratePanel(indent * (i + 1) + dim/2 + dim * i, 0);
        }      
    }
    public void ScrollLeft()
    {
        Index = Mathf.Clamp(--Index,0, Amount - 1);
        RenderAll();
    }
    public void ScrollRight()
    {
        Index = Mathf.Clamp(++Index, 0, Amount - _panelsOnScreen);
        RenderAll();
    }
    private void OnScrollBarValueChanged(float value)
    {
        Index = (int)((Amount - _panelsOnScreen) * value);
        RenderAll();
    }
    public void Clear()
    {
        if (_panelFillers != null)
        {
            for (var i = 0; i < _panelFillers.Count; i++)
            {
                Destroy(_panelFillers[i].gameObject);
            }
        }
        _panelFillers = null;
        _scrollbar.gameObject.SetActive(false);
        _panelsOnScreen = 0;
        _index = 0;
        _contexts = null;
        _scrollbar.onValueChanged.RemoveAllListeners();
    }
}
