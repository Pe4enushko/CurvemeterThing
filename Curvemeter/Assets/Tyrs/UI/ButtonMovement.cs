using System.Collections;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// �������� �� �������� ������
/// </summary>
public class ButtonMovement : MonoBehaviour
{
	[Header("�������: ��� �������� ������ �������� ������ ������")]
    public UnityEvent reachingBottomBorder;	
	[Header("�������: ��� �������� ������ �������� ������� ������")]
    public UnityEvent reachingUpperBorder;	
	
	[Header("������ �������� ������ ������� ��������� � ���� ������")]
	[SerializeField] private GameObject _trigger;
	[Header("����������� ������� �������� ������")]
	[SerializeField] private float _minHeight = 1;
	[Header("������������ ������� �������� ������")]
	[SerializeField] private float _maxHeight = 5;	
	[Header("������� �������� ������ ��� ���������")]
	[SerializeField] private float _stickingHeight = 2;	
	[Header("�������� �������� ������ � ��������� ��������� (������� � ������������ ������� ��������)")]
	[SerializeField] private float _recoverSpeed;
	[Header("������ � ����������?")]
	[SerializeField] private bool _isWithSticking;	
	//������� �� ������ � ������ ������
	private bool _isStick;
	//������ ������ �������� ����� � ��������� (� ������� 0.2f ��� 20% ��������� �������� � ���� ��������� �������)
	private float _triggerThresholdHeight = 0.2f;		
	//��� �������� ����������� ������������ ������ ������ �� �������, ����������� ��� ��������� ������ (�.�. ������������ �������� �������� ������ ����� ���������)
	private float _maxHeightDefault;
	//�������� �������� ������� �� ������
    private Coroutine _checkMovementLimit;
	//�������� �������� ������ � ��������� �������
    private Coroutine _returnToStartingPosition;
	//��� ������� ������������� �������� �������� � ������ ��� ��� �������� 
	private Rigidbody _triggerRigidbody;

	private UnityEvent _redyHidden = new UnityEvent();

    private void Awake()
    {
		_triggerRigidbody = _trigger.GetComponent<Rigidbody>();
        _maxHeightDefault = _maxHeight;
    }

    private void OnEnable() => _checkMovementLimit = StartCoroutine(CheckMovementLimit());

	/// <summary>
	/// ��� ����������� ����� ������� ��� ���������� �������� � ��� ���������������
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
	/// ������ ���������� ������, ���� ���������� �������� �� ������ �� ����� ��������
	/// </summary>
	/// <param name="value"></param>
    public void SetKinematic(bool value) => _triggerRigidbody.isKinematic = value;

    /// <summary>
    /// ���������� ��������� ������� ��� ��������
    /// � ������� ���� ������ ���������� ������ ���� � ��������� ������ ��� ��������
    /// </summary>
    public void SetStartMovePosition(bool isActive)
	{
		if (_isWithSticking == false)
			return;

		_isStick = isActive;//���� ������ ������� �� ��� ������ ���� �������� � ���������			
		_maxHeight = _isStick ? _stickingHeight : _maxHeightDefault;
		_trigger.transform.localPosition = new Vector3(0.0f, 0.0f, _maxHeight);
	}

    /// <summary>
    /// �������� ������ � ��������� ������������ ����� �������� �� ��� ��� 
    /// ��� �� ������ �� ��������� �� ������� ���������� ��� ��� �������
    /// </summary>
    private void FixingInLocalSpace()
    {
        _trigger.transform.localPosition = new Vector3(0.0f, 0.0f, Mathf.Clamp(_trigger.transform.localPosition.z, _minHeight, _maxHeight));
		_triggerRigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// �������� �������� ������ �� ���������� �������
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
	/// ������� �������� � ��������� �������
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
    /// ������ �� ������
    /// </summary>
    /// <returns></returns>
    private bool IsPressing() 
		=> GetPressTriggerPercentage() <= _triggerThresholdHeight ? true : false;

    /// <summary>
    /// �������� ������� ������� ������� ������
    /// </summary>
    /// <returns></returns>
    private float GetPressTriggerPercentage() 
		=> Mathf.Clamp01((_trigger.transform.localPosition.z - _minHeight) / (_maxHeight - _minHeight));
}