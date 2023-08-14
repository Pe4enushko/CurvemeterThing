using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Assets.Scipts.EduSystem.ApiModels
{
    [Serializable]
    public class AuthResponse : ScriptableObject
    {
        public string id {get; set;}
        public string accessToken {get; set;}
        public string refreshToken { get; set; }
    }
}