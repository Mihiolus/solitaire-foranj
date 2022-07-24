using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    private GameModel _model;
    private GameView _view;

    private void Awake()
    {
        _instance = this;
        _model = GetComponent<GameModel>();
        _view = GetComponent<GameView>();
    }

    public static bool TryMoveCard(CardModel card)
    {
        if (card.MyLocation == CardModel.Location.OnField
        && card.IsOpen
        && _instance._model.LastComboCard.Accepts(card))
        {
            var index = _instance._model.FieldStacks[card.Stack].IndexOf(card);
            _instance._model.FieldStacks[card.Stack].Remove(card);
            _instance.AddToEndStack(card);
            if (card.Ancestor)
            {
                card.Ancestor.IsOpen = true;
            }
            return true;
        }
        else if (card.MyLocation == CardModel.Location.InDeck
        && _instance._model.LastDeckCard == card)
        {
            _instance._model.Deck.Remove(card);
            card.IsOpen = true;
            _instance.AddToEndStack(card);
            return true;
        }
        return false;
    }

    private void AddToEndStack(CardModel card)
    {
        card.MyLocation = CardModel.Location.InEndStack;
        _model.EndStack.Add(card);
        _view.MoveToEndStack(card);
    }
}
