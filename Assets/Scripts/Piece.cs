using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;

public class Piece : NetworkBehaviour
{
    public QuickOutline outline;
    public Color outlineColor;
    public PieceColor color;
    public float animTime = 0.5f;
    public bool selected = false;
    public PieceType type;
    public List<GameObject> slots = new List<GameObject>();
    public bool moved = false;
    public Transform current;
    public NetworkIdentity identity;

    private void Start()
    {
        identity = GetComponent<NetworkIdentity>();
        GameObject slot = GameObject.Find("Slots");
        foreach (Transform child in slot.transform)
        {
            slots.Add(child.gameObject);
        }
    }



    private void OnMouseExit()
    {
        if (!selected) outline.enabled = false;
    }

    public void FindPathForward(int n)
    {
        List<GameObject> path = new List<GameObject>();      
        for (int i = 1; i <= n; i++)
        {
            int idx = slots.FindIndex(c => c.transform == current.transform);
            idx += color == PieceColor.White ? i : -i;
            if (slots[idx].GetComponent<Slot>().empty is false) continue;
            slots[idx].transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        current = other.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        current = null;
    }

}
