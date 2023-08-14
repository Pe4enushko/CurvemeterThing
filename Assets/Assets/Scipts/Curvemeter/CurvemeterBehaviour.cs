using Assets.Assets.Scipts.Curvemeter;
using HurricaneVR.Framework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurvemeterBehaviour : MonoBehaviour
{
    bool _isTouchngMap = false;
    bool _isGrabbed;
    float _distance = 0;
    bool _touchedA = false;
    bool _finished = false;
    bool _listenedToLecture = false;
    float koefUnitsToKilometers = 18.0849904124f;

    CurvemeterDirections _currentDirection = CurvemeterDirections.Forward;

    TextMeshPro _field;
    AudioSource _speaker;
    Material HoloArrowDisplayerMaterial;

    Vector3 StartPoint;
    List<Vector3> Points = new();
    List<Vector3> NegativePoints = new();
    float DistanceKm { get => _distance;
        set
        {
            float degrees = value / 3.545f;
            
            HoloArrowDisplayerMaterial.SetFloat("_Degrees", degrees / 0.07423352631f);
            CurvemeterArrow.transform.localEulerAngles = new Vector3(0, 0, 124.72f + degrees / 0.07147605533f);
            NumericDisplay.text = Mathf.Round(value).ToString() + " км";

            _distance = value;
        } }
    bool CanAddPoint => GetDistanceFromLastPoint() >= Accuracy;
    bool IsTouchingMap
    {
        get => _isTouchngMap;
        set
        {
            _isTouchngMap = value;
        }
    }
    bool IsGrabbed
    {
        get => _isGrabbed;
        set
        {
            CurvemeterHolo.GetComponent<MeshRenderer>().enabled = value;
            HoloArrowDisplay.GetComponent<MeshRenderer>().enabled = value;
            NumericDisplay.GetComponent<MeshRenderer>().enabled = value;

            _isGrabbed = value;
        }
    }
    /// <summary>
    /// Расстояние между точками, которыми курвиметр помечает пройденный путь
    /// </summary>
    public float Accuracy = 0.3f;
    /// <summary>
    /// Стрелочка на голограмме курвиметра
    /// </summary>
    public GameObject HoloArrowDisplay;
    /// <summary>
    /// Стрелочка на самом курвиметре
    /// </summary>
    public GameObject CurvemeterArrow;
    public GameObject CurvemeterHolo;
    public TextMeshPro NumericDisplay;

    Rigidbody thisRb;

    // Start is called before the first frame update
    void Start()
    {
        thisRb = GetComponent<Rigidbody>();
        if (HoloArrowDisplay != null)
        {
            HoloArrowDisplayerMaterial = HoloArrowDisplay.GetComponent<MeshRenderer>().material;
            print(HoloArrowDisplayerMaterial.name);
        }
        _speaker = GetComponent<AudioSource>();

        var grabbable = GetComponent<HVRGrabbable>();
        grabbable.Grabbed.AddListener((a, b) => 
        {
            IsGrabbed = true;
            if (!_listenedToLecture) 
            { 
                say("curvemeter_intro"); 
                _listenedToLecture = true; 
            }
        });
        grabbable.Released.AddListener((a, b) =>
        {
            IsGrabbed = false;
        });

        var _workingPart = transform.GetComponentInChildren<CurvemeterWorkingPart>();
        _workingPart.onMapEnter += EnterMap;
        _workingPart.onMapLeave += LeaveMap;

        var _directionGetter = transform.parent.GetComponentInChildren<DirectionTriggers>();
        //_directionGetter.OnDirectionTriggered += ChangeDirection;
    }
    void ChangeDirection(CurvemeterDirections newDirection)
    {
        _currentDirection = newDirection;
        print(newDirection);
    }
    private void EnterMap(Collider collision)
    {
        print("Reset points");

        Points.Clear();
        NegativePoints.Clear();
        AddPoint(_currentDirection);
            
        IsTouchingMap = true;
        
        if (collision.gameObject.name == "A")
        {
            _touchedA = true;
        }
    }

    private void FixedUpdate()
    {
        if (IsTouchingMap)
        {
            // Check direction by velocity
            if (thisRb.velocity.x > 0)
            {
                ChangeDirection(CurvemeterDirections.Forward);
            }
            else if (thisRb.velocity.x < 0)
            {
                ChangeDirection(CurvemeterDirections.Backward);
            }

            if (thisRb.velocity.z > 2)
            {
                ChangeDirection(CurvemeterDirections.Other);
            }
            else if (thisRb.velocity.z < -2)
            {
                ChangeDirection(CurvemeterDirections.Other);
            }

            if (CanAddPoint)
            {
                AddPoint(_currentDirection);
                DistanceKm = GetAllPointsDistance(Points, NegativePoints);
                print("new distance: " + DistanceKm);
            }

            if (DistanceKm > 7.85f && !_finished)
            {
                say("curvemeter_distance");
                _finished = true;
            }
        }
    }

    private void LeaveMap(Collider collision)
    {
        IsTouchingMap = false;
        _touchedA = false;   
    }
    void AddPoint(CurvemeterDirections direction)
    {
        switch (direction)
        {
            case CurvemeterDirections.Forward:
                Points.Add(transform.position);
                break;
            case CurvemeterDirections.Backward:
                NegativePoints.Add(transform.position);
                break;
            default:
                break;
        }
    }
    float GetAllPointsDistance(List<Vector3> positivePoints, List<Vector3> negativePoints)
    {
        if (positivePoints.Count <= 1)
        {
            return 0;
        }

        float result = 0;

        for (int i = 1; i < positivePoints.Count; i++)
        {
            result += GetDistanceBetweenTwoPoints(positivePoints[i - 1], positivePoints[i]);
        }

        for (int i = 1; i < negativePoints.Count; i++)
        {
            result -= GetDistanceBetweenTwoPoints(negativePoints[i - 1], negativePoints[i]);
        }

        return result;
    }
    float GetDistanceFromLastPoint()
    {
        if (Points.Count < 1)
        {
            return 0;
        }

        return GetDistanceBetweenTwoPoints(Points[Points.Count - 1], transform.position);
    }
    float GetDistanceBetweenTwoPoints(Vector3 A, Vector3 B)
    {
        Vector2 AFlat = new(A.x, A.z);
        Vector2 BFlat = new(B.x, B.z);

        float distanceInUnits = Mathf.Abs(Vector2.Distance(AFlat, BFlat));

        return distanceInUnits * koefUnitsToKilometers;
    }
    void say(string clipName)
    {
        AudioClip clip = Resources.Load<AudioClip>(clipName);
        if (clip != null)
        {
            _speaker.clip = clip;
            _speaker.Play();
        }
        else
        {
            print($"Клип с названием \"{clipName}\" не найден");
        }
    }
}
