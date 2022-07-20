using UnityEngine;
using UnityEngine.U2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public interface ISpriteLoader
{
    ///<returns> Card sprites sorted from A = 1, to King = 13;
    /// suit order not guaranteed </returns>
    SortedList<int, List<Sprite>> GetSortedSprites();
}

public class SpriteLoader : MonoBehaviour, ISpriteLoader
{
    [Header("Sprite source")]
    [SerializeField]
    [Tooltip("Sprites' names end with a letter or number indicating their rank: A,2-10,J,Q,K")]
    private SpriteAtlas _atlas;

    public SortedList<int, List<Sprite>> GetSortedSprites()
    {
        Sprite[] allSprites = new Sprite[_atlas.spriteCount];
        SortedList<int, List<Sprite>> sortedSprites = new SortedList<int, List<Sprite>>();
        _atlas.GetSprites(allSprites);
        foreach (var s in allSprites)
        {
            string name = string.Copy(s.name);
            int index = name.IndexOf("(Clone)");
            name = name.Remove(index);
            var match = Regex.Match(name, @"(\d{1,2}|A|J|Q|K)$");
            if (match.Success)
            {
                int value;

                if (!int.TryParse(match.Value, out value))
                {
                    switch (match.Value)
                    {
                        case "A": value = 1; break;
                        case "J": value = 11; break;
                        case "Q": value = 12; break;
                        case "K": value = 13; break;
                    }
                }
                if (!sortedSprites.ContainsKey(value))
                {
                    sortedSprites.Add(value, new List<Sprite>(4));
                }
                sortedSprites[value].Add(s);
            }
        }
        return sortedSprites;
    }
}
