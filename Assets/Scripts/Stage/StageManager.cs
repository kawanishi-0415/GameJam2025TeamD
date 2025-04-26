using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public enum EnumStageStatus
    {
        Init,
        Playing,
        GameOver,
        GameClear,
    }
    private EnumStageStatus m_status = EnumStageStatus.Init;

    private void Start()
    {
        
    }

    private IEnumerator CoPlay()
    {
        switch (m_status)
        {
            case EnumStageStatus.Init:
                yield return AsyncInit();
                break;
            case EnumStageStatus.Playing:
                break;
            case EnumStageStatus.GameOver:
                break;
            case EnumStageStatus.GameClear:
                break;
        }
        yield return null;
    }

    private IEnumerator AsyncInit()
    {
        yield return null;
    }
}
