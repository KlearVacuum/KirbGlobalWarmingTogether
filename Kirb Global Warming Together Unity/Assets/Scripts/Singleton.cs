using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T _instance = null;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				Debug.LogWarning("No instance of " + typeof(T) + " found!");
				return null;
			}
			else
			{
				return _instance;
			}
		}
	}

	protected virtual void Awake()
	{
		if (_instance != null)
		{
			Debug.LogWarning("More than 1 " + typeof(T) + " detected! Destroying " + gameObject.name + "!");
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this as T;
		}
	}
}