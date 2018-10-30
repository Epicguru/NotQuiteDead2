using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ObjectiveView : MonoBehaviour
{
    public Text Text;
    public Image Bar;
    public Animator Anim;

    public float LerpSpeed = 3f;

    public void Update()
    {
        LevelObjective current = LevelManager.CurrentObjective;

        Anim.SetBool("Active", current != null);
        
        if(current != null)
        {
            float p = Mathf.Clamp01(current.GetProgress());
            Bar.fillAmount = Mathf.Lerp(Bar.fillAmount, p, Time.unscaledDeltaTime * LerpSpeed);

            Text.text = current.GetPrompt();
        }
    }
}
