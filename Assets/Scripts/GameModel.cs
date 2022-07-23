using System.Collections.Generic;
using UnityEngine;

public class GameModel : MonoBehaviour
{
    private List<CardModel> _deck = new List<CardModel>();
    private List<CardModel>[] _fieldStacks;
    private List<CardModel> _endStack = new List<CardModel>();

    public CardModel LastComboCard { get => _endStack[_endStack.Count - 1]; }
    public CardModel LastDeckCard { get => _deck[_deck.Count - 1]; }

    public List<CardModel> Deck { get => _deck; }
    public List<CardModel>[] FieldStacks { get => _fieldStacks; }
    public List<CardModel> EndStack { get => _endStack; }

    public void Init(List<CardModel>[] fieldStacks, List<CardModel> deck)
    {
        _fieldStacks = fieldStacks;
        foreach (var stack in fieldStacks)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                var card = stack[i];
                if (i > 0)
                {
                    card.Ancestor = stack[i - 1];
                }
                if (i < stack.Count - 1)
                {
                    card.Descendant = stack[i + 1];
                }
                if (i == stack.Count - 1)
                {
                    card.IsOpen = true;
                }
                card.MyLocation = CardModel.Location.OnField;
            }
        }
        _deck = deck;
        foreach (var card in deck)
        {
            card.MyLocation = CardModel.Location.InDeck;
        }
    }
}
