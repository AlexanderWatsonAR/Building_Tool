//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.ProBuilder;

//public class District : MonoBehaviour
//{
//    public GameObject Pillar;
//    public GameObject WallOutline;
//    public Material Wood;

//    public void Build()
//    {
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            if (transform.GetChild(i).GetComponent<PolyShape>() != null)
//            {
//                Building house;
//                if (transform.GetChild(i).GetComponent<Building>() == null)
//                {
//                    house = transform.GetChild(i).AddComponent<Building>();
//                    house.SetBuildingMaterials(Pillar, WallOutline, Wood);
//                    house.Build();

//                }
//                else
//                {
//                    house = transform.GetChild(i).GetComponent<Building>();
//                    house.SetBuildingMaterials(Pillar, WallOutline, Wood); // To be Removed
//                    house.Build();
//                }
//            }
//        }
//    }

//    public void RevertToPolyshape()
//    {
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            if (transform.GetChild(i).GetComponent<PolyShape>() != null)
//            {
//                Building house = transform.GetChild(i).GetComponent<Building>();
//                house.RevertToPolyshape();
//            }
//        }
//    }

//}
