using UnityEngine;

public class CardModel : MonoBehaviour
{
    [SerializeField]
    private CardModel _ancestor, _descendant;
    private CardView _view;

    public CardModel Ancestor
    {
        get => _ancestor; set => _ancestor = value;
    }

    public CardModel Descendant
    {
        get => _descendant; set => _descendant = value;
    }
    public int Rank { get => _rank; set => _rank = value; }
    public int Stack{ get; set;}

    public bool IsOpen
    {
        get => _isOpen; set
        {
            _isOpen = value;
            _view.IsOpen = value;
        }
    }

    public enum Location { InDeck, OnField, InEndStack }
    public Location MyLocation { get; set; }

    private void Awake()
    {
        _view = GetComponent<CardView>();
    }

    private int _rank;
    private bool _isOpen;

    public bool Accepts(CardModel other)
    {
        return other.Rank == Rank + 1 || other.Rank == Rank - 1 || other.Rank == Rank + 12 || other.Rank == Rank - 12;
    }
}
