using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RPGTutorial : MonoBehaviour
{
    public static bool TutorialIsGoing = false;

    GameObject[] RocketStarters;
    RocketLauncherBehaviour Launcher;

    AudioSource _player;

    bool _grenadeReady = false;
    bool _launcherReady = false;
    bool _triggerReady = false;

    public void StartingTutorial()
    {
        GetComponentInChildren<Canvas>().enabled = false;

        _player = GetComponent<AudioSource>();
        // "Рокету собери"
        PlayRecord("Speech1");

        RocketStarters = GameObject.FindGameObjectsWithTag("RocketStarter");
        Launcher = GameObject.FindGameObjectsWithTag("RocketLauncher")
            .FirstOrDefault(a => a.activeInHierarchy)
            .GetComponent<RocketLauncherBehaviour>();

        if (Launcher == null)
        {
            return;
        }

        foreach (var rs in RocketStarters)
        {
            if(rs.TryGetComponent<RocketStarterBehaviour>(out var component))
                component.OnRocketAttacment += GrenadeReadyStepDone;
        }

        Launcher.OnRocketRemoval += (s, e) => _launcherReady = false;
        Launcher.OnRocketAttacment += RocketAttachedToLauncherStepDone;
        Launcher.TriggerReady += TriggerReadyStepDone;
        Launcher.OnShot += ShotStepDone;
        Launcher.IsTutorial = true;
        TutorialIsGoing = true;
    }

    public void GrenadeReadyStepDone(object sender, EventArgs e)
    {
        if (TutorialIsGoing)
        {
            // "Рокету вставь"
            PlayRecord("Speech2");

            _grenadeReady = true;
        }
    }

    public void RocketAttachedToLauncherStepDone(object sender, EventArgs e)
    {
        if(_grenadeReady)
        {
            Launcher.ResetTrigger();
            // "Взведите курок"
            PlayRecord("Speech5");

            _launcherReady = true;
        }
    }

    public void TriggerReadyStepDone(object sender, EventArgs e)
    {
        if(_grenadeReady && _launcherReady)
        {
            // "Высоту правь и стреляй"
            PlayRecord("Speech3_4");

            _triggerReady = true;
        }
    }

    public void ShotStepDone(object sender, EventArgs e)
    {
        if (_grenadeReady 
            && _launcherReady 
            && _triggerReady
            && Launcher.TutorialHeightCorrect)
        {
            // Браво
            PlayRecord("RPG_end");
            Launcher.OnRocketAttacment -= RocketAttachedToLauncherStepDone;
            Launcher.TriggerReady -= TriggerReadyStepDone;
            Launcher.OnShot -= ShotStepDone;

            Launcher.IsTutorial = false;
            TutorialIsGoing = false;
        }
    }

    void PlayRecord(string resourceName)
    {
        if (_player != null)
        {
            _player.clip = Resources.Load<AudioClip>(resourceName);
            _player.Play();
        }
    }

}
