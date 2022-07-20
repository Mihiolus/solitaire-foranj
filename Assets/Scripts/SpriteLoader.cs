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

    public SortedList<int, List<Sprite>> GetSortedSprites()
    {
        Sprite[] allSprites = new Sprite[_atlas.spriteCount];
        SortedList<int, List<Sprite>> sortedSprites = new SortedList<int, List<Sprite>>();
        _atlas.GetSprites(allSprites);
        var regex = new Regex(@"(\d{1,2}|A|J|Q|K)$");
        foreach (var s in allSprites)
        {
            //Copy because the sprite needs the original name
            string name = s.name;
            //Remove the "(Clone)" suffix that Unity adds to packed sprites
            int index = name.IndexOf("(Clone)");
            name = name.Remove(index);
            //Regex for 1-2 digits or letter at the end
            var match = regex.Match(name);
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
                // Don't forget to decrement value for 0-based numbering
                value--;
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
