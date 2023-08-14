using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Assets.Scipts.EduSystem.ApiModels
{
    public class Game : ScriptableObject
    {
        public string Id { get; set; }

        public string StudentId { get; set; }
        public int Score { get; set; }
        public string GameType { get; set; }
        public DateTime CreateDateTime { get; set; }

        public string TeacherId { get; set; }
        public string unityGameId { get; set; }
        public DateTime GameStartTime { get; set; }
    }
}