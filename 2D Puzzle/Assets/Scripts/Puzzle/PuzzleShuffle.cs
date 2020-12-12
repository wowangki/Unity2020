using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class PuzzleShuffle
{
    private static Dictionary<string, LinkedList<PuzzleBlock>> mItemBlock = new Dictionary<string, LinkedList<PuzzleBlock>>();
    public static Coroutine shuffle = null;

    public static void RegistBlock(PuzzleBlock block)
    {
        LinkedList<PuzzleBlock> temp = null;
        if (mItemBlock.TryGetValue(block.Item.ItemName, out temp))
            temp.AddLast(block);
        else
        {
            temp = new LinkedList<PuzzleBlock>();
            mItemBlock.Add(block.Item.ItemName, temp);
            temp.AddLast(block);
        }
    }

    public static void UnRegistBlock(PuzzleBlock block)
    {
        if (mItemBlock.ContainsKey(block.Item.ItemName))
            mItemBlock[block.Item.ItemName].Remove(block);
    }

    public static IEnumerator ShuffleCoroutine()
    {
        while (true)
        {
            if (ChkUseAblePuzzle()) yield break;
            // 셔플링
            //yield return null;

            Debug.Log("없어");
            yield break;
        }
    }

    private static bool ChkUseAblePuzzle()
    {
        foreach(KeyValuePair<string, LinkedList<PuzzleBlock>> blocks in mItemBlock)
        {
            LinkedList<PuzzleBlock> temp = blocks.Value;

            LinkedListNode<PuzzleBlock> first = temp.First;
            while (true)
            {
                Debug.Log(1);
                if (first == null) break;
                first.Value.Road.node.IsClose = false;
                LinkedListNode<PuzzleBlock> second = first.Next;
                while (true)
                {
                    if (second == null) break;
                    second.Value.Road.node.IsClose = false;
                    if (PathFinder.PathFinding(first.Value.Road.node,
                                               second.Value.Road.node,
                                               PathFinder.PATH_KIND.SHISENSHO) != null)
                    {
                        first.Value.Road.node.IsClose = true;
                        second.Value.Road.node.IsClose = true;
                        return true;
                    }

                    second.Value.Road.node.IsClose = true;
                    second = second.Next;
                }
                first.Value.Road.node.IsClose = true;
                first = first.Next;
            }
        }

        return false;
    }
}
