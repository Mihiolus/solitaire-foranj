using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Generator : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas _cardFronts;

    // Start is called before the first frame update
    void Start()
    {
        var cards = GameObject.FindGameObjectsWithTag("Card");
        Dictionary<Transform, SortedList<int, Transform>> groups = new Dictionary<Transform, SortedList<int, Transform>>();
        foreach (var c in cards)
        {
            if (!groups.ContainsKey(c.transform.parent))
            {
                groups.Add(c.transform.parent, new SortedList<int, Transform>());
            }
            groups[c.transform.parent].Add(c.transform.GetSiblingIndex(), c.transform);
        }
        Sprite[] sprites = new Sprite[_cardFronts.spriteCount];
        int spriteN = _cardFronts.GetSprites(sprites);
        Card card;
        foreach (var g in groups.Keys)
        {
            for (int i = 0; i < groups[g].Count; i++)
            {
                card = groups[g][i].GetComponent<Card>();
                if (i > 0)
                {
                    card.Ancestor = groups[g][i - 1].GetComponent<Card>();
                }
                if (i < groups[g].Count - 1)
                {
                    card.Descendant = groups[g][i + 1].GetComponent<Card>();
                }
                var sprite = sprites[Random.Range(0, spriteN)];
                card.SetFrontImage(sprite);
                if (i == groups[g].Count - 1)
                {
                    card.IsOpen = true;
                }
            }
        }
    }
}
