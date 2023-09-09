using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreamMessager : MonoBehaviour
{
    public Text messageText;
    public float displayDuration = 0.7f;
    public float fadeDuration = 0.3f;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isDisplayingMessage = false;

    private void Start()
    {
        messageText.enabled = false;
    }

    private void Update()
    {
        if (!isDisplayingMessage && messageQueue.Count > 0)
        {
            string nextMessage = messageQueue.Dequeue();
            StartCoroutine(DisplayMessage(nextMessage));
        }
    }

    public void ShowMessage(string message)
    {
        messageQueue.Enqueue(message);
    }

    private IEnumerator DisplayMessage(string message)
    {
        isDisplayingMessage = true;
        messageText.text = message;
        messageText.enabled = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        messageText.enabled = false;
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);

        isDisplayingMessage = false;
    }
}