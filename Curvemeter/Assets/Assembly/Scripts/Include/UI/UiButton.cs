using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent (typeof (CanvasGroup))]
public class UiButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[System.Serializable]
	public class GraphicColors
	{
		public Graphic graphic;
		[Space]
		public Color normalColor = Color.white;
		public Color downColor = Color.white;
		public Tweener tweener;
	}

	[Space]
	public string buttonId;
	public string buttonValue;
	public bool setIndexAsUserValue;
	[Space]
	[SerializeField]
	private bool _interactable = true;
	[Space (5)]
	[Range (0f, 1f)]
	public float disabledAlpha = 0.5f;
	[Header("COLORS")]
	[Space]
	public GraphicColors[] colors;
	[Space]
	public Transform transScale;
	public float scaleDown = 1f;
	[Space]
	public float transitionDuration = 0.1f;
	[Space (20)]
	public AudioClip soundDown;
	public float soundDownVolume = 1f;
	[Space (5)]
	public AudioClip soundClick;
	public float soundClickVolume = 1f;
	[Space (20)]
	public UnityEvent onClick;

	//-----------------------------------------------------------------------------------------------------------------//
	public object userValue;
	
	//-----------------------------------------------------------------------------------------------------------------//
	public bool interactable {
		get {
			return _interactable;
		}

		set {
			_interactable = value;
			
			if (_canvasGroup == null)
			{
				_canvasGroup = GetComponent <CanvasGroup> ();
			}
			
			_canvasGroup.blocksRaycasts = _interactable;
			_canvasGroup.alpha = _interactable ? 1f : disabledAlpha;
		}
	}

	//-----------------------------------------------------------------------------------------------------------------//
	private CanvasGroup _canvasGroup;
	private bool _isScreenSpace;
	private AudioSource _audioSource;
	private Transform _transListener;
	private Tweener _tweenerScale;
	
	//-----------------------------------------------------------------------------------------------------------------//
	void Awake ()
	{
		var canvas = GetComponentInParent<Canvas>();
		_isScreenSpace = canvas.renderMode != RenderMode.WorldSpace;

		if (_isScreenSpace)
		{
			_audioSource = gameObject.AddComponent<AudioSource>();
			_audioSource.playOnAwake = false;
		}
		else
		{
			var listener = FindObjectOfType<AudioListener>();
			if (listener != null)
			{
				_transListener = listener.transform;
			}
		}

		if (!string.IsNullOrEmpty(buttonValue)) {
			userValue = buttonValue;
		}
		else if (setIndexAsUserValue) {
			userValue = transform.GetSiblingIndex ();
		}
	}

	//-----------------------------------------------------------------------------------------------------------------//
	void OnEnable ()
	{
		Reset();
	}

	//-----------------------------------------------------------------------------------------------------------------//
	private void OnDestroy ()
	{
		foreach (var c in colors) {
			KillTweener(c.tweener);
		}
	}

	//-----------------------------------------------------------------------------------------------------------------//
	private void OnValidate ()
	{
		gameObject.name = "Button - " + buttonId;
		Reset();
	}

	//-----------------------------------------------------------------------------------------------------------------//
	public void Reset ()
	{
		if (colors != null) {
			foreach (var c in colors) {
				KillTweener(c.tweener);
	
				if (c.graphic != null) {
					c.graphic.color = c.normalColor;
				}
			}
		}

		if (transScale != null)
		{
			transScale.localScale = Vector3.one;
		}

		interactable = _interactable;
	}

	//-----------------------------------------------------------------------------------------------------------------//
	void KillTweener(Tweener tweener)
	{
		if ((tweener != null) && tweener.IsActive ()) {
			tweener.Kill ();
		}
	}

	//-----------------------------------------------------------------------------------------------------------------//
	void AnimateColor(Graphic graphic, ref Tweener tweener, Color color)
	{
		KillTweener(tweener);
		tweener = graphic.DOColor (color, transitionDuration);
	}

	//-----------------------------------------------------------------------------------------------------------------//
	void AnimateScale(float scale)
	{
		if (transScale == null)
		{
			return;
		}

		KillTweener(_tweenerScale);
		_tweenerScale = transScale.DOScale (scale, transitionDuration);
	}

	//-----------------------------------------------------------------------------------------------------------------//
	public void OnPointerEnter (PointerEventData eventData)
	{
		UiButtons.Send (buttonId, UiButtons.EventType.DOWN, userValue);

		foreach (var c in colors) {
			AnimateColor(c.graphic, ref c.tweener, c.downColor);
		}
		
		AnimateScale(scaleDown);
		PlaySound (soundDown, soundDownVolume);
	}

	//-----------------------------------------------------------------------------------------------------------------//
	public void OnPointerExit (PointerEventData eventData)
	{
		UiButtons.Send(buttonId, UiButtons.EventType.CLICK, userValue);
		onClick.Invoke();

		foreach (var c in colors) {
			AnimateColor(c.graphic, ref c.tweener, c.normalColor);
		}

		AnimateScale(1f);
		PlaySound (soundClick, soundClickVolume);
	}

	//-----------------------------------------------------------------------------------------------------------------//
	void PlaySound (AudioClip clip, float volume)
	{
		if (clip == null)
		{
			return;
		}

		if (_isScreenSpace)
		{
			_audioSource.clip = clip;
			_audioSource.volume = volume;
			_audioSource.Play();
		}
		else
		{
			Vector3 pos = _transListener.position + (transform.position - _transListener.position).normalized * 0.01f;
			AudioSource.PlayClipAtPoint (clip, pos, volume);					
		}
	}
}
