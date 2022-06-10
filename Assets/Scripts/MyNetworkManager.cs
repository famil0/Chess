using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;

public class MyNetworkManager : NetworkManager
{
    public Transform PlayerWhitePosition;
    public Transform PlayerBlackPosition;
    public Controller controller;


    //public Transform selected;
    //public Transform clicked;
    //public float animTime = 0.5f;
    //public List<GameObject> players = new List<GameObject>();
    //public int activePlayerIdx;
    //public NetworkIdentity identity;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = (numPlayers == 0) ? PlayerWhitePosition.transform : PlayerBlackPosition.transform;
        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
        controller.players.Add(player);
        controller.activePlayerIdx = 0;
        Debug.Log(controller.players.Count);
    }
}
