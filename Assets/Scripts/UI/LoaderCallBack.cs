using System.Collections;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    [SerializeField] private TypewriterEffect typewriterEffect;

    private bool isLoadingStarted = false;

    private void Update()
    {
        if (!isLoadingStarted)
        {
            isLoadingStarted = true;
            StartCoroutine(WaitForTyping());
        }
    }

    private IEnumerator WaitForTyping()
    {
        // wait until typing finishes
        yield return new WaitUntil(() => typewriterEffect.IsFinishedTyping());

        // small extra pause (optional)
        yield return new WaitForSeconds(0.5f);

        Loader.LoaderCallback();
    }
}