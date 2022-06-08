using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using DG.Tweening;

public class PlayerController : NetworkBehaviour
{
    public Camera cam;
    public Control control;
    public NetworkIdentity identity;
    public float animTime = 0.5f;
    public Controller controller;

    void Start()
    {
        identity = GetComponent<NetworkIdentity>();
        controller = GameObject.Find("GameController").GetComponent<Controller>();
        if (!isLocalPlayer)
        {
            GetComponent<PlayerController>().enabled = false;
            cam.gameObject.SetActive(false);
        }
        else
        {
            if (NetworkServer.connections.Count(kv => kv.Value.identity != null) == 1) control = Control.White;
            else control = Control.Black;
            cam.gameObject.SetActive(true);
        }
    }

    Ray ray;
    RaycastHit hit;

    [Client]
    void Update()
    {

        if (!isLocalPlayer) return;
        ray = cam.ScreenPointToRay(Input.mousePosition);
        GameObject g;
        Piece p;
        if (Physics.Raycast(ray, out hit))
        {
            g = hit.collider.gameObject;
            p = g.GetComponent<Piece>();
            if (g.name == "Indicator" && Input.GetMouseButtonDown(0))
            {
                controller.clicked = g.transform;
                controller.Move();
                controller.selected.GetComponent<Piece>().moved = true;
                controller.Select(controller.selected.gameObject);
            }
            if (g.tag != "Piece" || p.color.ToString() != control.ToString()) return;

            g.GetComponent<QuickOutline>().enabled = true;


            if (Input.GetMouseButtonDown(0))
            {
                controller.AddAuthority(controller.identity, identity);
                controller.Select(g);
            }
        }
    }

    


}
