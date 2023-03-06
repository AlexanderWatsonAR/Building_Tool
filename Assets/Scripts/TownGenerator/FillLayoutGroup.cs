using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoLayout3D;

public class FillLayoutGroup : MonoBehaviour
{
    // Prefabs attached with the layout element script.
    public LayoutElement3D[] layoutElements;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void FillGroup()
    {
        XAxisLayoutGroup3D xAxisLayoutGroup = GetComponent<XAxisLayoutGroup3D>();

        float xSize = xAxisLayoutGroup.size.x;

        while (xSize >= 0.1f)
        {
            GameObject shelfObject = Instantiate(layoutElements[Random.Range(0, layoutElements.Length)].gameObject);
            float shelfObjectXSize = shelfObject.GetComponent<LayoutElement3D>().size.x;

            if (xSize - shelfObjectXSize > 0)
            {
                shelfObject.transform.SetParent(transform, false);
                xSize -= shelfObjectXSize;
            }
            else
            {
                Destroy(shelfObject);
            }
        }
    }
}
