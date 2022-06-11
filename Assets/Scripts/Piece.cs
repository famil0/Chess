using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Piece : MonoBehaviour
{
    public QuickOutline outline;
    public Color outlineColor;
    public PieceColor color;
    public bool isSelected = false;
    public PieceType type;
    public List<List<GameObject>> slots = new List<List<GameObject>>();
    public bool moved = false;
    public Slot slot;
    public float zeroY;
    public Controller controller;
    public bool isInDanger = false;
    public bool isMoving = false;
    public List<GameObject> dangeredBy = new List<GameObject>();
    public List<GameObject> path = new List<GameObject>();

    private void Start()
    {
        zeroY = transform.position.y;
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        int i = -1;
        int j = 0;
        GameObject slot = GameObject.Find("Slots");
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

    private void LateUpdate()
    {
        if (!controller.canCheck)
        {
            isInDanger = false;
            dangeredBy.Clear();
        }
        else
        {
            ClearPath();
            controller.Check(GetComponent<Piece>(), true);
        }

        if (type == PieceType.King && isInDanger /*&& slot.dangeredBy.Find(x => x.GetComponent<Piece>().color != color)*/)
        {
            outline.OutlineColor = Color.red;
            outline.enabled = true;
        }
    }

    void ClearPath()
    {
        foreach (GameObject slot in path)
        {
            slot.GetComponent<Slot>().isDangered = false;
            slot.GetComponent<Slot>().dangeredBy.Clear();
        }
        path.Clear();
    }

    private void OnMouseDown()
    {
        if (isInDanger && type != PieceType.King && controller.selected)
        {
            controller.clicked = gameObject.transform;
            controller.Move();
            controller.selected.GetComponent<Piece>().dangeredBy.Remove(controller.clicked.gameObject);
            controller.pieces.Remove(GetComponent<Piece>());
            Destroy(gameObject);
        }
    }

    private void OnMouseExit()
    {
        if (!isInDanger) outline.enabled = false;
    }

    public Tuple<int, int> GetCurrentPos()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (IsOutOfBoard(i, j)) return null;
                if (slots[i][j].GetComponent<Slot>() == slot)
                {
                    return new Tuple<int, int>(i, j);
                }
            }
        }
        return null;
    }

    public void SetDanger(int i, int j, GameObject dangerous)
    {
        Piece piece = slots[i][j].GetComponent<Slot>().p;
        if (piece.color == color) return;
        if (piece.dangeredBy.Contains(gameObject)) return;
        piece.dangeredBy.Add(dangerous);
        piece.isInDanger = true;
    }

    public bool IsFree(int i, int j)
    {
        return slots[i][j].GetComponent<Slot>().empty;       
    }

    public bool IsOutOfBoard(int i, int j)
    {
        return i < 0 || i >= 8 || j < 0 || j >= 8;
    }

    public void FindPathForward(int n, Func<int, int> op, bool onlyDanger)
    {
        Tuple<int, int> current = GetCurrentPos();

        if (onlyDanger && type == PieceType.Pawn)
        {
            int l = op(0);
            int i = current.Item1 + l;
            int j = current.Item2 + l;
            if (IsOutOfBoard(i, j)) goto next;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                goto next;
            }

            next:
            i = current.Item1 - l;
            j = current.Item2 + l;
            if (IsOutOfBoard(i, j)) return;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                return;
            }
            
            return;
        }

        int k = 0;
        while (Math.Abs(k) < n)
        {
            k = op(k);
            int i = current.Item1;
            int j = current.Item2 + k;
            if (IsOutOfBoard(i, j)) break;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                if (type == PieceType.Pawn) return;
                SetDanger(i, j, gameObject);
                return;
            }

            if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);
        } 
    }


    public void FindPathStraight(int n, Func<int, int> op, bool onlyDanger)
    {
        FindPathForward(n, op, onlyDanger);
        Tuple<int, int> current = GetCurrentPos();
        int k = 0;
        while (Math.Abs(k) < n)
        {
            k = op(k);
            int i = current.Item1;
            int j = current.Item2 - k;
            if (IsOutOfBoard(i, j)) break;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                break;
            }
            if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);
        }

        k = 0;
        while (Math.Abs(k) < n)
        {
            k = op(k);
            int i = current.Item1 + k;
            int j = current.Item2;
            if (IsOutOfBoard(i, j)) break;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                break;
            }
            if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);
        }
        k = 0;
        while (Math.Abs(k) < n)
        {
            k = op(k);
            int i = current.Item1 - k;
            int j = current.Item2;
            if (IsOutOfBoard(i, j)) break;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                break;
            }
            if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);
        }
    }



    public void FindPathDiag(int n, Func<int, int> op, bool onlyDanger)
    {
        Tuple<int, int> current = GetCurrentPos();
        int k = 0;
        while (Math.Abs(k) < n)
        {
            k = op(k);
            int i = current.Item1 + k;
            int j = current.Item2 + k;
            if (IsOutOfBoard(i, j)) break;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                break;
            }
            if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);
        }
        k = 0;
        while (Math.Abs(k) < n)
        {
            k = op(k);
            int i = current.Item1 - k;
            int j = current.Item2 - k;
            if (IsOutOfBoard(i, j)) break;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                break;
            }
            if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);
        }

        k = 0;
        while (Math.Abs(k) < n)
        {
            k = op(k);
            int i = current.Item1 - k;
            int j = current.Item2 + k;
            if (IsOutOfBoard(i, j)) break;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                break;
            }
            if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);
        }
        k = 0;
        while (Math.Abs(k) < n)
        {
            k = op(k);
            int i = current.Item1 + k;
            int j = current.Item2 - k;
            if (IsOutOfBoard(i, j)) break;
            path.Add(slots[i][j]);
            if (IsFree(i, j) is false)
            {
                SetDanger(i, j, gameObject);
                break;
            }
            if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);
        }

    }

    public void FindPathKnight(bool onlyDanger)
    {
        Tuple<int, int> current = GetCurrentPos();
        int i = current.Item1 + 1;
        int j = current.Item2 + 2;
        if (IsOutOfBoard(i, j)) goto next;
        path.Add(slots[i][j]);
        if (IsFree(i, j) is false)
        {
            SetDanger(i, j, gameObject);
            goto next;
        }
        if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);

        next:
        i = current.Item1 + 2;
        j = current.Item2 + 1;
        if (IsOutOfBoard(i, j)) goto next1;
        path.Add(slots[i][j]);
        if (IsFree(i, j) is false)
        {
            SetDanger(i, j, gameObject);
            goto next1;
        }
        if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);

        next1:
        i = current.Item1 + 2;
        j = current.Item2 - 1;
        if (IsOutOfBoard(i, j)) goto next2;
        path.Add(slots[i][j]);
        if (IsFree(i, j) is false)
        {
            SetDanger(i, j, gameObject);
            goto next2;
        }
        if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);

        next2:
        i = current.Item1 + 1;
        j = current.Item2 - 2;
        if (IsOutOfBoard(i, j)) goto next3;
        path.Add(slots[i][j]);
        if (IsFree(i, j) is false)
        {
            SetDanger(i, j, gameObject);
            goto next3;
        }
        if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);

        next3:
        i = current.Item1 - 1;
        j = current.Item2 - 2;
        if (IsOutOfBoard(i, j)) goto next4;
        path.Add(slots[i][j]);
        if (IsFree(i, j) is false)
        {
            SetDanger(i, j, gameObject);
            goto next4;
        }
        if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);

        next4:
        i = current.Item1 - 2;
        j = current.Item2 - 1;
        if (IsOutOfBoard(i, j)) goto next5;
        path.Add(slots[i][j]);
        if (IsFree(i, j) is false)
        {
            SetDanger(i, j, gameObject);
            goto next5;
        }
        if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);

        next5:
        i = current.Item1 - 2;
        j = current.Item2 + 1;
        if (IsOutOfBoard(i, j)) goto next6;
        path.Add(slots[i][j]);
        if (IsFree(i, j) is false)
        {
            SetDanger(i, j, gameObject);
            goto next6;
        }
        if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);

        next6:
        i = current.Item1 - 1;
        j = current.Item2 + 2;
        if (IsOutOfBoard(i, j)) return;
        path.Add(slots[i][j]);
        if (IsFree(i, j) is false)
        {
            SetDanger(i, j, gameObject);
            return;
        }
        if (!onlyDanger) slots[i][j].GetComponent<Slot>().indicator.SetActive(true);


    }

    private void OnTriggerStay(Collider other)
    {
        slot = other.GetComponent<Slot>();        
    }
}
