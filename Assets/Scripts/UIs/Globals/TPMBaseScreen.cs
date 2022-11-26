using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPMBaseScreen : DTNView
{
    public Animator Animator;

    public override void Init()
    {
        
    }

    public override void Show()
    {
        base.Show();
        AddAllOnlickButtons();
        Animator.Play("Show");
    }

    public override void Hide()
    {
        RemoveAllOnlickButtons();
        Animator.Play("Hide");
    }

    public void BaseHide()
    {
        base.Hide();
    }

    public virtual void AddAllOnlickButtons()
    {

    }

    public virtual void RemoveAllOnlickButtons()
    {

    }
}
