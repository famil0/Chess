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
    public List<Transform> pieces = new List<Transform>();

    void Start()
    {
        
        controller = GameObject.Find("GameController").GetComponent<Controller>();
        if (!isLocalPlayer)
        {
            GetComponent<PlayerController>().enabled = false;
            cam.gameObject.SetActive(false);
        }
        else
        {
            if (NetworkServer.connections.Count(kv => kv.Value.identity != null) == 1)
            {
                control = Control.White;
                foreach (Transform child in GameObject.Find("Whites").transform)
                {
                    pieces.Add(child);
                }
            }
            else
            {
                control = Control.Black;
                Debug.Log(GameObject.Find("Blacks").transform.childCount);
                foreach (Transform child in GameObject.Find("Blacks").transform)
                {
                    pieces.Add(child);
                }
            }
            cam.gameObject.SetActive(true);
        }
    }

    Ray ray;
    RaycastHit hit;

    [Client]
    void Update()
    {

        if (controller.players[controller.activePlayerIdx] != gameObject) return;

        identity = GetComponent<NetworkIdentity>();
        if (!isLocalPlayer) return;
        ray = cam.ScreenPointToRay(Input.mousePosition);
        GameObject g;
        Piece p;
        if (Physics.Raycast(ray, out hit))
        {
            AddAuthority(controller.identity, identity);
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

            //Debug.Log(identity + " " + p.identity + " " + controller.identity);
            //AddAuthority(identity, controller.identity);
            //Debug.Log(identity + " " + p.identity + " " + controller.identity);
            //Debug.Log("-----------------------");
            //AddAuthority(identity, controller.identity);
            //controller.identity.AssignClientAuthority(connectionToClient);
            if (Input.GetMouseButtonDown(0))
            {
                controller.Select(g);
            }
        }
    }

    [Command]
    public void AddAuthority(NetworkIdentity id, NetworkIdentity id2)
    {
        id.AssignClientAuthority(id2.connectionToClient);
    }


}
