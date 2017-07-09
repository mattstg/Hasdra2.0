using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TagButton : MonoBehaviour {

    FilterTagManager tagManager;
    public Text buttonTitle;
    public bool tagSelected = false;

    public void Initialize(TagManager _tagManager, string tagName)
    {
        tagManager = (FilterTagManager)_tagManager;
        tagSelected = false;
        buttonTitle.text = tagName;
        GenerateRandomColor();
        SetTransparency();
    }

    public void ToggleTag()
    {
        tagSelected = !tagSelected;
        SetTransparency();
        tagManager.ToggleFilter(buttonTitle.text);
    }

    private void GenerateRandomColor()
    {
        float r,g,b;
        r = Random.Range(0, 1f);
        g = Random.Range(0, 1f);
        b = Random.Range(0, 1f);
        GetComponent<Button>().image.color = new Color(r, g, b, 1);
    }

    private void SetTransparency()
    {
        Color color = GetComponent<Image>().color;
        color.a = (tagSelected) ? 1 : GV.TAG_UNSELECTED_TRANSPARENCY;
        GetComponent<Button>().image.color = color;
    }
}
