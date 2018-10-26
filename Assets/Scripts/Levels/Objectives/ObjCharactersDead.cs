
using System.Collections.Generic;
using UnityEngine;

public class ObjCharactersDead : LevelObjective
{
    public List<Character> Characters = new List<Character>();

    public int CharactersAlive
    {
        get
        {
            int count = 0;
            foreach (var c in Characters)
            {
                if(c == null)                
                    continue;
                
                if (!c.IsDead)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public int TotalCharacters
    {
        get
        {
            return Characters.Count;
        }
    }

    public int CharactersDead
    {
        get
        {
            return TotalCharacters - CharactersAlive;
        }
    }

    public override float GetProgress()
    {
        return Mathf.Clamp01((float)CharactersDead / TotalCharacters);
    }

    public override bool IsComplete()
    {
        return CharactersAlive == 0;
    }

    public override string GetPrompt()
    {
        return "Kill all enemies. {0}/{1}".Form(CharactersDead, TotalCharacters);
    }
}
