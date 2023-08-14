using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Assets.Scipts.EduSystem.ApiModels
{
    public class Student : ScriptableObject
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Grade { get; set; }
        public DateTime? DateofBirth { get; set; }

        public bool? IsActive { get; set; }
        public string TeacherId { get; set; }
    }
}