using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Card sequence probabilities")]
    [SerializeField]
    [Range(0, 1)]
    private float _ascendingChance = 0.65f, _directionChangeChance = 0.15f;
    [SerializeField]
    private GameObject _deckPrefab;
    [SerializeField]
    private RectTransform _deck;
    private List<Sprite>[] _sortedSprites;
    [SerializeField]
    private GameModel _model;

    // Start is called before the first frame update
    void Start()
    {
        var stacks = GetSortedStacks();
        _sortedSprites = GetComponent<ISpriteLoader>().GetSortedSprites();
        int cardsOnField = CountCards(stacks);
        var combos = GetCombinations(cardsOnField);
        List<CardModel> deck;
        InitShuffle(stacks, combos, out deck);
        _model.Init(stacks, deck);
        GameController.TryMoveCard(_model.Deck[_model.Deck.Count - 1]);
    }

    private static int CountCards(List<CardModel>[] stacks)
    {
        int cardsOnField = 0;
        foreach (var stack in stacks)
        {
            cardsOnField += stack.Count;
        }

        return cardsOnField;
    }

    ///<returns>Stacks of cards as they are on the field under different parent transforms, sorted according to their order in the inspector</returns>
    private List<CardModel>[] GetSortedStacks()
    {
        Dictionary<Transform, SortedList<int, Transform>> stacks = new Dictionary<Transform, SortedList<int, Transform>>();
        var cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (var c in cards)
        {
            if (!stacks.ContainsKey(c.transform.parent))
            {
                stacks.Add(c.transform.parent, new SortedList<int, Transform>());
            }
            stacks[c.transform.parent].Add(c.transform.GetSiblingIndex(), c.transform);
        }

        //Convert
        var results = new List<CardModel>[stacks.Keys.Count];
        int stackIndex = 0;
        foreach (var key in stacks.Keys)
        {
            int cardIndex = 0;
            results[stackIndex] = new List<CardModel>(stacks[key].Count);
            foreach (var item in stacks[key])
            {
                results[stackIndex].Add(item.Value.GetComponent<CardModel>());
                cardIndex++;
            }
            stackIndex++;
        }
        return results;
    }

    ///<returns>Combinations of card ranks as ints</returns>
    private List<List<int>> GetCombinations(int cardsOnField)
    {
        List<List<int>> results = new List<List<int>>();
        while (cardsOnField > 0)
        {
            var rank = Random.Range(0, 13);
            results.Add(new List<int>());
            results[results.Count - 1].Add(rank);
            var length = Mathf.Min(Random.Range(2, 8), cardsOnField);
            cardsOnField -= length;
            var ascending = Random.value < _ascendingChance;
            while (length > 0)
            {
                rank += ascending ? 1 : -1;
                if (rank > 12)
                {
                    rank = 0;
                }
                else if (rank < 0)
                {
                    rank = 12;
                }
                results[results.Count - 1].Add(rank);
                length--;
                ascending = Random.value < _directionChangeChance ? !ascending : ascending;
            }
        }
        return results;
    }

    ///<summary>Completely initializes the layout of cards on the field and the deck</summary>
    private void InitShuffle(List<CardModel>[] cardStacks, List<List<int>> combinations, out List<CardModel> deck)
    {
        int[] lastIndexInStack = new int[cardStacks.Length];
        for (int i = 0; i < cardStacks.Length; i++)
        {
            lastIndexInStack[i] = cardStacks[i].Count - 1;
        }
        deck = new List<CardModel>();
        foreach (var combo in combinations)
        {
            deck.Add(CreateDeckCard(combo[0]));
            for (int i = 1; i < combo.Count; i++)
            {
                int rank = combo[i];
                int stack;
                do
                {
                    stack = Random.Range(0, cardStacks.Length);
                } while (lastIndexInStack[stack] < 0);
                var cardModel = cardStacks[stack][lastIndexInStack[stack]];
                InitFieldCard(rank, stack, cardModel);
                lastIndexInStack[stack]--;
            }
        }
        deck.Reverse();
    }

    private void InitFieldCard(int rank, int stack, CardModel cardModel)
    {
        cardModel.Rank = rank;
        cardModel.Stack = stack;
        cardModel.GetComponent<CardView>().SetFrontImage(_sortedSprites[cardModel.Rank][Random.Range(0, 4)]);
    }

    private CardModel CreateDeckCard(int rank)
    {
        var deckCard = Instantiate(_deckPrefab, _deck);
        deckCard.transform.SetAsFirstSibling();
        CardModel cardModel = deckCard.GetComponent<CardModel>();
        cardModel.Rank = rank;
        deckCard.GetComponent<CardView>().SetFrontImage(_sortedSprites[cardModel.Rank][Random.Range(0, 4)]);
        return cardModel;
    }
}
