using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class ObjectPoolingManager : MonoBehaviour
//{
//    [System.Serializable]
//    public class PoolObjectData
//    {
//        public GameObject prefab;

//        [Tooltip("poolCount 최소 1개 이상 설정")]
//        public int poolCount = 10;

//        public string explan;
//    }

//    [SerializeField]
//    PoolObjectData test;

//    private void Awake()
//    {

//    }
//}

[System.Serializable]
public class PoolObjectData
{
    public GameObject prefab;

    [Tooltip("poolCount는 최소 1개 이상 설정")]
    public int poolCount = 10;

    [Header("해당 오브젝트 설명")]
    public string explan;
}

/// <summary> Object Pooling Manager </summary>
public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager instance;

    [SerializeField] private bool dontDestroy = true;
    [SerializeField] private List<PoolObjectData> poolList = new List<PoolObjectData>();

    private Dictionary<string, Transform> poolParentDic = new Dictionary<string, Transform>();                  // 각각 풀링들의 부모
    private Dictionary<string, PoolObjectData> instantiateObject = new Dictionary<string, PoolObjectData>();    // 풀링으로 생성된 오브젝트

    /*
    PoolObjectData 를 직렬화 한다음 원하는 GameObject들을 인스펙터 창에서 받아 poolList 에 저장한다.
    캐싱에서
    MakeDir 메소드를 통해 먼저 "Using" key를 갖는 부모를 설정해주고
    초기 풀링 -> 받아온 poolList를 오브젝트 풀링을 시작한다. 즉 GameObject 형태로 모든 프리팹들을 다 받아와서 하나의 리스트에 저장해둔 셈
    
    이후 각 GameObject 의 프리팹 이름 별로 나누어 부모 딕셔너리 poolParentDic 에 나눠서 저장해준다.
      프리팹 이름을 먼저 비교하고 없으면 -> 프리팹 이름을 부모 딕셔너리의 리스트로 생성 후 Instantiate 실행.

    결론은 하이어라키 창에 빈 부모 오브젝트 -> 생성한 자식 풀링 오브젝트들을 보기 편하게 만들어 놓은것
    또한 클래스 타입을 나누어 리스트를 여러개 생성해서 딕셔너리에 집어넣는게 아니라 GameObject로 한꺼번에 받아오고
    자식들의 이름 별로 나누어 딕셔너리에 저장하여 더 효율적임
     */

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;

            if (dontDestroy == true)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);   // 오브젝트 풀링 중복 방지
        }

        MakeDir("Using");
        Pool(poolList);   // 초기 풀링에 들어가있는 오브젝트 풀링
    }

    public int GetPoolCount(string _id)
    {
        for(int i = 0; i < poolList.Count; i++)
        {
            if (_id == poolList[i].prefab.name)
                return poolList[i].poolCount;
            else
                continue;
        }

        return 0;
    }

    #region Instantiate
    public void Pool(List<PoolObjectData> _poolObjectList)
    {
        GameObject prefab;

        string prefabName;
        int count;

        PoolObjectData data;
        for (int i = 0; i < _poolObjectList.Count; i++)
        {
            data = _poolObjectList[i];

            prefab = data.prefab;
            prefabName = prefab.name;
            count = data.poolCount;

            if (prefab == null)
            {
                Debug.LogError($" <color=red> ObjectPooler Instantiate Error</color> : <color=yellow>Index</color> <color=orange>{i}</color> <color=yellow>Is prefab missing</color>");
                continue;
            }

            if (poolParentDic.ContainsKey(prefabName) == false)
            {
                MakeDir(prefabName);
                InstantiatePool(prefab, poolParentDic[prefabName], count);

                instantiateObject[prefabName] = data;
            }
            else
            {
                continue;
            }
        }
    }

    /// <summary> 오브젝트를 반환하거나 생성할 부모를 생성 </summary>
    void MakeDir(string _name)
    {
        GameObject newDir = new GameObject(_name);
        newDir.transform.SetParent(this.transform);
        poolParentDic[_name] = newDir.transform;
    }

    /// <summary> 오브젝트를 갯수 만큼 생성하고 마지막 생성된 오브젝트를 반환합니다 </summary>
    GameObject InstantiatePool(GameObject _prefab, Transform _parent, int _count)
    {
        if (_count <= 0)
        {
            Debug.LogError($" <color=red> ObjectPooler Instantiate Error </color> <color=Orange>{_prefab.name}</color> <color=yellow> is pooling count 0</color>");
            return null;
        }

        GameObject inst = null;
        for (int i = 0; i < _count; i++)
        {
            inst = Instantiate(_prefab, Vector3.zero, Quaternion.identity, _parent);
            inst.name = _prefab.name;
            inst.SetActive(false);
        }
        return inst;
    }

    #endregion

    #region  Get

    /// <summary> GameObject 타입을 받지않고 제네릭을 사용하여 원하는 클래스타입으로 바꿔서 불러오기 </summary>
    public T GetObject<T>(string _id, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트 불러오기 </summary>
    public GameObject GetObject(string _id, Transform _parent = null, bool _enable = true)
    {
        if (poolParentDic.ContainsKey(_id) == false)
        {
            Debug.LogErrorFormat(" <color=red> ObjectPooler Get Error </color> : <color=yellow>not found object name </color> <color=orange> {0} </color>", _id);
            return null;
        }

        if (poolParentDic[_id].childCount > 0)
        {
            Transform pool = poolParentDic[_id].GetChild(0);
            if (pool.gameObject.activeSelf == false)
            {
                if (_parent == null)
                {
                    pool.SetParent(poolParentDic["Using"]);
                }
                else
                {
                    pool.SetParent(_parent);
                }

                pool.gameObject.SetActive(_enable);
                return pool.gameObject;
            }
        }

        // 부족한 경우 추가 생성 후 하나를 반환해 줍니다.
        return InstantiatePool(instantiateObject[_id].prefab, poolParentDic[_id], instantiateObject[_id].poolCount);
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 반환합니다. </summary>
    public GameObject GetObject(string _id, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _parent, false);
        rtnObject.transform.SetPositionAndRotation(_position, _rotation);
        rtnObject.SetActive(_enable);
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 특정 타입의 컨퍼넌트를 반환합니다 </summary>
    public T GetObject<T>(string _id, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _position, _rotation, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 _rtnTime뒤 자동 리턴됩니다. </summary>
    public GameObject GetObject(string _id, Vector3 _position, Quaternion _rotation, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _position, _rotation, _parent, _enable);
        StartCoroutine(WaitReturnObject(rtnObject, _rtnTime));
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 _rtnTime뒤 자동 반환 및 컨퍼넌트 반환 </summary>
    public T GetObject<T>(string _id, Vector3 _position, Quaternion _rotation, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _position, _rotation, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정하고 반환합니다. </summary>
    public GameObject GetObject(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _parent, false);
        rtnObject.transform.SetPositionAndRotation(_position, _rotation);
        rtnObject.transform.localScale = _scale;
        rtnObject.SetActive(_enable);
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 컨퍼넌트 반환 </summary>
    public T GetObject<T>(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _position, _rotation, _scale, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 일정시간 뒤 자동 리턴 </summary>
    public GameObject GetObject(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _position, _rotation, _scale, _parent, _enable);
        StartCoroutine(WaitReturnObject(rtnObject, _rtnTime));
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 일정시간 뒤 자동 리턴 및 특정 컨퍼너는 반환 </summary>
    public T GetObject<T>(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _position, _rotation, _scale, _rtnTime, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 오브젝트가 비활성화 될 경우 자동 리턴 </summary>
    public GameObject GetObjectToAutoReturn(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _position, _rotation, _scale, _parent, _enable);
        StartCoroutine(AutoReturn(rtnObject));
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 오브젝트가 비활성화 될 경우 자동 리턴 실행 및 컨퍼넌트 반환 </summary>
    public T GetObjectToAutoReturn<T>(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        return GetObjectToAutoReturn(_id, _position, _rotation, _scale, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 일정 시간뒤 오브젝트 반환 </summary>
    public GameObject GetObject(string _id, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _parent, _enable);
        StartCoroutine(WaitReturnObject(rtnObject, _rtnTime));
        return rtnObject;
    }

    /// <summary> 컴퍼넌트 반환 </summary>
    public T GetObject<T>(string _id, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _rtnTime, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트 포지션과, 로테이션 및 부모를 설정 및 자동 리턴 실행 </summary>
    public GameObject GetObjectToAutoReturn(string _id, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _position, _rotation, _parent, _enable);
        StartCoroutine(AutoReturn(rtnObject));
        return rtnObject;
    }

    /// <summary> 오브젝트 포지션과, 로테이션 및 부모를 설정 및 자동 리턴 실행 및 특정 컨퍼넌트 반환 </summary>

    public T GetObjectToAutoReturn<T>(string _id, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        return GetObjectToAutoReturn(_id, _position, _rotation, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 부모 설정 및 오브젝트 비활성화시 자동 리턴 </summary>
    public GameObject GetObjectToAutoReturn(string _id, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _parent, _enable);
        StartCoroutine(AutoReturn(rtnObject));
        return rtnObject;
    }

    /// <summary> 특정 컨퍼넌트 반환 </summary>
    public T GetObjectToAutoReturn<T>(string _id, Transform _parent = null, bool _enable = true)
    {
        return GetObjectToAutoReturn(_id, _parent, _enable).GetComponent<T>();
    }

    #endregion

    #region Return

    /// <summary> 오브젝트 리턴 </summary>
    public void ReturnObject(GameObject _object)
    {
        if (_object == null)
        {
            Debug.LogErrorFormat(" <color=red> ObjectPooler Return Error </color> : <color=yellow> return object is null</color>");
            return;
        }

        if (poolParentDic.ContainsKey(_object.name) == false)
        {
            Debug.LogWarningFormat(" <color=red> ObjectPooler Return Error </color> : <color=yellow> not found object name is</color> <color=orange> {0} </color>", _object.name);
            return;
        }

        _object.transform.SetParent(poolParentDic[_object.name]);
        _object.SetActive(false);
    }

    private IEnumerator WaitReturnObject(GameObject _object, float _waitTIme)
    {
        yield return new WaitForSeconds(_waitTIme);
        ReturnObject(_object);
    }

    private IEnumerator AutoReturn(GameObject _object)
    {
        while (_object.activeSelf == true)
        {
            yield return null;
        }
        ReturnObject(_object);
    }
    #endregion
}