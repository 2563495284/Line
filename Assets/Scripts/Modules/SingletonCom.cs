using UnityEngine;

public abstract class SingletonCom<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Ins { get; private set; }

    protected virtual void Awake()
    {
        if (Ins != null)
        {
            Destroy(gameObject);
            return;
        }

        Ins = this as T;
    }

    protected virtual void OnApplicationQuit()
    {
        Ins = null;
        Destroy(gameObject);
    }
}

public abstract class PersistentSingleton<T> : SingletonCom<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}