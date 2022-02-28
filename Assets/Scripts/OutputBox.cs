using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class OutputBox : MonoBehaviour {

    AudioSource sound;

    public TextMeshProUGUI output;

    public float charTime = 0.1f;

    bool displaying = false;
    public static bool IsDisplaying
    {
        get => singleton.displaying;
    }

    string targetText = "";

    string actualText = "";
    int targetLength = 0;

    string prefixText = "";
    string suffixText = "";

    float timer = 0;
    int index = -1;

    static OutputBox singleton;

    private void Awake()
    {
        singleton = this;
        print("Output Box init");
        sound = GetComponent<AudioSource>();
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (actualText.Length < targetLength)
        {
            displaying = true;
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer += charTime;
                index++;
                string nextChar = targetText[index].ToString();

                if (nextChar == "<")
                {
                    string cache = "<";
                    while (!cache.EndsWith(">"))
                    {
                        index++;
                        cache += targetText[index];
                    }

                    index++;
                    if (index != targetText.Length)
                    {
                        nextChar = "" + targetText[index];
                    }
                    else
                    {
                        nextChar = "";
                    }

                    targetLength -= cache.Length;

                    if (!cache.Contains("/"))
                    {
                        //print(cache);
                        prefixText = cache;

                        int suffixIndex = targetText.NextIndexOf(index + 1, '<');

                        cache = "<";

                        while (!cache.EndsWith(">"))
                        {
                            suffixIndex++;
                            cache += targetText[suffixIndex];
                        }
                        //print(cache);
                        suffixText = cache;
                    }
                    else
                    {
                        prefixText = suffixText = "";
                    }

                }

                if (/*!sound.isPlaying && */!(nextChar == " "))
                {
                    sound.Play();
                }

                actualText += prefixText + nextChar + suffixText;
                targetLength += prefixText.Length + suffixText.Length;
                output.text = actualText;
            }
        }
        else displaying = false;
    }

    //public enum TextSpeed { Slow, Normal, Fast, Instant}
    /*public void SetSpeed(int speed)
    {
        switch((TextSpeed)speed)
        {
            case TextSpeed.Slow:
                {
                    charTime = 0.1f;
                };break;
            case TextSpeed.Normal:
                {
                    charTime = 0.08f;
                }; break;
            case TextSpeed.Fast:
                {
                    charTime = 0.05f;
                }; break;
            case TextSpeed.Instant:
                {
                    charTime = 0f;
                }; break;
        }
    }*/
    public static void Stop()
    {
        singleton.actualText = singleton.targetText;
    }
    public static void Close()
    {
        Stop();
        singleton.gameObject.SetActive(false);
    }
    public static void Clear()
    {
        singleton.output.text = string.Empty;
        singleton.targetText  = string.Empty;
        singleton.actualText  = string.Empty;
    }

    public void InstanceDisplay(string message)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        prefixText = "";
        suffixText = "";
        targetText = message;
        actualText = output.text = "";
        targetLength = message.Length;
        index = -1;
    }

    public static void Display(string message)
    {
        singleton.InstanceDisplay(message);
    }
    public static void DisplayImmediately(string message)
    {
        singleton.targetText = singleton.actualText = singleton.output.text = message;
        singleton.targetLength = message.Length;
        singleton.gameObject.SetActive(true);
    }
    public static void Append(string message, bool newLine = true)
    {
        if (singleton.gameObject.activeSelf == false)
        {
            singleton.gameObject.SetActive(true);
            Display(message);
        }
        else
        {
            singleton.prefixText = "";
            singleton.suffixText = "";

            singleton.actualText = singleton.output.text = singleton.targetText;
            singleton.index = singleton.targetText.Length - 1;
            singleton.targetText += (newLine ? "\n" : "") + message;


            singleton.targetLength = singleton.targetText.Length;
        }
    }
    public static IEnumerator DisplayTemporarilyEnum(string message, float seconds)
    {
        Display(message);
        yield return new WaitForSeconds(seconds);
        Close();
    }
    /*public static void DisplayTemporarily(string message, float seconds)
    {
        CoroutineRunner.StaticCoroutine(DisplayTemporarilyEnum(message, seconds));
    }*/
}
