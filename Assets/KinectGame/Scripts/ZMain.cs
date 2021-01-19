using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MessageManager.Instance.InitializeMessage();
        MessageManager.Instance.SendConnectServerMsg("192.168.68.187", "443");
#if UNITY_EDITOR
        GameManager.Instance.Init();
#else
        MessageManager.Instance.JoinRoomSuccessEvent += GameManager.Instance.Init;
#endif

        ZCoroutiner.SetCoroutiner(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
