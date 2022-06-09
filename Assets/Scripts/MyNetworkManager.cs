using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public Transform PlayerWhitePosition;
    public Transform PlayerBlackPosition;
    public static int players;
    public Controller controller;

    private void Update()
    {
        players = numPlayers;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {        
        Transform startPos = (numPlayers == 0) ? PlayerWhitePosition.transform : PlayerBlackPosition.transform;
        GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
        controller.players.Add(player);
        controller.activePlayerIdx = 0;
    }  
}
