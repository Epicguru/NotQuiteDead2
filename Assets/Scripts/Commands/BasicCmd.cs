using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class BasicCmd
{
    [DebugCommand("Displays info about all commands.", GodModeOnly = false)]
    public static void Help()
    {
        StringBuilder str = new StringBuilder();

        foreach (var pair in Commands.Loaded)
        {
            foreach (var item in pair.Value)
            {
                str.AppendLine(item.GetHelp(true));
                str.AppendLine();
            }
        }

        Commands.Log(str.ToString());
    }

    [DebugCommand("Clears all text off the console log.", GodModeOnly = false)]
    public static void ClearLog()
    {
        Commands.ClearLog();
    }

    [DebugCommand("Teleports the local player to a position.", GodModeOnly = true, ServerOnly = true, Parameters = "FLOAT:x:The x position., FLOAT:y:The y position.")]
    public static void Teleport(float x, float y)
    {
        // TODO redo once the final player-character-server relationship is completed!
        if(Player.Local != null && Player.Local.Manipulator.Target != null)
        {
            Debug.Log("Teleporting to {0}, {1}".Form(x, y));
            Player.Local.Manipulator.Target.transform.position = new UnityEngine.Vector3(x, y, 0f);
            Commands.Log("Teleported to {0}, {1}".Form(x, y));
        }
    }

    [DebugCommand("Teleports the local player to another player.", GodModeOnly = true, ServerOnly = true, Parameters = "STRING:name:The name of the other player.")]
    public static void Teleport(string target)
    {
        // TODO implement me!
        Commands.Log(RichText.InColour("Not implemented...", Color.yellow));
    }

    [DebugCommand("Get info about the current multiplayer network.")]
    public static void Net_State()
    {
        if(Player.Local == null)
        {
            Commands.Log(RichText.InColour("Not connected to game server.", Color.yellow));
        }
        else
        {
            int playerCount = Player.All.Count;
            bool isServer = Player.Local.isServer;
            string remoteId = Player.Local.GetComponent<NetworkIdentity>().connectionToServer.address;
            Commands.Log(("Is Server: {0}\n" +
                         "Player Count: {1}\n" +
                         "Server Address: {2}").Form(isServer, playerCount, remoteId));
        }
    }
}
