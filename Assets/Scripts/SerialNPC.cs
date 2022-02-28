using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SerialNPC : MonoBehaviour, IInteractable
{
    public TextWriter w;

    public Message[] serialMessages;

    int i = -1;

    void IInteractable.Interact(CubePlayer p)
    {
        if (i < serialMessages.Length - 1) i++;
        StartCoroutine(Process(p, serialMessages[i].messages));
    }

    IEnumerator Process(CubePlayer p, string[] messages)
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

    [System.Serializable]
    public class Message
    {
        [Multiline]
        public string[] messages;
    }
}
