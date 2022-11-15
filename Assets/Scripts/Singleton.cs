using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = (T) this;
        }
        else
        {
            var thisGameObject = gameObject;
            Debug.Log($"Destroyed existed singleton {thisGameObject.name} in scene {thisGameObject.scene.name}");
            Destroy(thisGameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}