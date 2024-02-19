using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyholeStateKeyIsOut : IState
{
    private readonly KeyholeSocket m_keyhole;

    public KeyholeStateKeyIsOut(KeyholeSocket keyhole)
    {
        m_keyhole = keyhole;
    }
    public void Enter()
    {
        m_keyhole.SetKeyInKeyhole(false);


    }
    public void Process()
    {

    }
    public void Exit()
    {

    }
}
