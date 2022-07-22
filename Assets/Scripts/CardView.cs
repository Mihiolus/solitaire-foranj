using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField]
    private GameObject _front, _back;

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
}
