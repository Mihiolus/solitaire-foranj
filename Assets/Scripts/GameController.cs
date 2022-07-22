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
            _instance._model.FieldStacks[card.Stack].Remove(card);
            card.MyLocation = CardModel.Location.InEndStack;
            _instance._model.EndStack.Add(card);
            _instance._view.MoveToEndStack(card);
            return true;
        }
        else if (card.MyLocation == CardModel.Location.InDeck
        && _instance._model.LastDeckCard == card)
        {
            _instance._model.Deck.Remove(card);
            card.MyLocation = CardModel.Location.InEndStack;
            card.IsOpen = true;
            _instance._view.MoveToEndStack(card);
            _instance._model.EndStack.Add(card);
            return true;
        }
        return false;
    }
}
