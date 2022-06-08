using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public Camera cam;

    [Client]
    void Awake()
    {
        if (!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
        }
        else
        {
            cam.gameObject.SetActive(true);            
        }
    }
}
