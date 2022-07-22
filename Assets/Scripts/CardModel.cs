using UnityEngine;

public class CardModel : MonoBehaviour
{
    [SerializeField]
    private CardModel _ancestor, _descendant;

    public CardModel Ancestor
    {
        get => _ancestor; set => _ancestor = value;
    }

    public CardModel Descendant
    {
        get => _descendant; set => _descendant = value;
    }
    public int Rank { get => _rank; set => _rank = value; }

    private int _rank;
}
