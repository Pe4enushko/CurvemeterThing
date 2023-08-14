using Assets.Assets.Scipts.EduSystem.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Assets.Scipts.EduSystem
{
    public class EduAPIWorker
    {
        string _baseAddress = @"http://45.146.164.180:5454/";
        static string _accessToken = "";
        public static string _teacherId = "";
        public async Task<bool> Auth(string login, string password)
        {
            HttpClient client = new();
            client.BaseAddress = new Uri(_baseAddress);

            HttpResponseMessage response = await client.PostAsync(new Uri($"api/auth/unityUser?login={login}&password={password}"),null);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonUtility.FromJson<AuthResponse>(json);

            _accessToken = result.accessToken;
            _teacherId = result.id;

            return response.IsSuccessStatusCode;
        }
        public async Task<List<Student>> GetAllStudents()
        {
            HttpClient client = new();
            client.BaseAddress = new Uri(_baseAddress);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

            HttpResponseMessage response = await client.PostAsync(new Uri($"api_v2/student/teacher?teacherId={_teacherId}"), null);
            
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonUtility.FromJson<Student[]>(json);

            var firstStud = result[0];
            
            Debug.Log(firstStud);

            return result.ToList();
        }
    }
}