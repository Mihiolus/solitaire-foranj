using UnityEngine;
using UnityEngine.U2D;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public interface ISpriteLoader
{
    ///<returns> Card sprites sorted from A = 0, to King = 12;
    /// suit order not guaranteed </returns>
    SortedList<int, List<Sprite>> GetSortedSprites();
}

public class SpriteLoader : MonoBehaviour, ISpriteLoader
{
    [Header("Sprite source")]
    [SerializeField]
    [Tooltip("Sprites' names end with a letter or number indicating their rank: A,2-10,J,Q,K")]
    private SpriteAtlas _atlas;
    private readonly Regex _nameValueRegex = new Regex(@"(\d{1,2}|A|J|Q|K)(?: \(Clone\))?$");
    public List<Sprite>[] sortedSprites;

    public SortedList<int, List<Sprite>> GetSortedSprites()
    {
        Sprite[] allSprites = new Sprite[_atlas.spriteCount];
        SortedList<int, List<Sprite>> sortedSprites = new SortedList<int, List<Sprite>>();
        _atlas.GetSprites(allSprites);
        foreach (var s in allSprites)
        {
            var match = _nameValueRegex.Match(s.name);
            if (match.Success)
            {
                int rank = StringToRank(match.Value);
                if (!sortedSprites.ContainsKey(rank))
                {
                    sortedSprites.Add(rank, new List<Sprite>(4));
                }
                sortedSprites[rank].Add(s);
            }
        }
        return sortedSprites;
    }

    private int StringToRank(string s)
    {
        int rank;
        if (!int.TryParse(s, out rank))
        {
            switch (s)
            {
                case "A": rank = 1; break;
                case "J": rank = 11; break;
                case "Q": rank = 12; break;
                case "K": rank = 13; break;
            }
        }
        // Don't forget to decrement value for 0-based numbering
        return rank - 1;
    }
}
