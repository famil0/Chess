using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;

public class Controller : NetworkBehaviour
{
    public Transform selected;
    public Transform clicked;
    public float animTime = 0.5f;
    public GameObject activePlayer;
    public NetworkIdentity identity;

    private void Start()
    {
        identity = GetComponent<NetworkIdentity>();
    }

    [Command]
    public void Move()
    {
        selected.transform.DOLocalMove(clicked.position, animTime);
    }

    [Command]
    public void Select(GameObject g)
    {
        g.GetComponent<Piece>().selected = !g.GetComponent<Piece>().selected;
        if (g.GetComponent<Piece>().selected)
        {
            g.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        else
        {
            g.layer = LayerMask.NameToLayer("Default");
            identity.RemoveClientAuthority();
        }
        if (g.GetComponent<Piece>().selected)
        {
            selected = g.transform;
            g.transform.DOLocalMoveY(1, animTime);

            if (selected.GetComponent<Piece>().type == PieceType.Pawn)
            {
                selected.GetComponent<Piece>().FindPathForward(!selected.GetComponent<Piece>().moved ? 2 : 1);                
            }
        }
        else
        {
            selected = null;
            g.transform.DOLocalMoveY(0, animTime);
        }
    }


    public void AddAuthority(NetworkIdentity id, NetworkIdentity id2)
    {
        id.AssignClientAuthority(id2.connectionToClient);
    }
}
