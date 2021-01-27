using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPart : MonoBehaviour
{
    public GameObject Part1;
    public List<GameObject> Part1List;
    public GameObject Part2;
    public List<GameObject> Part2List;
    public GameObject Part3;
    public List<GameObject> Part3List;

    private void OnEnable()
    {
        foreach (Transform item in Part1.transform)
        {
            Part1List.Add(item.gameObject);
        }

        foreach (Transform item in Part2.transform)
        {
            Part2List.Add(item.gameObject);
        }

        foreach (Transform item in Part3.transform)
        {
            Part3List.Add(item.gameObject);
        }
    }

    #region Load From Animtion Event

    public void ResetPart1()
    {
        Part1.SetActive(true);
        for (int i = 0; i < Part1List.Count; i++)
        {
            Part1List[i].SetActive(true);
        }
        Part2.SetActive(false);
        Part3.SetActive(false);
    }

    public void ResetPart2()
    {
        Part2.SetActive(true);
        for (int i = 0; i < Part2List.Count; i++)
        {
            Part2List[i].SetActive(true);
        }
    }

    public void ResetPart3()
    {
        Part3.SetActive(true);
        for (int i = 0; i < Part3List.Count; i++)
        {
            Part3List[i].SetActive(true);
        }
    }

    public void HidePart1()
    {
        Part1.SetActive(false);
    }

    public void HidePart2()
    {
        Part2.SetActive(false);
    }

    public void Inverse()
    {
        //Part1.transform.localScale = new Vector3(-1, 1, 1);
        //Part2.transform.localScale = new Vector3(-1, 1, 1);
        //Part3.transform.localScale = new Vector3(-1, 1, 1);
    }

    #endregion
}
