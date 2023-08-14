using System.Collections;
using UnityEngine;
namespace Tyrs { 
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator enumerator);
        void StopCoroutine(Coroutine tickCoroutine);        
    }
}
