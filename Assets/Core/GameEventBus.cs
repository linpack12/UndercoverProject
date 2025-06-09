using System;
public static class GameEventBus<T> where T : GameEvent
{
    public static event Action<T> OnGameEvent;

    public static void Raise(T e)
    {
        OnGameEvent?.Invoke(e);
    }
}
