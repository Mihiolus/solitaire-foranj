using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameObject _front, _back;
    private CardModel _model;
    private void Awake() {
       _model = GetComponent<CardModel>();
    }

    public void SetFrontImage(Sprite sprite)
    {
        _front.GetComponent<Image>().sprite = sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameController.TryMoveCard(_model);
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
