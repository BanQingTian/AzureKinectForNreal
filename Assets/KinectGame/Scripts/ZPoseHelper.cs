using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ZPose
{
    public void Set(int c)
    {
        count = c;
        position = new Vector3[c];
        rotation = new Quaternion[c];
    }
    public int count;
    public Vector3[] position;
    public Quaternion[] rotation;
}
public class ZPoseHelper : MonoBehaviour
{
    private bool m_Initialized = false;
    protected Transform RootNode;
    public List<GameObject> ModelNodes;


    public ZPose PoseData;

    #region Unity_Internal

    private void Start()
    {
        Init();
    }



    #endregion

    public void Init()
    {
        if (m_Initialized) return;

        RootNode = transform;

        InitAllNodes(RootNode);

        PoseData.Set(ModelNodes.Count);
    }

    private void InitAllNodes(Transform trans)
    {
        foreach (Transform item in trans)
        {
            if (item != null)
            {
                ModelNodes.Add(item.gameObject);
                InitAllNodes(item);
            }
        }
    }

    public void UpdataPose(ZPose p)
    {
        for (int i = 0; i < p.count; i++)
        {
            ModelNodes[i].transform.localPosition = p.position[i];
            ModelNodes[i].transform.localRotation = p.rotation[i];
        }
    }

    public void FreshPoseData()
    {
        for (int i = 0; i < PoseData.count; i++)
        {
            ModelNodes[i].transform.localPosition = ModelNodes[i].transform.localPosition;
            ModelNodes[i].transform.localRotation = ModelNodes[i].transform.localRotation;
        }
    }
}
