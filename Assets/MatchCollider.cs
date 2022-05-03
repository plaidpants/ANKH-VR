using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.01f);
    }
}
