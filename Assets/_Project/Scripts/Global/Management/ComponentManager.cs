using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public interface IRegisterable { }
public static class ComponentManager<T> where T : MonoBehaviour, IRegisterable
{
    public static Dictionary<Transform, T> Dict { get; } = new();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        //cleanup every time we switch to a new scene, there might be stuff we want to keep?
        SceneManager.activeSceneChanged += Clear;
        //clear everything when exiting the game, probably not needed but I love paranoia
        Application.quitting += Clear;
    }
    public static event System.Action<T> OnRegister;
    public static event System.Action<T> OnDeRegister;
    public static bool Register(T obj)
    {
        if (obj == null)
        {
            Debug.LogError($"Attempted to register null {typeof(T).FullName} on {typeof(T).FullName} manager!!");
            return false;
        }
        if (Dict.TryAdd(obj.transform, obj))
        {
            OnRegister?.Invoke(obj);
            return true;
        }
        return false;
    }
    public static T Get(Transform transform)
    {
        if (transform == null)
        {
            Debug.LogError($"Cannot get component for null transform on {typeof(T).FullName} manager!");
            return null;
        }
        return transform != null && Dict.TryGetValue(transform, out var t) ? t : default;
    }
    public static bool DeRegister(Transform transform)
    {
        if (transform == null)
        {
            Debug.LogError($"Attempted to deregister null transform on {typeof(T).FullName} manager!!");
            return false;
        }
        if (Dict.TryGetValue(transform, out var t))
        {
            OnDeRegister?.Invoke(t);
            Dict.Remove(transform);
            return true;
        }
        return false;
    }
    public static void Cleanup(Scene _, Scene __)
    {
        Cleanup();
    }
    public static void Cleanup()
    {
        Stack<Transform> toRemove = new();
        foreach (var kvp in Dict)
        {
            if (kvp.Key == null || kvp.Value == null) toRemove.Push(kvp.Key);
        }
    }
    public static void ClearWithEvents()
    {
        foreach (var kvp in Dict)
        {
            if (kvp.Value) OnDeRegister?.Invoke(kvp.Value);
        }
        Clear();
    }
    static void Clear(Scene _, Scene __) => ClearWithEvents();
    public static void Clear()
    {
        Dict.Clear();
        OnRegister = null;
        OnDeRegister = null;
    }
}