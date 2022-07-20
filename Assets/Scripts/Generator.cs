using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Card sequence probabilities")]
    [SerializeField]
    [Range(0, 1)]
    private float _ascendingChance = 0.65f, _directionChangeChance = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        Transform[][] groups = GetSortedGroups();
        var sprites = GetComponent<ISpriteLoader>().GetSortedSprites();
        var combos = GetCombinations();
        Card card;
        Sprite sprite;
        foreach (var g in groups)
        {
            for (int i = 0; i < g.Length; i++)
            {
                card = g[i].GetComponent<Card>();
                if (i > 0)
                {
                    card.Ancestor = g[i - 1].GetComponent<Card>();
                }
                if (i < g.Length - 1)
                {
                    card.Descendant = g[i + 1].GetComponent<Card>();
                }
                sprite = sprites[Random.Range(0, sprites.Count)][Random.Range(0, 4)];
                card.SetFrontImage(sprite);
                if (i == g.Length - 1)
                {
                    card.IsOpen = true;
                }
            }
        }
    }

    ///<returns>Groups of card transforms as they are on the field under different parent transforms, sorted according to their order in the inspector</returns>
    private static Transform[][] GetSortedGroups()
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
        
        //Convert to a simple array
        Transform[][] results = new Transform[4][];
        int groupIndex = 0;
        foreach (var key in groups.Keys)
        {
            int cardIndex = 0;
            results[groupIndex] = new Transform[10];
            foreach (var item in groups[key])
            {
                Debug.Log("["+groupIndex+"]["+cardIndex+"]");
                results[groupIndex][cardIndex] = item.Value;
                cardIndex ++;
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
}
