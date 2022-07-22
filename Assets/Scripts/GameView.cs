using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField]
    private RectTransform _endStack;

    public void MoveToEndStack(CardModel card)
    {
        card.transform.SetParent(_endStack);
    }
}
