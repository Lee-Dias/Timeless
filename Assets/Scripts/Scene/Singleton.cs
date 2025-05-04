using UnityEngine;

/// <summary>
/// A generic base class for implementing a Singleton pattern in Unity.
/// Ensures that only one instance of a specific MonoBehaviour type <typeparamref name="T"/> exists in the scene at any time.
/// This class provides global access to that single instance and handles its lifecycle, including automatic creation if the instance is missing.
/// It also prevents the singleton instance from being destroyed on scene load, ensuring it persists throughout the game session.
/// </summary>
/// <typeparam name="T">The MonoBehaviour type that this Singleton will manag where only one instance should exist.</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // The single instance of the singleton.
    private static T _instance;

    // Lock object for thread safety when accessing or creating the singleton instance.
    private static object _lock = new object();

    // Flag indicating if the application is quitting, used to avoid creating a new instance on shutdown.
    private static bool _applicationIsQuitting = false;

    /// <summary>
    /// Provides global access to the single instance of <typeparamref name="T"/> in the scene.
    /// If no instance exists, it will create a new GameObject with this component attached.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindFirstObjectByType(typeof(T));

                    if (_instance == null)
                    {
                        // Create a new GameObject to hold the singleton instance
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }
    }

    /// <summary>
    /// Marks that the application is quitting to prevent re-creation of the singleton.
    /// </summary>
    protected virtual void OnDestroy()
    {
        _applicationIsQuitting = true;
    }

    /// <summary>
    /// Ensures only one instance of this Singleton exists in the scene.
    /// If another instance is found, it destroys the new GameObject.
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
