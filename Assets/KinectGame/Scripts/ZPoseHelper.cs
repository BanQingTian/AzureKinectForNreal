using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ZPose
{
    public void Set(int c,PlayerRoleModel pm)
    {
        role = pm;
        count = c;
        position = new Vector3[c];
        rotation = new Quaternion[c];
    }
    public int count;
    public Vector3[] position;
    public Quaternion[] rotation;
    public PlayerRoleModel role;
}
public class ZPoseHelper : MonoBehaviour
{
    private bool m_Initialized = false;
    protected Transform RootNode;
    public List<GameObject> ModelNodes;


    public ZPose PoseData;


    public void Init(PlayerRoleModel pm)
    {
        RootNode = transform;

        ModelNodes.Clear();

        InitAllNodes(RootNode);
        Debug.Log(ModelNodes.Count);
        PoseData.Set(ModelNodes.Count,pm);
    }

    private void InitAllNodes(Transform trans)
    {
        foreach (Transform item in trans)
        {
            if (item != null && item.gameObject.activeInHierarchy)
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
            if (i >= ModelNodes.Count || ModelNodes[i] == null)
            {
                //ModelNodes.Clear();
                InitAllNodes(RootNode);
                return;
            }
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
