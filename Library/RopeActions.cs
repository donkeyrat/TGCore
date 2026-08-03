using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library;

public class RopeActions : MonoBehaviour
{
    private Rope Rope;

    public Transform pos1;
    public Transform pos2;
    public float lerpSpeed = 1f;

    private void Start()
    {
        Rope = GetComponent<Rope>();
        Rope.position1 = pos1.position;
        Rope.Position2 = pos2.position;
        Rope.middleVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (!Rope.done)
        {
            Rope.position1 = pos1.position;
            Rope.Position2 = pos2.position;
        }
    }
    
    public void SetDone()
    {
        Rope.done = true;
    }

    public void LerpPos1(Transform destination)
    {
        StartCoroutine(DoLerp1(destination));
    }

    private IEnumerator DoLerp1(Transform destination)
    {
        var startingPos = pos1.position;
        var counter = 0f;
        while (counter < 1f)
        {
            counter += Time.deltaTime * lerpSpeed;
            pos1.position = Vector3.Lerp(startingPos, destination.position, counter);
            yield return null;
        }

        pos1 = destination;
    }
    
    public void LerpPos2(Transform destination)
    {
        StartCoroutine(DoLerp2(destination));
    }

    private IEnumerator DoLerp2(Transform destination)
    {
        var startingPos = pos2.position;
        var counter = 0f;
        while (counter < 1f)
        {
            counter += Time.deltaTime * lerpSpeed;
            pos2.position = Vector3.Lerp(startingPos, destination.position, counter);
            yield return null;
        }

        pos2 = destination;
    }
}
