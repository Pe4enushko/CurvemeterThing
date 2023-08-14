using System.Collections.Generic;
using UnityEngine.Events;

public class UiButtons
{
	//-----------------------------------------------------------------------------------------------------------------//
	public enum EventType
	{
		DOWN,
		CLICK
	}

	//-----------------------------------------------------------------------------------------------------------------//
	private static Dictionary<string, Dictionary<EventType, UnityEvent<object>>> _dict =
		new Dictionary<string, Dictionary<EventType, UnityEvent<object>>>();

	//-----------------------------------------------------------------------------------------------------------------//
	public static void onClick(string buttonId, UnityAction<object> onEvent)
	{
		Listen(buttonId, EventType.CLICK, onEvent);
	}

	//-----------------------------------------------------------------------------------------------------------------//
	public static void Listen(string buttonId, EventType eventType, UnityAction<object> onEvent)
	{
		if (!_dict.ContainsKey(buttonId))
		{
			UnityEvent<object> evn = new UnityEvent<object>();
			_dict.Add(buttonId, new Dictionary<EventType, UnityEvent<object>>() {{eventType, evn}});
		}
		else
		{
			if (!_dict[buttonId].ContainsKey(eventType))
			{
				UnityEvent<object> evn = new UnityEvent<object>();
				_dict[buttonId].Add(eventType, evn);
			}
		}

		_dict[buttonId][eventType].AddListener(onEvent);
	}

	//-----------------------------------------------------------------------------------------------------------------//
	public static void Send(string buttonId, EventType eventType, object userValue = null)
	{
		if (_dict.ContainsKey(buttonId))
		{
			if (_dict[buttonId].ContainsKey(eventType))
			{
				_dict[buttonId][eventType].Invoke(userValue);
			}
		}
	}
}
