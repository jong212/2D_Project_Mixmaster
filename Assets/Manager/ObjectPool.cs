using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private void Awake()
    {
        //List<GameObject> : ���� �̸� ������ ������ ����Ʈ �ڷ���
        list = new Dictionary<string, List<GameObject>>();
        //inActiveList : ��Ȱ��ȭ ����Ʈ
        inActiveList = new Dictionary<string, List<GameObject>>();
    }
    //������ƮǮ�� ������ �ִ� ����
    public int maxCount = 300;
    public Dictionary<string, GameObject> dicPrefabs;
    public Dictionary<string, List<GameObject>> list;
    // �Ⱥ��̴� ����� ���� �����ϰ�ʹ�.
    Dictionary<string, List<GameObject>> inActiveList;

    /// <summary>
    /// ObjectPool�� ��ü�� �̸� �����ϰ�ʹ�.
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="parent"></param>
    /// <param name="amount"></param>
    public void CreateInstance(string prefabName, Transform parent, int amount)
    {
        string key = prefabName;
        maxCount = amount;
        if (dicPrefabs == null)
        {
            dicPrefabs = new Dictionary<string, GameObject>();
        }
        GameObject prefab;
        if (dicPrefabs.ContainsKey(key))
        {
            prefab = dicPrefabs[key];
        }
        else
        {
            prefab = (GameObject)Resources.Load("Prefabs/" + prefabName);
            dicPrefabs.Add(key, prefab);
        }

        // �̸� maxCount��ŭ �������� ��Ȱ��ȭ �ϰ�ʹ�.
        // ��Ͽ� ��Ƴ���ʹ�.
        for (int i = 0; i < maxCount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.parent = parent;
            obj.name = key;
            obj.SetActive(false);
            // ���� list�� key�� �������� �ʴ´ٸ� 
            if (false == list.ContainsKey(key))
            {
                // key�� value�� �߰��ϰ�ʹ�.
                list.Add(key, new List<GameObject>());
                inActiveList.Add(key, new List<GameObject>());
            }

            list[key].Add(obj);
            inActiveList[key].Add(obj);
        }
    }
    public List<GameObject> GetAllInactiveObjects(string key)
    {
        List<GameObject> activatedObjects = new List<GameObject>();

        if (!inActiveList.ContainsKey(key))
        {
            return activatedObjects;
        }

        // ��Ȱ�� ������ ��� ��ü�� Ȱ��ȭ�ϱ�
        foreach (GameObject obj in inActiveList[key])
        {
            obj.SetActive(true);
            activatedObjects.Add(obj);
        }

        // ��� ��ü�� Ȱ��ȭ�� �Ŀ��� ��Ȱ��ȭ ����Ʈ�� ����ݴϴ�.
        inActiveList[key].Clear();

        return activatedObjects;
    }

    /// <summary>
    /// �ش� key�� ��Ȱ�� ��ü�� �ϳ� ����ʹ�.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public GameObject GetInactiveObject(string key)
    {
        if (false == inActiveList.ContainsKey(key))
        {
            return null;
        }
        // ���� ��Ȱ������� 0�� ���� ũ�ٸ�
        if (inActiveList[key].Count > 0)
        {
            
            inActiveList[key][0].SetActive(true);
            GameObject temp = inActiveList[key][0];
            //  ��Ȱ����Ͽ��� �����ϰ�
            inActiveList[key].RemoveAt(0);
            //  ��ȯ�ϰ�ʹ�.
            return temp;
        }
        // �׷����ʴٸ�(���� ��Ȱ������� 0�����)        //  null�� ��ȯ�ϰ�ʹ�.

        GameObject prefab = dicPrefabs[key];
        GameObject obj = Instantiate(prefab);
        obj.transform.parent = list[key][0].transform.parent;
        obj.name = key;
        list[key].Add(obj);
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// �� ����� ��ü�� ObjectPool�� ��ȯ�ϰ�ʹ�.
    /// </summary>
    /// <param name="obj"></param>
    public void AddInactiveObject(GameObject obj)
    {
        obj.SetActive(false);
        string key = obj.name;
        if (inActiveList.ContainsKey(key))
        {
            if (false == inActiveList[key].Contains(obj))
            {
                inActiveList[key].Add(obj);
            }
        }
    }

    /// <summary>
    /// �ش� ��ü��(obj) ObjectPool���� �����Ǵ� �༮���� �˰�ʹ�.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsObjectPoolObject(GameObject obj)
    {
        string key = obj.name;
        if (Instance.list.ContainsKey(key))
        {
            return Instance.list[key].Contains(obj);
        }

        return false;
    }


}