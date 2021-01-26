using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZEnum
{


}
public enum GameMode
{
    Prepare,
    Drum,
    //Football,
}

/// <summary>
/// 角色类型
/// </summary>
public enum PlayerRoleModel
{
    BlackGirl,
    CodeMan,
}


public enum CollisionTypeEnum
{
    Foot,
    Hand,
    Hip,
}


public enum BarrierTypeEnum
{
    Barrier,
    Icon,
    LeftHand,
    RightHand,
    NeedDestroy,
    CanPickUp,
}

public enum BarrierModelEnum
{
    /// <summary>
    /// 正常障碍物模型
    /// </summary>
    Normal1,
    Normal2,
    /// <summary>
    /// 手类障碍物模型
    /// </summary>
    LeftHand,
    RightHand,
    /// <summary>
    /// 金币模型
    /// </summary>
    Icon1,
    Icon2,
    /// <summary>
    /// 可拾取障碍物
    /// </summary>
    PickUp1,
    pickUp2,
    /// <summary>
    /// 需要破坏障碍物
    /// </summary>
    NeedDestroy1,
    NeedDestroy2,
}

public enum SpecialEffectEnum
{
    SpecialEffect1,
    SpecialEffect2,
    SpecialEffect3,
    SpecialEffect4,
}

public enum SoundEffEnum
{
    SoundEff1,
    SoundEff2,
    SoundEff3,
    SoundEff4,
}

