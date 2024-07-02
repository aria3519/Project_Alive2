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
        GameEvent.RegisterListener(this); // 현재 클래스 등록
    }
    private void OnDisable()
    {
        if (gameEvent == null) return;
        GameEvent.UnregisterListener(this); // 현재 클래스 삭제
    }

    public void OnEventRaised(T item) // 이벤트 발생했을 때
    {
        if(unityEventResponse != null)
        {
            unityEventResponse.Invoke(item);
        }
    }
}
