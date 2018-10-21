
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableObject))]
public class TempEffect : MonoBehaviour
{
    public static Dictionary<EffectPrefab, TempEffect> loaded = new Dictionary<EffectPrefab, TempEffect>();
    public EffectPrefab ID;

    public float Duration;
    public AnimationCurve AlphaCurve = AnimationCurve.Constant(0f, 1f, 1f);
    public Sprite[] Sprites;

    public PoolableObject PoolableObject
    {
        get
        {
            if (_po == null)
                _po = GetComponent<PoolableObject>();
            return _po;
        }
    }
    private PoolableObject _po;

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_spr == null)
                _spr = GetComponentInChildren<SpriteRenderer>();
            return _spr;
        }
    }
    private SpriteRenderer _spr;

    private float timer;

    public void UponSpawned()
    {
        timer = Duration;

        if(Sprites != null && Sprites.Length > 0 && SpriteRenderer != null)
        {
            var index = Random.Range(0, Sprites.Length);
            var spr = Sprites[index];

            SpriteRenderer.sprite = spr;
        }

        SetAlpha(AlphaCurve.Evaluate(0f));
    }

    private void SetAlpha(float a)
    {
        if(SpriteRenderer != null)
        {
            var c = SpriteRenderer.color;
            c.a = a;
            SpriteRenderer.color = c;
            var c2 = SpriteRenderer.material.GetColor("_Color");
            c2.a = a;
            SpriteRenderer.material.SetColor("_Color", c2);
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        SetAlpha(AlphaCurve.Evaluate(1f - timer / Duration));

        if(timer <= 0f)
        {
            timer = 0f;
            Pool.Return(this.PoolableObject);
        }
    }

    public static void LoadAll()
    {
        loaded.Clear();
        var fromDisk = Resources.LoadAll<TempEffect>("Effects");

        foreach (var e in fromDisk)
        {
            if (loaded.ContainsKey(e.ID))
            {
                Debug.LogError("Duplicate effect Id '{0}'".Form(e.ID));
            }
            else
            {
                loaded.Add(e.ID, e);
            }
        }

        Debug.Log("Loaded {0} temp effects from disk.".Form(loaded.Count));
    }

    public static void UnloadAll()
    {
        loaded.Clear();
    }

    public static bool IsLoaded(EffectPrefab id)
    {
        return loaded.ContainsKey(id);
    }

    public static TempEffect GetPrefab(EffectPrefab id)
    {
        if (!IsLoaded(id))
            return null;

        return loaded[id];
    }

    public static TempEffect Spawn(EffectPrefab id, Vector2 position, float angle)
    {
        var prefab = GetPrefab(id);
        if(prefab == null)
        {
            Debug.LogWarning("Failed to find effect prefab for Id {0}, no effect spawned.".Form(id));
            return null;
        }

        return Pool.Get(prefab.PoolableObject, position, Quaternion.Euler(0f, 0f, angle)).GetComponent<TempEffect>();
    }
}

public enum EffectPrefab : ushort
{
    MUZZLE_FLASH
}
