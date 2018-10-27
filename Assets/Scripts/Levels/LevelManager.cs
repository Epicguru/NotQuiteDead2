
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    public List<LevelObjective> Objectives = new List<LevelObjective>();

    private void Awake()
    {
        instance = this;
        RefreshObjectivesList();
    }

    public void RefreshObjectivesList()
    {
        Objectives.Clear();
        Objectives.AddRange(GetComponentsInChildren<LevelObjective>());
    }

    private void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }
}
