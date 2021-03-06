﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    public static void Clear_Log()
    {
        Commands.ClearLog();
    }

    [DebugCommand("Teleports the player to a position.", GodModeOnly = true, Parameters = "FLOAT:x:The x position., FLOAT:y:The y position.")]
    public static void Teleport(float x, float y)
    {
        // TODO redo once the final player-character-server relationship is completed!
        if(Player.Character != null)
        {
            Player.Character.transform.position = new UnityEngine.Vector3(x, y, 0f);
            Commands.Log("Teleported to {0}, {1}".Form(x, y));
        }
    }

    [DebugCommand("Sets the global time scale.", GodModeOnly = true, Parameters = "FLOAT:scale:The time scale coefficient. 1 is normal time; 0 is frozen time.")]
    public static void Time_Scale(float scale)
    {
        if(scale <= 0f)
        {
            scale = 0f;
        }

        Time.timeScale = scale;
    }
}
