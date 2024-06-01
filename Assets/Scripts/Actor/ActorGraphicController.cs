using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GBGame.Actors
{
    public class ActorGraphicController : MonoBehaviour
    {
        [SerializeField] private Animation Anim;
        [SerializeField] private Actor Actor;

        private Dictionary<string, AnimationState> Anims = new Dictionary<string, AnimationState>();

        private void Awake()
        {
            foreach (AnimationState item in Anim)
            {
                Anims.Add(item.name, item);
                item.speed = 0;
            }
        }

        public void TryPlayAnim(in string toPlay, in uint duration)
        {
            if (!Anims.ContainsKey(toPlay))
            {
                Debug.LogWarning($"Missing animation {toPlay}");
                return;
            }
            //Anims[toPlay].speed = Anims[toPlay].length/duration;
            //Anim.Play(toPlay);
            StartCoroutine(TickLerpAnim(toPlay, duration));
        }

        private IEnumerator TickLerpAnim(string toPlay, uint duration)
        {
            ulong start_time = Actor.Com.TickManager.CurrentTick;
            ulong end_time = start_time + duration;

            Anim.Play(toPlay);

            AnimationState state = Anims[toPlay];

            while (true)
            {
                if (Actor.Com.TickManager.CurrentTick == end_time) break;
                state.normalizedTime = Mathf.MoveTowards(state.normalizedTime,
                                            Mathf.InverseLerp(start_time, end_time, Actor.Com.TickManager.CurrentTick),
                                            Time.deltaTime / Actor.Com.TickManager.TickRate);
                yield return null;
            }
            Anim[toPlay].normalizedTime = 1;
        }
    } 
}