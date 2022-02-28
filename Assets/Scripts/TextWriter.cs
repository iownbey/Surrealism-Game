using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class TextWriter : MonoBehaviour {

    AudioSource sound;

    public TMP_Text output;

    public float charTime = 0.1f;

    bool displaying = false;

    string targetText = "";

    string actualText = "";
    int targetLength = 0;

    string prefixText = "";
    string suffixText = "";

    float timer = 0;
    int index = -1;

    private void Awake()
    {
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
    public void Stop()
    {
        actualText = targetText;
    }
    public void Close()
    {
        Stop();
        gameObject.SetActive(false);
    }
    public void Clear()
    {
        output.text = string.Empty;
        targetText  = string.Empty;
        actualText  = string.Empty;
    }

    public void Display(string message)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        prefixText = "";
        suffixText = "";
        targetText = message;
        actualText = output.text = "";
        targetLength = message.Length;
        index = -1;
    }
    public void DisplayImmediately(string message)
    {
        targetText = actualText = output.text = message;
        targetLength = message.Length;
        gameObject.SetActive(true);
    }
    public void Append(string message, bool newLine = true)
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
            Display(message);
        }
        else
        {
            prefixText = "";
            suffixText = "";

            actualText = output.text = targetText;
            index = targetText.Length - 1;
            targetText += (newLine ? "\n" : "") + message;


            targetLength = targetText.Length;
        }
    }
    public IEnumerator DisplayTemporarilyEnum(string message, float seconds)
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
