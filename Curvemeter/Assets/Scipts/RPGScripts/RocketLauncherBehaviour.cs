using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using System;
using UnityEngine;

public class RocketLauncherBehaviour : MonoBehaviour
{
    public event EventHandler OnShot;
    public event EventHandler OnRocketAttacment;
    public event EventHandler OnRocketRemoval;
    public event EventHandler TriggerReady;
    public event EventHandler OnHeightIsCorrect;

    public GameObject EmittedRocketPrefab_OG_7V; //OG
    public GameObject EmittedRocketPrefab_PG_7VL; //L
    public GameObject EmittedRocketPrefab_PG_7VS; //M

    public GameObject ReadyRocketPrefab_OG_7V; //OG
    public GameObject ReadyRocketPrefab_PG_7VL; //L
    public GameObject ReadyRocketPrefab_PG_7VS; //M

    public RocketTypes AttachedRocketType { get => _attachedRocketStarter
            ?.AttachedRocketType ?? RocketTypes.None;
    }
    public bool IsTutorial {
        get => _isTutorial;
        set 
        { 
            if (value)
            {
                InvokeRepeating("CheckHeight", 0.2f, 0.1f);
            }
            else
            {
                CancelInvoke("CheckHeight");
            }
            _isTutorial = value;
        }
    }
    [HideInInspector]
    public bool TutorialHeightCorrect;

    object _locker = new object();

    bool _isTutorial = false;

    bool _rocketAttached = false;

    bool _triggerReady = false;

    MeshRenderer _scopeRenderer;

    RocketStarterBehaviour _attachedRocketStarter;

    ConfigurableJoint _rocketStarterJoint;

    Transform _trigger;

    HVRGrabbable _grabbable;
    //public RocketTypes TestRocketType = RocketTypes.OG;

    private void Awake()
    {
        _grabbable = GetComponent<HVRGrabbable>();

        var trigger = transform.Find("RPG7").Find("ch")
            .GetComponent<HVRGrabbable>();
        
        trigger.Grabbed
            .AddListener(PrepareTrigger);
        _trigger = trigger.transform;
    }
    private void Update()
    {
        if (CheckTrigger())
            Shoot();
        // Чтобы быстро тестировать выстрелы мышкой
        //if (Input.GetMouseButtonUp(0))
        //    TestShoot(TestRocketType);
    }
    bool CheckTrigger()
    {
        if (_grabbable != null
            && _grabbable.HandGrabbers.Count > 0)
        {
            var controller = _grabbable.HandGrabbers[0].Controller;

            return controller.Trigger > 0.7 && _triggerReady;
        }
        return false;
    }

    void PrepareTrigger(HVRGrabberBase bs, HVRGrabbable gr)
    {
        if(!_triggerReady)
        {
            _trigger.Rotate(new Vector3(0, 0, -84.459f), Space.Self);
            bs.ForceRelease();
            TriggerIsReady();
        }
    }
    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "RocketStarter" 
            && !_rocketAttached
            && obj.attachedRigidbody != null
            && !obj.attachedRigidbody.isKinematic
            && !obj.GetComponent<RocketStarterBehaviour>().Shot
            && obj.GetComponent<RocketStarterBehaviour>().AttachedRocket != null)
        {
            var rocketBody = obj.attachedRigidbody;
            var heldInHand = obj.GetComponent<RocketStarterBehaviour>();
            RocketTypes rType = heldInHand.AttachedRocketType;

            var anchor = transform.Find("rocketPosition");

            var rocket = InstantiateReadyRocketByType(rType, anchor);
            
            if (rocket == null)
            {
                return;
            }

            heldInHand.SelfDestruct();

            // Крепим стартуйку рокеты
            var RocketStarterJoint = gameObject.AddComponent<ConfigurableJoint>();
            RocketStarterJoint.breakForce = float.PositiveInfinity;
            RocketStarterJoint.xMotion = RocketStarterJoint.yMotion 
                = RocketStarterJoint.zMotion = RocketStarterJoint.angularYMotion
                = RocketStarterJoint.angularXMotion = RocketStarterJoint.angularZMotion
                = ConfigurableJointMotion.Locked;
            RocketStarterJoint.enableCollision = false;
            RocketStarterJoint.massScale = 0.1f;
            RocketStarterJoint.connectedMassScale = 10;


            RocketStarterJoint.connectedBody = rocket.GetComponent<Rigidbody>();
            
            _rocketStarterJoint = RocketStarterJoint;
            _attachedRocketStarter = rocket;
            _rocketAttached = true;
            OnRocketAttacment?.Invoke(this,EventArgs.Empty);
        }
    }
    void OnJointBreak(float breakForce)
    {
        _rocketAttached = false;
    }
    
    public void TestShoot(RocketTypes rType)
    {
        var spawner = transform.Find("rocketSpawner");

        InstantiateEmittedRocketByType(rType, spawner).Shoot();
    }
    public void Shoot()
    {
        if (_attachedRocketStarter != null
        && !_attachedRocketStarter.Shot
        && _rocketAttached
        && (!IsTutorial || (IsTutorial && TutorialHeightCorrect)))
        {
            RocketTypes rType = _attachedRocketStarter
                .GetComponent<RocketStarterBehaviour>()
                .AttachedRocketType;

            var spawner = transform.Find("rocketSpawner");

            _attachedRocketStarter.SelfDestruct();
            Destroy(_rocketStarterJoint);

            InstantiateEmittedRocketByType(rType, spawner).Shoot();

            var audio = GetComponent<AudioSource>();
            audio.Play();

            _rocketStarterJoint = null;
            _rocketAttached = false;
            ResetTrigger();
            _attachedRocketStarter = null;
            OnShot?.Invoke(this, EventArgs.Empty);
        }
    }
    public void ResetTrigger()
    {
        if (_triggerReady)
        {
            _trigger.Rotate(new Vector3(0, 0, 84.459f), Space.Self);
            _triggerReady = false;
        }
    }
    public void TriggerIsReady()
    {
        _triggerReady = true;
        TriggerReady?.Invoke(this, EventArgs.Empty);
    }
    void CheckHeight()
    {
        TutorialHeightCorrect = IsHeightCorrect();
        ChangeScopeColor(TutorialHeightCorrect);
    }
    public bool IsHeightCorrect()
    {
        float desiredRotation;
        switch (AttachedRocketType)
        {
            case RocketTypes.OG: desiredRotation = 360 - 0.9f;
                break;
            case RocketTypes.L: desiredRotation = 0.44f;
                break;
            case RocketTypes.M: desiredRotation = 1.73f;
                break;
            default: return false;
        }
        
        var rotation = transform.rotation.eulerAngles;
        var desiredAngle = new Vector3(desiredRotation, rotation.y, rotation.z);
        var distance = Vector3.Distance(rotation, desiredAngle);

        // погрешность 0.3
        return distance < 0.5f || distance > 360 - 0.3f && IsTutorial;
    }
    /// <summary>
    /// Включает/Выключает подсветку прицела
    /// </summary>
    /// <remarks>1 - подсвечено, 2 - не подсвечено</remarks>
    /// <param name="value"></param>
    void ChangeScopeColor(bool value)
    {
        if (_scopeRenderer == null)
        {
            _scopeRenderer = transform.FindChildRecursive("lens")
                .GetComponent<MeshRenderer>();
        }
        _scopeRenderer.material.SetFloat("_IsHeightCorrect", value ? 1 : 0);
    }
    RocketStarterBehaviour InstantiateEmittedRocketByType(RocketTypes rType,Transform anchor)
    {
        GameObject rocket;
        switch (rType)
        {
            case RocketTypes.OG:
                rocket = Instantiate(EmittedRocketPrefab_OG_7V, anchor.position, anchor.rotation);
                break;
            case RocketTypes.L:
                rocket = Instantiate(EmittedRocketPrefab_PG_7VL, anchor.position, anchor.rotation);
                break;
            case RocketTypes.M:
                rocket = Instantiate(EmittedRocketPrefab_PG_7VS, anchor.position, anchor.rotation);
                break;
            default:
                return null;
        }
        return rocket.GetComponentInChildren<RocketStarterBehaviour>();
    }
    RocketStarterBehaviour InstantiateReadyRocketByType(RocketTypes rType, Transform anchor)
    {
        GameObject rocket;
        switch (rType)
        {
            case RocketTypes.OG:
                rocket = Instantiate(ReadyRocketPrefab_OG_7V, anchor.position, anchor.rotation, transform);
                break;
            case RocketTypes.L:
                rocket = Instantiate(ReadyRocketPrefab_PG_7VL, anchor.position, anchor.rotation, transform);
                break;
            case RocketTypes.M:
                rocket = Instantiate(ReadyRocketPrefab_PG_7VS, anchor.position, anchor.rotation, transform);
                break;
            default:
                return null;
        }
        return rocket.GetComponentInChildren<RocketStarterBehaviour>();
    }
}
