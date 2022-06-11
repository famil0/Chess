using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Controller : MonoBehaviour
{
    public Transform selected;
    public Transform clicked;
    public float animTime = 0.5f;
    public List<GameObject> players = new List<GameObject>();
    public PieceColor activePlayer;
    public bool canCheck = true;
    public List<Piece> pieces = new List<Piece>();

    private void Start()
    {
        foreach (Transform child in GameObject.Find("Whites").transform)
        {
            pieces.Add(child.gameObject.GetComponent<Piece>());
        }
        foreach (Transform child in GameObject.Find("Blacks").transform)
        {
            pieces.Add(child.gameObject.GetComponent<Piece>());
        }
    }

    public void Move()
    {
        canCheck = false;
        Transform tmp = selected;
        Piece p = selected.GetComponent<Piece>();
        p.outline.enabled = false;
        p.outline.OutlineColor = p.outlineColor;
        p.moved = true;
        selected.transform.DOMove(clicked.position, animTime).OnComplete(() => canCheck = true );
        Select(selected.gameObject);
        activePlayer = players.Count - 1 - activePlayer;
        selected = null;
    }

    private void LateUpdate()
    {
        foreach (Piece p in pieces)
        {
            foreach (GameObject slot in p.path)
            {
                if (slot.GetComponent<Slot>().dangeredBy.Contains(p.gameObject) is false)
                    slot.GetComponent<Slot>().dangeredBy.Add(p.gameObject);
                slot.GetComponent<Slot>().isDangered = true;
            }
        }
    }

    public void Select(GameObject g)
    {
        g.GetComponent<Piece>().isSelected = !g.GetComponent<Piece>().isSelected;
        Piece p = g.GetComponent<Piece>();
        if (p.isSelected)
        {
            g.layer = LayerMask.NameToLayer("Ignore Raycast");
            selected = g.transform;
            g.transform.DOLocalMoveY(p.zeroY + 1, animTime);

            Check(p, false);
        }
        else
        {
            g.transform.DOLocalMoveY(p.zeroY, animTime);
            g.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void Check(Piece p, bool onlyDangers)
    {
        int c = p.color == PieceColor.White ? 1 : -1;
        Func<int, int> op = new Func<int, int>(x => x = x + c);

        if (p.type == PieceType.Pawn)
        {
            p.FindPathForward(!p.moved ? 2 : 1, op, onlyDangers);
        }
        else if (p.type == PieceType.Bishop)
        {
            p.FindPathDiag(7, op, onlyDangers);
        }
        else if (p.type == PieceType.Knight)
        {
            p.FindPathKnight(onlyDangers);
        }
        else if (p.type == PieceType.Queen)
        {
            p.FindPathStraight(7, op, onlyDangers);
            p.FindPathDiag(7, op, onlyDangers);
        }
        else if (p.type == PieceType.King)
        {
            p.FindPathStraight(1, op, onlyDangers);
            p.FindPathDiag(1, op, onlyDangers);
        }
        else if (p.type == PieceType.Rook)
        {
            p.FindPathStraight(7, op, onlyDangers);
        }
    }

}
