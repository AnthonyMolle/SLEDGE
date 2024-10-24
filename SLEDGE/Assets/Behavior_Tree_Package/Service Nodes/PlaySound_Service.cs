using UnityEngine;
using static Unity.VisualScripting.Member;

public class PlaySound_Service : ServiceNode
{
    public AudioClip soundToPlay;
    AudioSource sourceToPlayAt;

    protected override void OnStart()
    {
        sourceToPlayAt = blackboard.getCurrentRunner().GetComponent<AudioSource>();
        if (sourceToPlayAt == null)
        {
            Debug.LogError("Trying to run a PlaySound Service but missing AudioSource on gameObject running the Tree.");
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        sourceToPlayAt.PlayOneShot(soundToPlay);
        return State.Success;
    }
}