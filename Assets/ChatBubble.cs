using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    public TMPro.TextMeshPro text;
    public SpriteRenderer background;

    // Start is called before the first frame update
    void Start()
    {
        background = transform.Find("background").GetComponent<SpriteRenderer>();
        text = transform.Find("text").GetComponent<TMPro.TextMeshPro>();

        SetBubble("Hello world how are you");
    }

    void SetBubble(string bubbleText)
    {
        text.SetText(bubbleText);
        text.ForceMeshUpdate();
        Vector2 textSize = text.GetRenderedValues(false);
        Vector2 padding = new Vector2(4.0f, 4.0f);
        background.size = textSize + padding;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // Align floating text perpendicular to Camera.
        if (!lastPOS.Compare(m_cameraTransform.position, 1000) || !lastRotation.Compare(m_cameraTransform.rotation, 1000))
        {
            lastPOS = m_cameraTransform.position;
            lastRotation = m_cameraTransform.rotation;
            m_floatingText_Transform.rotation = lastRotation;
            Vector3 dir = m_transform.position - lastPOS;
            m_transform.forward = new Vector3(dir.x, 0, dir.z);
        }
        */
    }
}
