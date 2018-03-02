using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour {

    private EcoListController list;
    private List<string> data = new List<string>();

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 50; i++)
            data.Add(string.Format("Item{0}", i));
        list = transform.Find("group").GetComponent<EcoListController>();
        list.Init(data.Count, OnRenderItem);
	}

    void OnRenderItem(RectTransform trans, int dataIndex) 
    {
        trans.Find("Text").GetComponent<UnityEngine.UI.Text>().text = data[dataIndex];
    }
}
