using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Button _startButton;
    [SerializeField]
    Button _backButton;
    [SerializeField]
    ViewRenderer _viewRenderer;
    [SerializeField]
    Controller _controller;

    void Awake()
    {
        RenderStartMenu();
        _startButton.onClick.AddListener(RenderDataView);
        _backButton.onClick.AddListener(RenderStartMenu);
    }
    private void RenderStartMenu()
    {
        _viewRenderer.Clear();
        _startButton.gameObject.SetActive(true);
        _backButton.gameObject.SetActive(false);
    }
    private void RenderDataView()
    {
        _startButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(true);

        //Some loading animation here
        
        StartCoroutine(WaitQuerry());
    }

    IEnumerator WaitQuerry()
    {
        bool receiverd = false;
        _controller.DataChanged += (t) => { receiverd = true; _viewRenderer.RenderRequest(t);};
        _controller.QuerryData();
        while (!receiverd)
        {
            yield return null;
        }
        receiverd = false;
    }
}
