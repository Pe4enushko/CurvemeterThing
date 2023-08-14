using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class TriggerEvents : MonoBehaviour
{
	[Space(10)]
	public bool checkLayer = false;
	public LayerMask layerMask = 0;
	[Space(10)]
	public bool checkTags = false;
	public string[] tags;
	
	[Space (10)]
	public UnityEvent onEnter;
	public UnityEvent onExit;

	public Action<Collider> onTriggerEnter;
	public Action<Collider, object> onTriggerEnterParam;
	public Action<Collider> onTriggerExit;
	public Action<Collider, object> onTriggerExitParam;

	public object param { get; set; }

	void Awake ()
	{
		this.enabled = false;
	}

	void OnTriggerEnter (Collider other)
	{
		if (Checks (other.gameObject)) {
			this.enabled = true;
				
			onTriggerEnter?.Invoke(other);
			onTriggerEnterParam?.Invoke(other, param);
			onEnter?.Invoke();
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (Checks (other.gameObject)) {
			onTriggerExit?.Invoke(other);
			onTriggerExitParam?.Invoke(other, param);
			onExit?.Invoke();
		}
	}

	bool Checks (GameObject go)
	{
		if (checkLayer && CheckLayerInMask (go.layer, layerMask)) {
			return true;
		}

		if (checkTags) {
			foreach (var t in tags)
			{
				if (go.CompareTag(t))
				{
					return true;
				}
			}
		}

		return false;
	}
	
	bool CheckLayerInMask (int layer, int mask)
	{
		return mask == (mask | (1 << layer));
	}
}
