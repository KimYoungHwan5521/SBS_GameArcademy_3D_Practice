using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingInfo : MonoBehaviour
{
    // ¿øº» ÇÁ¸®ÆÕ
    ResourceEnum.Prefab origin;
    public ResourceEnum.Prefab Origin => origin;

    public void SetInfo(ResourceEnum.Prefab wantOrigin)
    {
        origin= wantOrigin;
    }

}
