using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public Dictionary<PathFinder.PATH_KIND, List<List<Node>>> MapList { get; private set; } = new Dictionary<PathFinder.PATH_KIND, List<List<Node>>>();
    private Tilemap[] fieldMap;

    public void Init()
    {
        RegistDefaultMap();
        RegistPuzzleMap();
    }

    private void RegistDefaultMap()
    {
        fieldMap = GameObject.Find("Map").GetComponentsInChildren<Tilemap>();

        Dictionary<Vector3, int> temp = new Dictionary<Vector3, int>();
        // -1 (사용 X), 0 (갈 수 있음), 1 (못 감)
        // 불가능 0,1 -> -1 / 1 -> 0
        Transform npcPoint = GameObject.Find("Point").transform;
        Vector3 sPos = Vector3.zero;
        List<Vector3> ePos = new List<Vector3>();
        List<Vector3> wPos = new List<Vector3>();

        for (int i = 0; i < npcPoint.childCount; i++)
        {
            Transform tr = npcPoint.GetChild(i);
            if (tr.name.Contains("Start"))
                sPos = tr.position;
            else if (tr.name.Contains("End"))
                ePos.Add(tr.position);
            else if (tr.name.Contains("Wait"))
                wPos.Add(tr.position);
        }

        // 타일맵 정보 모으기 (좌표, 용도)
        for (int i = 0; i < fieldMap.Length; i++)
        {
            int IsWall = fieldMap[i].GetComponent<TilemapCollider2D>() ? 1 : 0;

            foreach(var pos in fieldMap[i].cellBounds.allPositionsWithin)
            {
                Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);
                Vector3 worldPos = fieldMap[i].CellToWorld(localPos) + fieldMap[i].tileAnchor;

                if (fieldMap[i].HasTile(localPos))
                {
                    if (temp.ContainsKey(worldPos))
                    {
                        if (temp[worldPos] == 1 && IsWall == 0) continue;
                        temp[worldPos] = IsWall;
                    }
                    else
                        temp.Add(worldPos, IsWall);
                }
                else
                {
                    if (temp.ContainsKey(worldPos)) continue;
                    temp.Add(worldPos, -1);
                }
            }

            Vector3Int sLocal = fieldMap[i].WorldToCell(sPos);
            if (fieldMap[i].HasTile(sLocal))
            {
                sPos = sLocal + fieldMap[i].tileAnchor;
                //fieldMap[i].SetTileFlags(sLocal, TileFlags.None);
                //fieldMap[i].SetColor(sLocal, Color.red);
                //Debug.Log("Start: " + sPos);
            }
                
            
            for (int j = 0; j < ePos.Count; j++)
            {
                Vector3Int eLocal = fieldMap[i].WorldToCell(ePos[j]);
                if (fieldMap[i].HasTile(eLocal))
                {
                    ePos[j] = eLocal + fieldMap[i].tileAnchor;
                    //fieldMap[i].SetTileFlags(eLocal, TileFlags.None);
                    //fieldMap[i].SetColor(eLocal, Color.green);
                    //Debug.Log("End: " + ePos[j]);
                }
            }
            
            for (int j = 0; j < wPos.Count; j++)
            {
                Vector3Int wLocal = fieldMap[i].WorldToCell(wPos[j]);
                if (fieldMap[i].HasTile(wLocal))
                {
                    wPos[j] = wLocal + fieldMap[i].tileAnchor;
                    //fieldMap[i].SetTileFlags(wLocal, TileFlags.None);
                    //fieldMap[i].SetColor(wLocal, Color.blue);
                    //Debug.Log("Wait: " + wPos[j]);
                }
            }
        }

        // 이중 리스트 형태로 가공
        List<List<Node>> totalMap = new List<List<Node>>();
        List<Node> tNode = null;
        Vector2Int tId = Vector2Int.zero;

        foreach (var key in temp)
        {
            if (tNode != null && tNode[tNode.Count - 1].pos.y != key.Key.y)
            {
                totalMap.Add(tNode);
                tNode = null;
                tId.x = 0;
                tId.y++;
            }
            if (key.Value == -1) continue;
            if (tNode == null)
                tNode = new List<Node>();

            Node node = new Node();
            node.pos = key.Key;
            
            if (node.pos == sPos)
                GameManager.Instance.NpcStart = node;
            else if (ePos.Contains(node.pos))
                GameManager.Instance.NpcEndList.Add(node);
            else if (wPos.Contains(node.pos))
                GameManager.Instance.NpcWaitList.Add(node);

            node.IsClose = key.Value == 1 ? true : false;
            node.Id = tId;
            tId.x++;
            tNode.Add(node);
        }

        MapList.Add(PathFinder.PATH_KIND.DEFAULT, totalMap);

        #region TestFor Tilemap
        /* TEST FOR TILEMAP
        for (int i = 0; i < fieldMap.Length; i++)
        {
            for(int y = 0; y < totalMap.Count; y++)
            {
                for (int x = 0; x < totalMap[y].Count; x++)
                {
                    Vector3Int local = fieldMap[i].WorldToCell(totalMap[y][x].pos);
                    if (fieldMap[i].HasTile(local))
                    {
                        fieldMap[i].SetTileFlags(local, TileFlags.None);
                        if (totalMap[y][x].IsClose)
                            fieldMap[i].SetColor(local, Color.red);
                        else
                            fieldMap[i].SetColor(local, Color.green);
                    }
                }
            }
        }
        */
        #endregion
    }

    private void RegistPuzzleMap()
    {
        List<List<Node>> tPuzzle = null;
        if (!MapList.TryGetValue(PathFinder.PATH_KIND.SHISENSHO, out tPuzzle))
        {
            tPuzzle = new List<List<Node>>();
            MapList.Add(PathFinder.PATH_KIND.SHISENSHO, tPuzzle);
        }

        RectTransform puzzle = GameObject.Find("Menu").transform.Find("Collect").GetComponent<RectTransform>();
        RectTransform mRoad = puzzle.Find("MoveRoad").GetComponent<RectTransform>();
        RectTransform mBlock = puzzle.Find("BlockList").GetComponent<RectTransform>();

        GridLayoutGroup grid = mRoad.GetComponent<GridLayoutGroup>();
        Vector2 cellSize = grid.cellSize;
        Vector2 spacing = grid.spacing;
        int tX = (int)(mRoad.sizeDelta.x / (cellSize.x + spacing.x));
        int tY = (int)(mRoad.sizeDelta.y / (cellSize.y + spacing.y));

        PuzzleRoad[] roads = mRoad.GetComponentsInChildren<PuzzleRoad>();
        PuzzleBlock[] blocks = mBlock.GetComponentsInChildren<PuzzleBlock>();

        for (int y = 0; y < tY; y++)
        {
            List<Node> temp = new List<Node>();
            for (int x = 0; x < tX; x++)
            {
                PuzzleRoad tRoad = roads[(y * tX) + x];
                tRoad.node.Id = new Vector2Int(x, y);

                if ((x > 0 && y > 0) && (x < tX - 1 && y < tY - 1))
                {
                    tRoad.Block = blocks[((y - 1) * (tX - 2)) + (x - 1)];
                    tRoad.Block.Init();
                    tRoad.Block.Road = tRoad;
                    tRoad.Block.Item = GameManager.Instance.ObjPool.GetRandomItem(Items.ITEM_TYPE.BASEMATERIAL);
                    tRoad.Block.itemImg.sprite = tRoad.Block.Item.Sprite;
                    tRoad.node.IsClose = true;
                }

                tRoad.Init();
                temp.Add(tRoad.node);
            }
            tPuzzle.Add(temp);
        }
    }

}
