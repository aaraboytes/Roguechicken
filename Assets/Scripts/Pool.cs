using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bucket
{
    public int amount;
    public GameObject gameObject;
}
public class Pool : MonoBehaviour
{
    public static Pool Instance;
    public List<Bucket> buckets;

    Dictionary<GameObject, Queue<GameObject>> pool = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        foreach (Bucket bucket in buckets)
        {
            GameObject b = new GameObject(bucket.gameObject.name + "_bucket");
            Queue<GameObject> elements = new Queue<GameObject>();
            for (int i = 0; i < bucket.amount; i++)
            {
                GameObject element = Instantiate(bucket.gameObject, b.transform);
                elements.Enqueue(element);
                element.SetActive(false);
            }
            pool.Add(bucket.gameObject, elements);
        }
    }

    public GameObject Recycle(GameObject pooledObj, Vector3 position, Quaternion rotation)
    {
        Queue<GameObject> bucket = new Queue<GameObject>();
        if (pool.TryGetValue(pooledObj, out bucket))
        {
            GameObject element = bucket.Dequeue();
            bucket.Enqueue(element);
            element.transform.position = position;
            element.transform.rotation = rotation;
            if (element.GetComponent<Rigidbody>())
                element.GetComponent<Rigidbody>().velocity = Vector3.zero;
            element.SetActive(true);
            if (element.GetComponent<Rigidbody2D>())
                element.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return element;
        }
        else
        {
            Debug.LogError("There is not bucket in the pool with this object");
            return null;
        }
    }
}