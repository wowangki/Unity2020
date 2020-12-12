using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MenuController
{
    // For Shisensho
    static public PuzzleRoad Start = null;
    static public PuzzleRoad End = null;
    private List<PuzzleRoad> mResult = new List<PuzzleRoad>();
    private AudioSource mAudio;

    public override void Init()
    {
        base.Init();
        mAudio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(DrawResultRoadCoroutine());
    }

    private void Update()
    {
        PathFinding();
    }

    private void PathFinding()
    {
        if (!Start || !End) return;
        if (Start.node.Id == End.node.Id
            || Start.Block.Item.ItemName != End.Block.Item.ItemName)
        {
            Start.Block.UnSelect();
            End.Block.UnSelect();
            Start = null;
            End = null;
            return;
        }

        AddResult(PathFinder.PathFinding(Start.node, End.node, PathFinder.PATH_KIND.SHISENSHO));
    }

    private void AddResult(List<Node> _result)
    {
        if(_result == null)
        {
            if (Start) Start.Block.UnSelect();
            if (End) End.Block.UnSelect();
        }
        else
        {
            for (int i = 0; i < _result.Count; i++)
            {
                mResult.Add(_result[i].Owner.GetComponent<PuzzleRoad>());
            }

            if (Start) Start.Block.BreakBlock();
            if (End) End.Block.BreakBlock();
        }

        Start = null;
        End = null;
    }

    private IEnumerator DrawResultRoadCoroutine()
    {
        WaitUntil wu = new WaitUntil(() => mResult.Count != 0);

        while (true)
        {
            yield return wu;

            for (int i = 0; i < mResult.Count; i++)
            {
                mResult[i].DrawLine();
            }

            mAudio.Play();

            yield return null;

            for (int i = 0; i < mResult.Count; i++)
            {
                mResult[i].ResetNode();
            }

            mResult.Clear();
        }
    }
}
