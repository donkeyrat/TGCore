using UnityEngine;

namespace TGCore.Library;

public class PlayAnimation : MonoBehaviour
{
    private Animation animation;
    
    private void Start()
    {
        animation = GetComponent<Animation>();
    }

    public void Play()
    {
        animation.Play();
    }
    
    public void Stop()
    {
        animation.Stop();
    }
}