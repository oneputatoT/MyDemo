using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGround : MonoBehaviour
{
    [SerializeField]
    Sprite[] temp;

    private void Start()
    {
        int n = Random.Range(0, temp.Length);
        GetComponent<Image>().sprite = temp[n];
    }
}
