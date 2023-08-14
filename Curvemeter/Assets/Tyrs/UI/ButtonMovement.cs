using System.Collections;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Отвечает за движение кнопки
/// </summary>
public class ButtonMovement : MonoBehaviour
{
	[Header("Событие: При движении кнопка достигла нижних границ")]
    public UnityEvent reachingBottomBorder;	
	[Header("Событие: При движении кнопка достигла верхних границ")]
    public UnityEvent reachingUpperBorder;	
	
	[Header("Объект триггера кнопки который двигается в базе кнопки")]
	[SerializeField] private GameObject _trigger;
	[Header("Минимальная позиция триггера кнопки")]
	[SerializeField] private float _minHeight = 1;
	[Header("Максимальная позиция триггера кнопки")]
	[SerializeField] private float _maxHeight = 5;	
	[Header("Позиция триггера кнопки при залипании")]
	[SerializeField] private float _stickingHeight = 2;	
	[Header("Скорость возврата кнопки в дефолтное состояние (возврат в максимальную позицию триггера)")]
	[SerializeField] private float _recoverSpeed;
	[Header("Кнопка с залипанием?")]
	[SerializeField] private bool _isWithSticking;	
	//Залипла ли кнопка в данный момент
	private bool _isStick;
	//Высота порога нажимной части в процентах (к примеру 0.2f при 20% утопления триггера в базу считается нажатие)
	private float _triggerThresholdHeight = 0.2f;		
	//Для хранения стандартной максимальной высоты кнопки по дефолту, применяется при залипании кнопки (т.к. максимальное значение движения кнопки будет изменятся)
	private float _maxHeightDefault;
	//Корутина проверки нажатия на кнопку
    private Coroutine _checkMovementLimit;
	//Корутина возврата кнопки в стартовую позицию
    private Coroutine _returnToStartingPosition;
	//Для задания перманентного импульса триггера и сброса его при движении 
	private Rigidbody _triggerRigidbody;

	private UnityEvent _redyHidden = new UnityEvent();

    private void Awake()
    {
		_triggerRigidbody = _trigger.GetComponent<Rigidbody>();
        _maxHeightDefault = _maxHeight;
    }

    private void OnEnable() => _checkMovementLimit = StartCoroutine(CheckMovementLimit());

	/// <summary>
	/// При деактивации всего объекта все запущенные корутины и так останавливаются
	/// </summary>
    private void OnDisable()
    {
		if (_checkMovementLimit != null)
			StopCoroutine(_checkMovementLimit);

		if (_returnToStartingPosition != null)
			StopCoroutine(_returnToStartingPosition);
    }

    private void LateUpdate() => FixingInLocalSpace();

	/// <summary>
	/// Задать кинематику кнопки, если кинематика включена то кнопка не будет двигатся
	/// </summary>
	/// <param name="value"></param>
    public void SetKinematic(bool value) => _triggerRigidbody.isKinematic = value;

    /// <summary>
    /// Установить стартовую позицию для движения
    /// к примеру если кнопка изначально должна быть в положении нажата или наоборот
    /// </summary>
    public void SetStartMovePosition(bool isActive)
	{
		if (_isWithSticking == false)
			return;

		_isStick = isActive;//если кнопка активна то она должна быть залипной и наооборот			
		_maxHeight = _isStick ? _stickingHeight : _maxHeightDefault;
		_trigger.transform.localPosition = new Vector3(0.0f, 0.0f, _maxHeight);
	}

    /// <summary>
    /// Фиксация кнопки в локальном пространстве после действия на нее сил 
    /// что бы кнопка не двигалась за границы положенной для нее границы
    /// </summary>
    private void FixingInLocalSpace()
    {
        _trigger.transform.localPosition = new Vector3(0.0f, 0.0f, Mathf.Clamp(_trigger.transform.localPosition.z, _minHeight, _maxHeight));
		_triggerRigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// Проверка движения кнопки на достижении границы
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckMovementLimit()
	{
        while (true)
        {
			yield return null;
			if (IsPressing())
			{				
				reachingBottomBorder?.Invoke();
				_returnToStartingPosition = StartCoroutine(ReturnToStartingPosition());
				if (_isWithSticking)
				{					
					_isStick = !_isStick;					
					_maxHeight = _isStick ? _stickingHeight : _maxHeightDefault;					
				}
				StopCoroutine(_checkMovementLimit);					
			}
        }		
	}

	/// <summary>
	/// Возврат триггера в стартовую позицию
	/// </summary>
	/// <returns></returns>
	private IEnumerator ReturnToStartingPosition()
	{
        while (true)
        {			
			yield return null;
			Vector3 position = _trigger.transform.localPosition;
			position.z = _trigger.transform.localPosition.z + (_recoverSpeed * Time.deltaTime);
			_trigger.transform.localPosition = position;

			if (GetPressTriggerPercentage() >= 1)
			{ 
				StopCoroutine(_returnToStartingPosition);
				_checkMovementLimit = StartCoroutine(CheckMovementLimit());
				reachingUpperBorder?.Invoke();
			}									
        }		
	}

    /// <summary>
    /// Нажата ли кнопка
    /// </summary>
    /// <returns></returns>
    private bool IsPressing() 
		=> GetPressTriggerPercentage() <= _triggerThresholdHeight ? true : false;

    /// <summary>
    /// Получить текущий процент нажатия кнопки
    /// </summary>
    /// <returns></returns>
    private float GetPressTriggerPercentage() 
		=> Mathf.Clamp01((_trigger.transform.localPosition.z - _minHeight) / (_maxHeight - _minHeight));
}