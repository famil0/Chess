using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;
using System;

public class Piece : NetworkBehaviour
{
    public QuickOutline outline;
    public Color outlineColor;
    public PieceColor color;
    public float animTime = 0.5f;
    public bool selected = false;
    public PieceType type;
    public List<List<GameObject>> slots = new List<List<GameObject>>();
    public bool moved = false;
    public Transform currentPos;
    public NetworkIdentity identity;
    public float zeroY;

    private void Start()
    {
        zeroY = transform.position.y;
        identity = GetComponent<NetworkIdentity>();
        GameObject slot = GameObject.Find("Slots");
        int i = -1;
        int j = 0;
        foreach (Transform s in slot.transform)
        {
            if (j++ % 8 == 0)
            {
                slots.Add(new List<GameObject>());
                i++;
            }
            slots[i].Add(s.gameObject);
        }        
    }



    private void OnMouseExit()
    {
        if (!selected) outline.enabled = false;
    }

    public Tuple<int, int> GetCurrentPos()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (slots[i][j].transform == currentPos)
                {
                    return new Tuple<int, int>(i, j);
                }
            }
        }
        return null;
    }

    public void FindPathForward(int n)
    {
        Tuple<int, int> current = GetCurrentPos();
        for (int k = 1; k <= n; k++)
        {
            int i = current.Item1;
            int j = current.Item2 + k;
            if (i < 0 || i >= 8 || j < 0 || j >= 8) return;
            if (slots[i][j].GetComponent<Slot>().empty is false) return;
            slots[i][j].transform.GetChild(0).gameObject.SetActive(true);
        }        
    }

    public void FindPathStraight(int n)
    {
        FindPathForward(n);
        Tuple<int, int> current = GetCurrentPos();
        for (int k = 1; k <= n; k++)
        {
            int i = current.Item1;
            int j = current.Item2 - k;
            if (i < 0 || i >= 8 || j < 0 || j >= 8) break;
            if (slots[i][j].GetComponent<Slot>().empty is false) break;
            slots[i][j].transform.GetChild(0).gameObject.SetActive(true);
        }

        for (int k = 1; k <= n; k++)
        {
            int i = current.Item1 + k;
            int j = current.Item2;
            if (i < 0 || i >= 8 || j < 0 || j >= 8) break;
            if (slots[i][j].GetComponent<Slot>().empty is false) break;
            slots[i][j].transform.GetChild(0).gameObject.SetActive(true);
        }
        for (int k = 1; k <= n; k++)
        {
            int i = current.Item1 - k;
            int j = current.Item2;
            if (i < 0 || i >= 8 || j < 0 || j >= 8) break;
            if (slots[i][j].GetComponent<Slot>().empty is false) break;
            slots[i][j].transform.GetChild(0).gameObject.SetActive(true);
        }
    }



    public void FindPathDiag(int n)
    {
        Tuple<int, int> current = GetCurrentPos();
        for (int k = 1; k <= n; k++)
        {
            int i = current.Item1 + k;
            int j = current.Item2 + k;
            if (i < 0 || i >= 8 || j < 0 || j >= 8) break;
            if (slots[i][j].GetComponent<Slot>().empty is false) break;
            slots[i][j].transform.GetChild(0).gameObject.SetActive(true);
        }
        for (int k = 1; k <= n; k++)
        {
            int i = current.Item1 - k;
            int j = current.Item2 - k;
            if (i < 0 || i >= 8 || j < 0 || j >= 8) break;
            if (slots[i][j].GetComponent<Slot>().empty is false) break;
            slots[i][j].transform.GetChild(0).gameObject.SetActive(true);
        }


        for (int k = 1; k <= n; k++)
        {
            int i = current.Item1 - k;
            int j = current.Item2 + k;
            if (i < 0 || i >= 8 || j < 0 || j >= 8) break;
            if (slots[i][j].GetComponent<Slot>().empty is false) break;
            slots[i][j].transform.GetChild(0).gameObject.SetActive(true);
        }
        for (int k = 1; k <= n; k++)
        {
            int i = current.Item1 + k;
            int j = current.Item2 - k;
            if (i < 0 || i >= 8 || j < 0 || j >= 8) break;
            if (slots[i][j].GetComponent<Slot>().empty is false) break;
            slots[i][j].transform.GetChild(0).gameObject.SetActive(true);
        }

    }

    public void FindPathKnight()
    {
        Tuple<int, int> current = GetCurrentPos();
        int i = current.Item1 + 1;
        int j = current.Item2 + 2;
        if (i < 0 || i >= 8 || j < 0 || j >= 8) goto next;
        if (slots[i][j].GetComponent<Slot>().empty is false) goto next;
        slots[i][j].transform.GetChild(0).gameObject.SetActive(true);

        next:
        i = current.Item1 + 2;
        j = current.Item2 + 1;
        if (i < 0 || i >= 8 || j < 0 || j >= 8) goto next1;
        if (slots[i][j].GetComponent<Slot>().empty is false) goto next1;
        slots[i][j].transform.GetChild(0).gameObject.SetActive(true);

        next1:
        i = current.Item1 + 2;
        j = current.Item2 - 1;
        if (i < 0 || i >= 8 || j < 0 || j >= 8) goto next2;
        if (slots[i][j].GetComponent<Slot>().empty is false) goto next2;
        slots[i][j].transform.GetChild(0).gameObject.SetActive(true);

        next2:
        i = current.Item1 + 1;
        j = current.Item2 - 2;
        if (i < 0 || i >= 8 || j < 0 || j >= 8) goto next3;
        if (slots[i][j].GetComponent<Slot>().empty is false) goto next3;
        slots[i][j].transform.GetChild(0).gameObject.SetActive(true);

        next3:
        i = current.Item1 - 1;
        j = current.Item2 - 2;
        if (i < 0 || i >= 8 || j < 0 || j >= 8) goto next4;
        if (slots[i][j].GetComponent<Slot>().empty is false) goto next4;
        slots[i][j].transform.GetChild(0).gameObject.SetActive(true);

    next4:
        i = current.Item1 - 2;
        j = current.Item2 - 1;
        if (i < 0 || i >= 8 || j < 0 || j >= 8) goto next5;
        if (slots[i][j].GetComponent<Slot>().empty is false) goto next5;
        slots[i][j].transform.GetChild(0).gameObject.SetActive(true);

    next5:
        i = current.Item1 - 2;
        j = current.Item2 + 1;
        if (i < 0 || i >= 8 || j < 0 || j >= 8) goto next6;
        if (slots[i][j].GetComponent<Slot>().empty is false) goto next6;
        slots[i][j].transform.GetChild(0).gameObject.SetActive(true);

    next6:
        i = current.Item1 - 1;
        j = current.Item2 + 2;
        if (i < 0 || i >= 8 || j < 0 || j >= 8) return;
        if (slots[i][j].GetComponent<Slot>().empty is false) return;
        slots[i][j].transform.GetChild(0).gameObject.SetActive(true);


    }

    private void OnTriggerStay(Collider other)
    {
        currentPos = other.transform;
    }
}
