using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameEvent<T> : ScriptableObject
{
    // �б��������� �����ʵ��� �����ϴ� ����Ʈ�� �����
    private readonly List<IGameEventListener<T>> eventListeners = new List<IGameEventListener<T>>();

    // ����Ʈ�� ����� �����ʵ��� �ݺ����� ���� 
    public void Raise(T item) // �����ʵ鿡�� ������ ������Ʈ ���ִ� �κ�.
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(item);
    }

    public void RegisterListener(IGameEventListener<T> listener) // ������ ���
    {
        if (!eventListeners.Contains(listener)) // listener���� �ѹ��� üũ
            eventListeners.Add(listener);
    }

    public void UnregisterListener(IGameEventListener<T> listener) // ������ ����
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}
