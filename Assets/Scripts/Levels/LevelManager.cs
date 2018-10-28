
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance;

    public static LevelObjective CurrentObjective
    {
        get
        {
            if (instance == null)
                return null;

            return instance.Current;
        }
    }

    [HideInInspector]
    public List<LevelObjective> Objectives = new List<LevelObjective>();
    public LevelObjective Current
    {
        get
        {
            return _current;
        }
        private set
        {
            _current = value;
        }
    }
    [SerializeField]
    [ReadOnly]
    private LevelObjective _current;
    [ReadOnly]
    public bool LevelComplete = false;

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
        UpdateCurrentObj();
    }

    private void UpdateCurrentObj()
    {
        if (LevelComplete)
            return;        

        if (Current != null && Current.IsComplete())
        {
            Current = null;
        }

        if (Current == null)
        {
            if (Objectives.Count == 0)
            {
                LevelComplete = true;
                return;
            }
            else
            {
                Current = Objectives[0];
                Objectives.RemoveAt(0);
            }
        }
    }

    private void UpdateOverview()
    {
        if(Objectives.Count == 0 && Current == null)
        {
            Overview = "No remaining objectives.";
            return;
        }

        const char WHITESPACE = '\t';
        str.Clear();
        pending.Clear();
        int indent = 0;

        if (Current != null)
            pending.Add(Current);
        pending.AddRange(Objectives);

        LevelObjective current = pending[0];
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
