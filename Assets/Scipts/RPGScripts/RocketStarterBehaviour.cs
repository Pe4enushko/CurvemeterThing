using UnityEngine;
using System;

public class RocketStarterBehaviour : MonoBehaviour
{
    public event EventHandler OnRocketAttacment;
    public event EventHandler OnRocketRemoval;

    float _shotStrength = 1500;
    float _connectionStrength = float.PositiveInfinity;
    ConfigurableJoint _rocketJoint;
    Rigidbody _attachedRocket;
    
    public bool Shot = false;
    public Rigidbody AttachedRocket
    {
        get => _attachedRocket;
        set
        {
            var rb = GetComponent<Rigidbody>();
            if (value is not null)
            {
                var rocket = value.transform;
                var target = transform.Find("RocketJoinPosition");

                // Подправляем положение рокеты
                rocket.position = target.position;
                rocket.rotation = target.rotation;
                rb.mass = value.mass;
                rb.drag = value.drag;

                // Закрепляем
                AttachRocket(value);
                _attachedRocket = value;
            }
            else
            {
                _attachedRocket = value;
                rb.mass = 1;
                rb.drag = 0;
            }
        }
    }

    public RocketTypes AttachedRocketType
    {
        get => AttachedRocket?
            .GetComponent<Rocket>()
            .RocketType ?? RocketTypes.None;
    }

    protected void AttachRocket(Rigidbody rocket)
    {
        if (_rocketJoint is null)
        {
            var newJoint = gameObject.AddComponent<ConfigurableJoint>();
            newJoint.breakForce = _connectionStrength;
            newJoint.xMotion = newJoint.yMotion
                = newJoint.zMotion = newJoint.angularYMotion
                = newJoint.angularXMotion = newJoint.angularZMotion
                = ConfigurableJointMotion.Locked;
            newJoint.enableCollision = false;
            newJoint.connectedBody = rocket;

            _rocketJoint = newJoint;

            //rocket.GetComponent<HVRGrabbable>().enabled = false;
        }
    }

    private void OnJointBreak(float breakForce)
    {
        _rocketJoint = null;
        //_attachedRocket.GetComponent<HVRGrabbable>().enabled = true;
    }

    void OnTriggerEnter(Collider obj)
    {
        // Прикрепление гранаты
        if (obj.tag == "Rocket"
            && AttachedRocket == null
            && !obj.GetComponent<Rocket>().isAttached)
        {
            obj.GetComponent<Rocket>().isAttached = true;
            AttachedRocket = obj.attachedRigidbody;
            OnRocketAttacment?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Shoot()
    {
        var thisRb = GetComponent<Rigidbody>();
        if (!Shot 
            && AttachedRocket != null
            && !AttachedRocket.isKinematic
            && !thisRb.isKinematic)
        {
            Shot = true;
            _rocketJoint.massScale = 0.1f;
            // Запуск
            
            thisRb?.AddForce(-thisRb.transform.forward * _shotStrength, ForceMode.Impulse);
        }
    }
    public void SelfDestruct()
    {
        Destroy(AttachedRocket?.gameObject);
        Destroy(gameObject);
    }
}

