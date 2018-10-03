using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ActionPopup : MonoBehaviour
{
    public static UI_ActionPopup Instance;

    public Text Title;
    public Text Key;

    public KeyCode KeyCode;
    public string ActionName;

    public float FadeTime = 0.2f;
    public AnimationCurve FadeCurve;
    public CanvasGroup Group;

    private float timer;

    private void Awake()
    {
        Instance = this;
        timer = 0f;
    }

    private void Update()
    {
        if(Title.text != ActionName)        
            Title.text = ActionName;
        
        if(Key.text != KeyCode.ToString())        
            Key.text = KeyCode.ToString();

        timer = Mathf.Clamp(timer, 0f, FadeTime);
        float p = timer == 0 ? 0 : timer / FadeTime;
        float x = FadeCurve.Evaluate(p);

        Group.alpha = x;

        timer -= Time.unscaledDeltaTime;
    }

    public static void Display(string action, KeyCode key, Vector2 worldPos)
    {
        if (Instance == null)
            return;

        Instance.timer = Instance.FadeTime;
        Instance.KeyCode = key;
        Instance.ActionName = action;

        Vector2 screenPos = MainCamera.Cam.WorldToScreenPoint(worldPos);
        (Instance.transform as RectTransform).anchoredPosition = screenPos;
    }
}
