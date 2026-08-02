using System.Collections;

using UnityEngine;

[DefaultExecutionOrder(100)]
public class ScreenShake : MonoBehaviour
{
    private Coroutine activeShake;
    private Vector3 frameOffset;
    private Vector3 appliedOffset;

    public void Play(float duration, float magnitude)
    {
        if (!isActiveAndEnabled)
            return;

        if (activeShake != null)
            StopCoroutine(activeShake);

        frameOffset = Vector3.zero;
        activeShake = StartCoroutine(PlayShake(duration, magnitude));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        duration = Mathf.Max(0f, duration);
        magnitude = Mathf.Max(0f, magnitude);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            frameOffset = new Vector3(x, y, 0f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        frameOffset = Vector3.zero;
    }

    private IEnumerator PlayShake(float duration, float magnitude)
    {
        yield return Shake(duration, magnitude);
        activeShake = null;
    }

    private void Update()
    {
        RemoveAppliedOffset();
    }

    private void LateUpdate()
    {
        transform.localPosition += frameOffset;
        appliedOffset = frameOffset;
    }

    private void OnDisable()
    {
        frameOffset = Vector3.zero;
        RemoveAppliedOffset();
        activeShake = null;
    }

    private void RemoveAppliedOffset()
    {
        if (appliedOffset == Vector3.zero)
            return;

        transform.localPosition -= appliedOffset;
        appliedOffset = Vector3.zero;
    }
}
