using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    private Dictionary<System.Type, UnityEventBase> eventDictionary;
    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType<EventManager>();
                if (!eventManager)
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                else
                    eventManager.Init();
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
            eventDictionary = new Dictionary<System.Type, UnityEventBase>();
    }

    static T GetOrCreate<T>() where T : UnityEventBase, new()
    {
        if (instance.eventDictionary.TryGetValue(typeof(T), out var existing))
            return (T)existing;

        var newEvent = new T();
        instance.eventDictionary.Add(typeof(T), newEvent);
        return newEvent;
    }

    public static void StartListening<T>(UnityAction listener) where T : UnityEvent, new()
        => GetOrCreate<T>().AddListener(listener);

    public static void StartListening<T, T0>(UnityAction<T0> listener) where T : UnityEvent<T0>, new()
        => GetOrCreate<T>().AddListener(listener);

    public static void StopListening<T>(UnityAction listener) where T : UnityEvent, new()
        => GetOrCreate<T>().RemoveListener(listener);

    public static void StopListening<T, T0>(UnityAction<T0> listener) where T : UnityEvent<T0>, new()
        => GetOrCreate<T>().RemoveListener(listener);

    public static void TriggerEvent<T>() where T : UnityEvent, new()
        => GetOrCreate<T>().Invoke();

    public static void TriggerEvent<T, T0>(T0 arg) where T : UnityEvent<T0>, new()
        => GetOrCreate<T>().Invoke(arg);
}