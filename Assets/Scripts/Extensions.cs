using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions
{
    // 싱글턴
    // "먼저 온" 사람 승 -> 뒤에 온 사람 파괴
    // "무엇이든지" : 어떤 자료형이든 들어올 수 있어야 한다.
    // int용, float용, monobehaviour용...
    // 어차피 똑같은 역할을 하는 함수라면 자료형만 바꾸면 된다. : <Type>이라고 명시
    // GetComponent<T>()
    //                                                                제일 먼저온 놈을 저장해 논다.
    public static WantType MakeSingleton<WantType>(this WantType target, ref WantType location, bool destroyObject = true)
    {
        // 제일 먼저 왔으면 : 저장
        if(location == null)
        {
            location = target;
        }
        // 누가 이미 와있다 : 파괴
        else
        {
            //                    타겟을 오브젝트인 것처럼 취급
            //                    오브젝트 아니면 -> null 반환
            Component targetComponent = target as Component;

            if(targetComponent)
            {
                GameObject.Destroy(destroyObject ? targetComponent.gameObject : targetComponent);

            }
            else
            {
                Object targetObject = target as Object;
                if (targetObject)
                {
                    GameObject.Destroy(targetObject);
                }
            }
        }
        return location;
    }

    public static string GetFileName(this string path)
    {
        if (path == null || path.Length == 0) return "";
        string newPath = "";
        bool findSpace = false;
        for(int i=0; i<path.Length; i++)
        {
            if (path[i] != ' ')
            {
                if(findSpace)
                {
                    newPath += char.ToUpper(path[i]);
                    findSpace= false;
                }
                else
                {
                    newPath += path[i];

                }

            }
            else
            {
                    findSpace= true;

            }
        }

        // string[] splited = newPath.Split("/");
        // return splited[splited.Length - 1];

        // return newPath.Substring(path.LastIndexOf("/") + 1);

        return newPath[(path.LastIndexOf("/") + 1)..];

    }
}
