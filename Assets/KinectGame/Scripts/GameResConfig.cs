using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// tmp , 资源配表
/// </summary>
public class GameResConfig : MonoBehaviour
{
    public static GameResConfig Instance;
    /// <summary>
    /// 下标对应特效类型顺序
    /// </summary>
    public List<GameObject> SpecialEffList = new List<GameObject>();

    //public Dictionary<BarrierTypeEnum, GameObject> VFXDic = new Dictionary<BarrierTypeEnum, GameObject>();

    /// <summary>
    /// 下标对应音频顺序
    /// </summary>
    public List<AudioClip> AudioClipList = new List<AudioClip>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 获取特效预制体
    /// </summary>
    public GameObject GetSpecialEff(SpecialEffectEnum se, bool destroySelf = true, float destroyTime = 2f)
    {
        GameObject go = PoolManager.Instance.Get(SpecialEffList[(int)se]);
        if (destroySelf)
        {
            StartCoroutine(destroySelfCor(go, destroyTime));
        }
        return go;
    }

    private IEnumerator destroySelfCor(GameObject g, float time)
    {
        yield return new WaitForSeconds(time);
        PoolManager.Instance.Release(g);
    }

    /// <summary>
    /// 获取音频预制体
    /// </summary>
    public AudioClip GetAudioEff(SoundEffEnum se)
    {
        return AudioClipList[(int)se];
    }
}
