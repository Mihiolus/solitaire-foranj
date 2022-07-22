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
    private List<Sprite>[] sortedSprites;
    [SerializeField]
    private GameModel _model;

    // Start is called before the first frame update
    void Start()
    {
        var groups = GetSortedGroups();
        sortedSprites = GetComponent<ISpriteLoader>().GetSortedSprites();
        var combos = GetCombinations();
        List<CardModel> deck;
        InitShuffle(groups, combos, out deck);
        _model.Init(groups, deck);
        CardModel card;
        foreach (var g in groups)
        {
            for (int i = 0; i < g.Count; i++)
            {
                card = g[i].GetComponent<CardModel>();
                if (i > 0)
                {
                    card.Ancestor = g[i - 1].GetComponent<CardModel>();
                }
                if (i < g.Count - 1)
                {
                    card.Descendant = g[i + 1].GetComponent<CardModel>();
                }
                if (i == g.Count - 1)
                {
                    card.IsOpen = true;
                }
            }
        }
        GameController.TryMoveCard(_model.Deck[_model.Deck.Count - 1]);
    }

    ///<returns>Groups of card transforms as they are on the field under different parent transforms, sorted according to their order in the inspector</returns>
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

    private List<List<int>> GetCombinations()
    {
        List<List<int>> results = new List<List<int>>();
        int remainingCards = 40;
        int start, number, value;
        bool ascending;
        while (remainingCards > 0)
        {
            start = Random.Range(0, 13);
            results.Add(new List<int>());
            results[results.Count - 1].Add(start);
            value = start;
            number = Mathf.Min(Random.Range(2, 8), remainingCards);
            ascending = Random.value < _ascendingChance;
            while (number > 0)
            {
                value += ascending ? 1 : -1;
                if (value > 12)
                {
                    value = 0;
                }
                else if (value < 0)
                {
                    value = 12;
                }
                results[results.Count - 1].Add(value);
                number--;
                remainingCards--;
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
            deckCard.GetComponent<CardView>().SetFrontImage(sortedSprites[cardModel.Rank][Random.Range(0, 4)]);
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
                cardModel.GetComponent<CardView>().SetFrontImage(sortedSprites[cardModel.Rank][Random.Range(0, 4)]);
                lastIndexInGroup[group]--;
            }
        }
        deck.Reverse();
    }
}
