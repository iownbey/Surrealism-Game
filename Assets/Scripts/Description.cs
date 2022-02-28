using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Description : MonoBehaviour, IInteractable
{
    public TextWriter w;

    [Multiline]
    public string[] messages;

    void IInteractable.Interact(CubePlayer p) => StartCoroutine(Process(p));

    IEnumerator Process(CubePlayer p)
    {
        p.Freeze();

        foreach (string s in messages)
        {
            w.Display(s);
            yield return p.WaitForInput();
        }
        w.Close();

        p.Unfreeze();
    }
}
