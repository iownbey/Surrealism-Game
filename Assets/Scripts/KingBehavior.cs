using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBehavior : MonoBehaviour, IInteractable
{
    public TextWriter w;

    public GameObject door;

    [Multiline]
    public string[] intro;
    bool spokenTo = false;

    [Multiline]
    public string[] talkagain;

    void IInteractable.Interact(CubePlayer p)
    {
        StartCoroutine(Process(spokenTo? talkagain : intro,p));
        spokenTo = true;
    }

    IEnumerator Process(string[] messages, CubePlayer p)
    {
        p.Freeze();

        foreach (string s in messages)
        {
            w.Display(s);
            yield return p.WaitForInput();
        }
        w.Close();

        p.Unfreeze();
        door.SetActive(false);
    }
}
