using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TickManager
{
    public ulong CurrentTick {  get; private set; }

    /// <summary>
    /// Time between ticks based on given ticks per second
    /// </summary>
    public readonly float TickRate;
    /// <summary>
    /// 
    /// </summary>
    private readonly uint FlushInterval;


    private Dictionary<ulong, HashSet<Action>> ScheduledEvents = new Dictionary<ulong, HashSet<Action>>();

    public TickManager(uint tps)
    {
        TickRate = 1.0f / tps;
        FlushInterval = tps;
    }

    public uint TimeToTicks(float time)
    {
        return (uint)Mathf.FloorToInt(Mathf.Abs(time / TickRate));
    }

    public float TicksToTime(ulong ticks)
    {
        return (float)(ticks * TickRate);
    }

    public void ProgressTick()
    {
        CurrentTick++;
        if (ScheduledEvents.ContainsKey(CurrentTick))
        {
            foreach (var item in ScheduledEvents[CurrentTick])
            {
                item.Invoke();
            }
        }
        if (CurrentTick % FlushInterval == 0)
        {
            foreach (uint item in ScheduledEvents.Keys.ToArray().Where(k => k <= CurrentTick))
            {
                ScheduledEvents.Remove(item);
            }
        }
    }

    public void RegisterToNextTick(Action action, out TickCancelToken token)
    {
        RegisterToFutureTick(1, action, out token);
    }

    public void RegisterToFutureTick(ulong ticks, Action action, out TickCancelToken token)
    {
        ulong new_tick = CurrentTick + ticks;
        if (!ScheduledEvents.ContainsKey(new_tick)) ScheduledEvents.Add(new_tick, new HashSet<Action>());
        ScheduledEvents[new_tick].Add(action);
        token = new TickCancelToken(new_tick, ()=> DeregisterAction(new_tick, action));
    }

    private void DeregisterAction(in ulong tick, in Action action)
    {
        if (tick <= CurrentTick) return; // weve already passed this tick, assume it just hasnt been cleaned up yet;
        if (!ScheduledEvents.ContainsKey(tick))
        {
            Debug.LogError($"Attempted to deregister from tick {tick} when it wasnt schedule"); return;
        }
        if (!ScheduledEvents[tick].Remove(action))
        {
            Debug.LogError($"Attempted to deregister from tick {tick} with unregistered action"); return;
        }
    }

    public struct TickCancelToken
    {
        public readonly ulong Tick;
        public readonly Action ToCancel;

        public TickCancelToken(ulong tick, Action action)
        {
            Tick = tick;
            ToCancel = action;
        }
    }
}
