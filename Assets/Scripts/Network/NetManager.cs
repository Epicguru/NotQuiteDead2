using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : NetworkManager
{
    public List<GameObject> StaticSpawnables = new List<GameObject>();
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
        NetworkServer.AddPlayerForConnection(conn, go, playerControllerId);

        // Here we might load player data from disk and instantiate a character based on the data.
        // For now, just make them a new character. Spawn with client authority!
        var character = Instantiate(PlayerCharacter, Vector3.zero, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(character.gameObject, player.gameObject);

        var item = Item.Spawn((ushort)Random.Range(0, 3), Vector2.zero);
        var item2 = Item.Spawn((ushort)Random.Range(0, 3), Vector2.zero);
        character.Hands.StoreItem(item); // Put on back.
        character.Hands.StoreItem(item2); // Put on back.
        character.Hands.EquipItem(item); // Put in hands (requires it to be on the back).

        player.Manipulator.Target = character;
    }

    private static List<GameObject> registerPending = new List<GameObject>();
    public static void Register(GameObject obj)
    {
        if (obj == null)
            return;

        if(!registerPending.Contains(obj))
            registerPending.Add(obj);
    }

    public static void ApplyRegister()
    {
        singleton.spawnPrefabs.Clear();
        singleton.spawnPrefabs.AddRange((singleton as NetManager).StaticSpawnables);
        singleton.spawnPrefabs.AddRange(registerPending);        
    }
}
