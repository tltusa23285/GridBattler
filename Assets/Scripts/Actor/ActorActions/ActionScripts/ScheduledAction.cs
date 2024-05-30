using System;

public struct ScheduledAction : IEquatable<ScheduledAction>
{
    public readonly Action Action;
    public string Info;
    public ScheduledAction(Action action)
    {
        Action = action;
        Info = null;
    }
    public ScheduledAction(Action action, string info)
    {
        Action = action;
        Info = info;
    }

    bool IEquatable<ScheduledAction>.Equals(ScheduledAction other)
    {
        return this.Action == other.Action;
    }
}
