
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    public List<LevelObjective> Objectives = new List<LevelObjective>();
    public LevelObjective Current;

    public string Overview
    {
        get
        {
            return _overview;
        }
        private set
        {
            _overview = value;
        }
    }
    [SerializeField]
    [TextArea(10, 30)]
    private string _overview;
    private StringBuilder str = new StringBuilder();
    private List<LevelObjective> pending = new List<LevelObjective>();

    private void Awake()
    {
        instance = this;
        RefreshObjectivesList();
    }

    private void Update()
    {
        UpdateOverview();
    }

    private void UpdateOverview()
    {
        const char WHITESPACE = '\t';
        str.Clear();
        pending.Clear();
        int indent = 0;

        pending.AddRange(Objectives);

        LevelObjective current = Objectives[0];
        while(pending.Count > 0)
        {
            str.Append(WHITESPACE, indent);
            str.AppendLine(current.ToString().Trim());
            pending.Remove(current);

            bool comp = current is CompoundLevelObjective;
            if (comp)
            {
                pending.AddRange((current as CompoundLevelObjective).Requirements);
            }

            if(pending.Count > 0)
                current = pending[0];
        }

        Overview = str.ToString().TrimEnd();
    }

    public void RefreshObjectivesList()
    {
        Objectives.Clear();
        foreach (Transform child in transform)
        {
            Objectives.AddRange(child.GetComponents<LevelObjective>());
        }
    }

    private void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }
}
