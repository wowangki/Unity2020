using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathFinder
{
    public enum PATH_KIND
    {
        DEFAULT,
        SHISENSHO
    }

    public static List<Node> PathFinding(Node Start, Node End, PATH_KIND kind = PATH_KIND.DEFAULT)
    {
        List<List<Node>> total = GameManager.Instance.Map.MapList[kind];
        Dictionary<Vector2Int, Node> openList = new Dictionary<Vector2Int, Node>();
        Dictionary<Vector2Int, Node> closeList = new Dictionary<Vector2Int, Node>();
        closeList.Add(Start.Id, Start);

        List<Node> result = null;
        switch (kind)
        {
            case PATH_KIND.DEFAULT:
                result = PathFinding(Start, End, total, openList, closeList);
                break;
            case PATH_KIND.SHISENSHO:
                int turnCount = 0;
                result = PathFinding(Start, End, total, openList, closeList, turnCount);
                break;
            default:
                break;
        }

        return result;
    }

    private static List<Node> PathFinding(Node Start, Node End,
                                   List<List<Node>> _Total,
                                   Dictionary<Vector2Int, Node> _OpenList,
                                   Dictionary<Vector2Int, Node> _CloseList)
    {
        AddOpenList(Start, End, _Total, _OpenList, _CloseList);
        Node dest = FindBetterPath(_OpenList, Start);

        if (dest == null)
        {
            UnArriveReset(_OpenList, _CloseList);
            return null;
        }

        if(dest == End)
        {
            List<Node> result = new List<Node>();
            while (true)
            {
                result.Add(dest);
                if (dest.Parent == null) break;
                dest = dest.Parent;
            }

            return result;
        }

        _OpenList.Remove(dest.Id);
        if (!_CloseList.ContainsKey(dest.Id))
            _CloseList.Add(dest.Id, dest);

        return PathFinding(dest, End, _Total, _OpenList, _CloseList);
    }

    private static void AddOpenList(Node cur, Node _End, 
                                    List<List<Node>> _total, Dictionary<Vector2Int, Node> _openList, 
                                    Dictionary<Vector2Int, Node> _closeList)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue;
                if (Mathf.Abs(x + y) != 1) continue;

                Vector2Int tId = new Vector2Int(cur.Id.x + x, cur.Id.y + y);
                if (tId.y < 0 || tId.y >= _total.Count) continue;
                if (tId.x < 0 || tId.x >= _total[tId.y].Count) continue;
                if (_closeList.ContainsKey(tId)) continue;

                Node temp = _total[tId.y][tId.x];
                if (temp.IsClose) continue;

                Node.NODE_DIRECTION tDir = Node.NODE_DIRECTION.NONE;
                if (y == -1 && x == 0) tDir = Node.NODE_DIRECTION.UP;
                else if (y == 0 && x == -1) tDir = Node.NODE_DIRECTION.LEFT;
                else if (y == 0 && x == 1) tDir = Node.NODE_DIRECTION.RIGHT;
                else if (y == 1 && x == 0) tDir = Node.NODE_DIRECTION.DOWN;

                float distToStart = 10 + cur.DistToStart;
                if (_openList.ContainsKey(tId))
                {
                    if (temp.DistToStart >= distToStart)
                    {
                        temp.Direction = tDir;
                        temp.DistToStart = distToStart;
                        temp.Parent = cur;
                    }
                }
                else
                {
                    temp.Direction = tDir;
                    temp.DistToStart = distToStart;
                    temp.DistToEnd = Mathf.RoundToInt(Vector2Int.Distance(temp.Id, _End.Id) * 10);
                    temp.Parent = cur;
                    _openList.Add(tId, temp);
                }
            }
        }
    }

    private static List<Node> PathFinding(Node Start, Node End,
                                   List<List<Node>> _Total,
                                   Dictionary<Vector2Int, Node> _OpenList,
                                   Dictionary<Vector2Int, Node> _CloseList,
                                   int _turnCount)
    {
        AddOpenListForShisensho(Start, End, _Total, _OpenList, _CloseList);
        Node dest = FindBetterPath(_OpenList, Start);

        if (dest == null)
        {
            UnArriveReset(_OpenList, _CloseList);
            return null;
        }

        // 도착점에 도착
        if (dest == End)
        {
            List<Node> result = new List<Node>();

            while (true)
            {
                result.Add(dest);
                if (dest.Parent == null) break;
                if (dest.Direction != dest.Parent.Direction)
                    _turnCount++;
                dest = dest.Parent;
            }

            if (_turnCount > 3)
            {
                UnArriveReset(_OpenList, _CloseList);
                return null;
            }
            else
            {
                ArriveReset(_OpenList, _CloseList, result);
                return result;
            }
        }

        _OpenList.Remove(dest.Id);
        if (!_CloseList.ContainsKey(dest.Id))
            _CloseList.Add(dest.Id, dest);

        return PathFinding(dest, End, _Total, _OpenList, _CloseList, _turnCount);
    }

    private static void AddOpenListForShisensho(Node cur,
                                         Node _End,
                                         List<List<Node>> _total,
                                         Dictionary<Vector2Int, Node> _openList,
                                         Dictionary<Vector2Int, Node> _closeList)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue;
                if (Mathf.Abs(x + y) != 1) continue;

                Vector2Int tId = new Vector2Int(cur.Id.x + x, cur.Id.y + y);
                if (tId.y < 0 || tId.y >= _total.Count) continue;
                if (tId.x < 0 || tId.x >= _total[tId.y].Count) continue;
                if (_closeList.ContainsKey(tId)) continue;

                Node temp = _total[tId.y][tId.x];
                if (temp.IsClose) continue;

                Node.NODE_DIRECTION tDir = Node.NODE_DIRECTION.NONE;
                if (y == -1 && x == 0) tDir = Node.NODE_DIRECTION.UP;
                else if (y == 0 && x == -1) tDir = Node.NODE_DIRECTION.LEFT;
                else if (y == 0 && x == 1) tDir = Node.NODE_DIRECTION.RIGHT;
                else if (y == 1 && x == 0) tDir = Node.NODE_DIRECTION.DOWN;

                float distToStart = tDir == cur.Direction ? 10 + cur.DistToStart : 14 + cur.DistToStart;
                if (_openList.ContainsKey(tId))
                {
                    if (temp.DistToStart >= distToStart)
                    {
                        temp.Direction = tDir;
                        temp.DistToStart = distToStart;
                        temp.Parent = cur;
                    }
                }
                else
                {
                    temp.Direction = tDir;
                    temp.DistToStart = distToStart;
                    temp.DistToEnd = Mathf.RoundToInt(Vector2Int.Distance(temp.Id, _End.Id) * 10);
                    temp.Parent = cur;
                    _openList.Add(tId, temp);
                }
            }
        }
    }

    private static Node FindBetterPath(Dictionary<Vector2Int, Node> _OpenList, Node _Current)
    {
        float totalCost = 9999999f;
        Node result = null;
        foreach (KeyValuePair<Vector2Int, Node> openRoad in _OpenList)
        {
            Node _temp = openRoad.Value;
            // 가장 적은 비용의 노드 선정
            if (_temp.TotalDist < totalCost)
            {
                totalCost = _temp.TotalDist;
                result = _temp;
            }
        }

        return result;
    }

    private static void UnArriveReset(Dictionary<Vector2Int, Node> _OpenList,
                                      Dictionary<Vector2Int, Node> _CloseList)
    {
        foreach(KeyValuePair<Vector2Int, Node> openRoad in _OpenList)
        {
            openRoad.Value.ResetNode();
        }

        foreach(KeyValuePair<Vector2Int, Node> closeRoad in _CloseList)
        {
            closeRoad.Value.ResetNode();
        }
    }

    private static void ArriveReset(Dictionary<Vector2Int, Node> _OpenList,
                             Dictionary<Vector2Int, Node> _CloseList, 
                             List<Node> result)
    {
        foreach(KeyValuePair<Vector2Int, Node> openRoad in _OpenList)
        {
            if (result.Contains(openRoad.Value)) continue;
            openRoad.Value.ResetNode();
        }

        foreach(KeyValuePair<Vector2Int, Node> closeRoad in _CloseList)
        {
            if (result.Contains(closeRoad.Value)) continue;
            closeRoad.Value.ResetNode();
        }
    }
}
