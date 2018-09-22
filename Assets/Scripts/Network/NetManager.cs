using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{
    public Character PlayerCharacter;

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Client has connected to server: IP: {0}, connection ID: {1}. Welcome!".Form(conn.address, conn.connectionId));
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var go = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        // Setup player. For now just give them a faction and a name based on their player number.
        Player player = go.GetComponent<Player>();
        player.Name = "Player #" + Player.All.Count;

        // Here we might load player data from disk and instantiate a character based on the data.
        // For now, just make them a new character.
        var spawned = Instantiate(PlayerCharacter, Vector3.zero, Quaternion.identity);
        player.Manipulator.Target = spawned;
        NetworkServer.Spawn(spawned.gameObject);

        NetworkServer.AddPlayerForConnection(conn, go, playerControllerId);
    }
}
