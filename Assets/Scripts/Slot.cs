using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool empty;
    public GameObject indicatorPrefab;
    public Controller controller;
    public Piece p;
    public GameObject indicator;
    public bool isDangered = false;
    public List<GameObject> dangeredBy;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        indicator = Instantiate(indicatorPrefab, transform.position - new Vector3(0, 0.6f, 0), transform.rotation);
        indicator.transform.parent = transform;
        indicator.name = "Indicator";
        indicator.SetActive(false);
        empty = true;
    }

    private void OnTriggerStay(Collider other)
    {
        empty = false;
        p = other.GetComponent<Piece>();
        if (isDangered && !dangeredBy.Find(x => x.name.Contains(p.color.ToString())) && p.type == PieceType.King)
        {
            p.isInDanger = true;
        }
        else if (p.type == PieceType.King)
        {
            p.outline.enabled = false;
            p.outline.OutlineColor = p.outlineColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        empty = true;
        p = null;
    }

    private void LateUpdate()
    {
        if (controller.selected is null) indicator.SetActive(false);
        if (!controller.selected) return;
        if (isDangered && dangeredBy.Find(x => x.name.Contains(controller.players[1 - (int)controller.activePlayer].name.Split(" ")[1]) && controller.selected.GetComponent<Piece>().type == PieceType.King))
        {
            indicator.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            indicator.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }

    }
}
