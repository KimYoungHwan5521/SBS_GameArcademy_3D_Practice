using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MonoBehaviour
{
    // ���� ������
    ResourceEnum.Prefab origin;
    public ResourceEnum.Prefab Origin => origin;

    public void SetInfo(ResourceEnum.Prefab wantOrigin)
    {
        origin= wantOrigin;
    }

}
