using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFieldData : MonoBehaviour
{
    private bool unlocked;
    private GameObject go;
    private string unlockedInfo = "Research to unlock";

    public GameObject Prefab;





    // Start is called before the first frame update
    void Start()
    {
        GlobalMessageBus.Instance.Subscribe(this);
        gameObject.name = Prefab.name;
        GetComponent<UnityEngine.UI.Text>().text = Prefab.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Spawn()
    {
        return Instantiate<GameObject>(Prefab);
        
    }
}
