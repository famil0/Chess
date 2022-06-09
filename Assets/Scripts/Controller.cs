using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;

public class Controller : NetworkBehaviour
{
    [SyncVar] public Transform selected;
    [SyncVar] public Transform clicked;
    [SyncVar] public float animTime = 0.5f;
    [SyncVar] public List<GameObject> players = new List<GameObject>();
    [SyncVar] public int activePlayerIdx;
    [SyncVar] public NetworkIdentity identity;
    
    private void Start()
    {                
        identity = GetComponent<NetworkIdentity>();
    }

    [Command]
    
    public void Move()
    {
        selected.transform.DOLocalMove(clicked.position, animTime);
        activePlayerIdx = players.Count - 1 - activePlayerIdx;
    }

    [Command]
    public void Select(GameObject g)
    {
        g.GetComponent<Piece>().selected = !g.GetComponent<Piece>().selected;
        Piece p = g.GetComponent<Piece>();
        if (p.selected)
        {
            g.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        else
        {
            g.layer = LayerMask.NameToLayer("Default");
            
        }
        if (p.selected)
        {
            selected = g.transform;
            g.transform.DOLocalMoveY(p.zeroY + 1, animTime);

            if (p.type == PieceType.Pawn)
            {
                p.FindPathForward(!selected.GetComponent<Piece>().moved ? 2 : 1);                
            }
            else if (p.type == PieceType.Bishop)
            {
                p.FindPathDiag(7);
            }
            else if (p.type == PieceType.Knight)
            {
                p.FindPathKnight();
            }
            else if (selected.GetComponent<Piece>().type == PieceType.Queen)
            {
                p.FindPathStraight(7);
                p.FindPathDiag(7);
            }
            else if (p.type == PieceType.King)
            {
                p.FindPathStraight(1);
                p.FindPathDiag(1);
            }
            else if (p.type == PieceType.Rook)
            {
                p.FindPathStraight(7);
            }
        }
        else
        {
            selected = null;
            g.transform.DOLocalMoveY(p.zeroY, animTime);
        }
    }
}
