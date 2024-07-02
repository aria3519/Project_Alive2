using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameEventListener <T, E, UER> : MonoBehaviour,
    IGameEventListener<T> where E : BaseGameEvent<T> where UER : UnityEvent<T>
    {
    [SerializeField] private E gameEvent;
    public E GameEvent { get { return gameEvent; } set { gameEvent = value; } }

    [SerializeField] private UER unityEventResponse;



    private void OnEnable()
    {
        if (gameEvent == null) return;
        GameEvent.RegisterListener(this); // ���� Ŭ���� ���
    }
    private void OnDisable()
    {
        if (gameEvent == null) return;
        GameEvent.UnregisterListener(this); // ���� Ŭ���� ����
    }

    public void OnEventRaised(T item) // �̺�Ʈ �߻����� ��
    {
        if(unityEventResponse != null)
        {
            unityEventResponse.Invoke(item);
        }
    }
}
