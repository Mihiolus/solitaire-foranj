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
        var groups = GetSortedGroups();
        _sortedSprites = GetComponent<ISpriteLoader>().GetSortedSprites();
        int cardsOnField = CountCards(groups);
        var combos = GetCombinations(cardsOnField);
        List<CardModel> deck;
        InitShuffle(groups, combos, out deck);
        _model.Init(groups, deck);
        GameController.TryMoveCard(_model.Deck[_model.Deck.Count - 1]);
    }

    private static int CountCards(List<CardModel>[] groups)
    {
        int cardsOnField = 0;
        foreach (var stack in groups)
        {
            cardsOnField += stack.Count;
        }

        return cardsOnField;
    }

    ///<returns>Groups of cards as they are on the field under different parent transforms, sorted according to their order in the inspector</returns>
    private List<CardModel>[] GetSortedGroups()
    {
        Dictionary<Transform, SortedList<int, Transform>> groups = new Dictionary<Transform, SortedList<int, Transform>>();
        var cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (var c in cards)
        {
            if (!groups.ContainsKey(c.transform.parent))
            {
                groups.Add(c.transform.parent, new SortedList<int, Transform>());
            }
            groups[c.transform.parent].Add(c.transform.GetSiblingIndex(), c.transform);
        }

        //Convert
        var results = new List<CardModel>[groups.Keys.Count];
        int groupIndex = 0;
        foreach (var key in groups.Keys)
        {
            int cardIndex = 0;
            results[groupIndex] = new List<CardModel>(groups[key].Count);
            foreach (var item in groups[key])
            {
                results[groupIndex].Add(item.Value.GetComponent<CardModel>());
                cardIndex++;
            }
            groupIndex++;
        }
        return results;
    }

    ///<returns>Combinations of card ranks as ints</returns>
    private List<List<int>> GetCombinations(int cardsOnField)
    {
        List<List<int>> results = new List<List<int>>();
        int length, rank;
        bool ascending;
        while (cardsOnField > 0)
        {
            rank = Random.Range(0, 13);
            results.Add(new List<int>());
            results[results.Count - 1].Add(rank);
            length = Mathf.Min(Random.Range(2, 8), cardsOnField);
            cardsOnField -= length;
            ascending = Random.value < _ascendingChance;
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

    private void InitShuffle(List<CardModel>[] cardGroups, List<List<int>> combinations, out List<CardModel> deck)
    {
        int[] lastIndexInGroup = new int[cardGroups.Length];
        for (int i = 0; i < cardGroups.Length; i++)
        {
            lastIndexInGroup[i] = 9;
        }
        deck = new List<CardModel>();
        foreach (var combo in combinations)
        {
            var deckCard = Instantiate(_deckPrefab, _deck);
            deckCard.transform.SetAsFirstSibling();
            CardModel cardModel = deckCard.GetComponent<CardModel>();
            cardModel.Rank = combo[0];
            deckCard.GetComponent<CardView>().SetFrontImage(_sortedSprites[cardModel.Rank][Random.Range(0, 4)]);
            deck.Add(cardModel);
            for (int i = 1; i < combo.Count; i++)
            {
                int value = combo[i];
                int group;
                do
                {
                    group = Random.Range(0, cardGroups.Length);
                } while (lastIndexInGroup[group] < 0);
                cardModel = cardGroups[group][lastIndexInGroup[group]];
                cardModel.Rank = value;
                cardModel.Stack = group;
                cardModel.GetComponent<CardView>().SetFrontImage(_sortedSprites[cardModel.Rank][Random.Range(0, 4)]);
                lastIndexInGroup[group]--;
            }
        }
        deck.Reverse();
    }
}
