
using System.Collections.Generic;
using UnityEngine;

public class ObjDestroy : LevelObjective
{
    public List<Health> Things = new List<Health>();

    public string Prompt = "[Destroy/kill] the [objects/enemies]. {0}/{1}";
    public bool Format = true;

    public int ThingsAlive
    {
        get
        {
            int count = 0;
            foreach (var c in Things)
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

    public int TotalThings
    {
        get
        {
            return Things.Count;
        }
    }

    public int ThingsDead
    {
        get
        {
            return TotalThings - ThingsAlive;
        }
    }

    public override float GetProgress()
    {
        return Mathf.Clamp01((float)ThingsDead / TotalThings);
    }

    public override bool IsComplete()
    {
        return ThingsAlive == 0;
    }

    public override string GetPrompt()
    {
        if (Format)
            return Prompt.Form(ThingsDead, TotalThings);
        else
            return Prompt;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        foreach (var t in Things)
        {
            if (t == null)
                continue;

            if (t.IsDead)
                continue;

            Gizmos.DrawWireCube(t.transform.position, Vector3.one * 1.2f);
            //Gizmos.DrawWireSphere(t.transform.position, 1.2f);
        }
    }
}
