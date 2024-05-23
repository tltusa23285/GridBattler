using System.Collections.Generic;
using UnityEngine;

public class ActorGraphicController : MonoBehaviour
{
    [SerializeField] private Animation Anim;

    private Dictionary<string, AnimationState> Anims = new Dictionary<string, AnimationState>();

    private void Awake()
    {
        foreach (AnimationState item in Anim)
        {
            Anims.Add(item.name, item);
        }
    }

    public void TryPlayAnim(in string toPlay, in float duration)
    {
        if (!Anims.ContainsKey(toPlay))
        {
            Debug.LogWarning($"Missing animation {toPlay}");
            return;
        }
        Anims[toPlay].speed = Anims[toPlay].length/duration;
        Anim.Play(toPlay);
    }
}
