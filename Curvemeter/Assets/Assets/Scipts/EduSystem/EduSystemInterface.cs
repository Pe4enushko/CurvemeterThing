using Assets.Assets.Scipts.EduSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EduSystemInterface : MonoBehaviour
{
    
    public TMP_InputField Login;
    public TMP_InputField Password;
    // Start is called before the first frame update
    public async void TryLogin()
    {
        EduAPIWorker api = new EduAPIWorker();

        print(await api.Auth("mr.stone9999@gmail.cpm", "123"));

        api.GetAllStudents();
    }
}
