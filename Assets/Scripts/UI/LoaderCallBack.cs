using System.Collections;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    [SerializeField] private TypewriterEffect typewriterEffect;

    private IEnumerator Start()
    {
        yield return null;

        yield return new WaitUntil(() => typewriterEffect.IsFinishedTyping());

        yield return new WaitForSeconds(0.5f);

        Loader.LoaderCallback();
    }
}