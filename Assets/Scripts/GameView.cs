using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField]
    private RectTransform _endStack;

    public void MoveToEndStack(CardModel card)
    {
        card.transform.SetParent(_endStack, false);
        card.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
