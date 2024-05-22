using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions
{
    // �̱���
    // "���� ��" ��� �� -> �ڿ� �� ��� �ı�
    // "�����̵���" : � �ڷ����̵� ���� �� �־�� �Ѵ�.
    // int��, float��, monobehaviour��...
    // ������ �Ȱ��� ������ �ϴ� �Լ���� �ڷ����� �ٲٸ� �ȴ�. : <Type>�̶�� ���
    // GetComponent<T>()
    //                                                                ���� ������ ���� ������ ���.
    public static WantType MakeSingleton<WantType>(this WantType target, ref WantType location, bool destroyObject = true)
    {
        // ���� ���� ������ : ����
        if(location == null)
        {
            location = target;
        }
        // ���� �̹� ���ִ� : �ı�
        else
        {
            //                    Ÿ���� ������Ʈ�� ��ó�� ���
            //                    ������Ʈ �ƴϸ� -> null ��ȯ
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
