using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public Control control;
    public Controller controller;
    public List<Transform> pieces = new List<Transform>();

    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        
        
        
    }

    Ray ray;
    RaycastHit hit;

    void Update()
    {

        if (controller.activePlayer.ToString() != control.ToString()) return;

        ray = cam.ScreenPointToRay(Input.mousePosition);
        GameObject g;
        Piece p;
        if (Physics.Raycast(ray, out hit))
        {
            g = hit.collider.gameObject;
            p = g.GetComponent<Piece>();
            if (g.name == "Indicator" && Input.GetMouseButtonDown(0))
            {
                if (g.transform.GetComponent<MeshRenderer>().material.color == Color.red && controller.selected.GetComponent<Piece>().type == PieceType.King) return;
                controller.clicked = g.transform.parent;
                controller.Move();
            }
            if (g.tag != "Piece" || p.color.ToString() != control.ToString()) return;

            if (controller.selected || p.isMoving) return;
            g.GetComponent<QuickOutline>().enabled = true;

            if (Input.GetMouseButtonDown(0))
            {
                controller.Select(g);
            }
        }
    }


}
