using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelFiller : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private TMP_Text _descriptionText;
    [SerializeField]
    private Image _iconImage;

    public void Fill(string name, string description, Sprite sprite = null)
    {
        _nameText.text = name;
        _descriptionText.text = description;
        _iconImage.sprite = sprite;
    }
}
