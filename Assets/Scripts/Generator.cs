using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField]
    private int _groupsOnField = 4;

    // Start is called before the first frame update
    void Start()
    {
        var cards = GameObject.FindGameObjectsWithTag("Card");
        var cardSize = cards[0].GetComponent<RectTransform>().rect;
        //Find field dimensions
        var bottomLeft = Camera.main.ViewportToScreenPoint(Vector3.zero);
        var topRight = Camera.main.ViewportToScreenPoint(Vector3.one);
        Vector3 randPos = new Vector3();
        foreach (var c in cards)
        {
            randPos.x = Random.Range(bottomLeft.x + cardSize.width,topRight.x - cardSize.width);
            randPos.y = Random.Range(bottomLeft.y + cardSize.height, topRight.y - cardSize.height);
            c.transform.position = randPos;
        }
    }
}
