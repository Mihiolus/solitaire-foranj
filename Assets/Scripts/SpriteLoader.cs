using UnityEngine;

public interface ISpriteLoader
{
    ///<returns> Card sprites sorted from A = 0, to King = 12;
    /// suit order not guaranteed </returns>
    Sprite[] GetSortedSprites();
}

public class SpriteLoader : MonoBehaviour, ISpriteLoader
{
    [Header("Sprite source")]
    [SerializeField]
    [Tooltip("Looks for the texture in the Resources folder")]
    private Texture2D _cardTexture;

    public Sprite[] GetSortedSprites()
    {
        return Resources.LoadAll<Sprite>(_cardTexture.name);
    }
}
