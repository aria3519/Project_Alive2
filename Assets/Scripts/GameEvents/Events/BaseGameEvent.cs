using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameEvent<T> : ScriptableObject
{
    // 읽기전용으로 리스너들을 저장하는 리스트를 만든다
    private readonly List<IGameEventListener<T>> eventListeners = new List<IGameEventListener<T>>();

    // 리스트에 저장된 리스너들을 반복문을 돌려 
    public void Raise(T item) // 리스너들에게 정보를 업데이트 해주는 부분.
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(item);
    }

    public void RegisterListener(IGameEventListener<T> listener) // 리스너 등록
    {
        if (!eventListeners.Contains(listener)) // listener인지 한번더 체크
            eventListeners.Add(listener);
    }

    public void UnregisterListener(IGameEventListener<T> listener) // 리스너 삭제
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
