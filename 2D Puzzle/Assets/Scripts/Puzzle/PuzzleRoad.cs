using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleRoad : MonoBehaviour
{
    public PuzzleBlock Block { get; set; } = null;
    public Text text;
    public GameObject[] mLine = new GameObject[5];
    public Node node = new Node();

    public void Init()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            mLine[i] = transform.GetChild(0).GetChild(i).gameObject;

        node.Owner = gameObject;
        //TestText();
        //gameObject.name = "Road" + "{" + Id.x.ToString() + "," + Id.y.ToString() + "}";
    }

    public void ResetNode()
    {
        node.ResetNode();

        for (int i = 0; i < mLine.Length; i++)
        {
            mLine[i].gameObject.SetActive(false);
        }
    }

    public void DrawLine()
    {
        Node tParent = node.Parent;
        if (tParent == null) return;
        PuzzleRoad parentRoad = null;

        if (tParent.Owner.TryGetComponent(out parentRoad))
        {
            parentRoad.mLine[0].SetActive(true);
            parentRoad.mLine[(int)node.Direction].SetActive(true);
        }
        else return;
        
        mLine[0].SetActive(true);
        mLine[5 - (int)node.Direction].SetActive(true);

        //Test용
        //switch (Direction)
        //{
        //    case ROAD_DIRECTION.UP:
        //        text.text = "U";
        //        break;
        //    case ROAD_DIRECTION.LEFT:
        //        text.text = "L";
        //        break;
        //    case ROAD_DIRECTION.RIGHT:
        //        text.text = "R";
        //        break;
        //    case ROAD_DIRECTION.DOWN:
        //        text.text = "D";
        //        break;
        //}
    }

    private void SetAlpha (Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private void TestText()
    {
        text = gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.raycastTarget = false;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.blue;
        text.fontSize = 20;
    }

    public void TestActivatingLine(bool active)
    {
        for (int i = 0; i < mLine.Length; i++)
        {
            mLine[i].gameObject.SetActive(active);
            text.color = Color.blue;
        }
    }


    public void TestShowCost()
    {
        //text.text = "Ts: " + DistToStart + "\nTe: " + DistToEnd + "\nT: " + TotalDist;
        text.text = "Ts: " + node.DistToStart + "\nT: " + node.TotalDist;
        //TestActivatingLine(false);
        //mLine[0].SetActive(true);
        //mLine[5 - (int)Direction].SetActive(true);
    }
}
