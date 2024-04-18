using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public AnimationCurve curve;
    float returnSpeed = 0.1f;

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration) * magnitude;
            Debug.Log(Random.insideUnitSphere);
            transform.localPosition += Random.insideUnitSphere * (strength / 2);
            yield return null;
        }

        while (Vector2.Distance(transform.position, startPos) > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, returnSpeed);
            yield return new WaitForEndOfFrame();
        }

    }
}
