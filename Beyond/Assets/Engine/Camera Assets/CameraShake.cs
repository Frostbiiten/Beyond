///Daniel Moore (Firedan1176) - Firedan1176.webs.com/
///26 Dec 2015
///
///Shakes camera parent object

using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    public bool debugMode = false;//Test-run/Call ShakeCamera() on start

    public float shakeAmount;//The amount to shake this frame.
    public float shakeDuration;//The duration this frame.

    //Readonly values...
    float shakePercentage = 1;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    float startDuration;//The initial shake duration, set when ShakeCamera is called.

    bool isRunning = false; //Is the coroutine running right now?

    public bool smooth;//Smooth rotation?
    public float smoothAmount = 5f;//Amount to smooth


    public IEnumerator Shake(float shakeValue, float time)
    {
        isRunning = true;
        float t = time;

        while (time > 0.005f)
        {
            Vector3 shakeAmount = Random.insideUnitSphere * shakeValue;//A Vector3 to add to the position
            time = Mathf.Lerp(time, 0f, Time.deltaTime);

            if (smooth)
                transform.position = Vector3.Lerp(transform.position, shakeAmount, Time.deltaTime * smoothAmount);
            else
                transform.position = shakeAmount; //Set the local rotation the be the rotation amount.

            yield return null;
        }
        shakeDuration = time;
        transform.position = Vector3.zero;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        isRunning = false;
    }

}