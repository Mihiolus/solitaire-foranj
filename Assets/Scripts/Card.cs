using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField]
    private GameObject _front, _back;
    [SerializeField]
    private Card _ancestor, _descendant;

    public Card Ancestor
    {
        get => _ancestor; set => _ancestor = value;
    }

    public Card Descendant
    {
        get => _descendant; set => _descendant = value;
    }

    public void SetFrontImage(Sprite sprite)
    {
        _front.GetComponent<Image>().sprite = sprite;
    }

    public bool IsOpen
    {
        get
        {
            return _back.activeSelf;
        }
        set
        {
            _back.SetActive(!value);
        }
    }

    public int Rank;
}
