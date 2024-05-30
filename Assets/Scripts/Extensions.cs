using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// encoding
using System.Text;
using System.Runtime.InteropServices;
using System;

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
                UnityEngine.Object targetObject = target as UnityEngine.Object;
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

    public static byte[] ToByteArray_UTF8(this string target)
    {
        return Encoding.UTF8.GetBytes(target);
    }

    public static string ToString_UTF8(this byte[] target)
    {
        return Encoding.UTF8.GetString(target);
    }

    public static byte[] ToByteArray<T>(this T target) where T : struct
    {
        // C# 내장함수를 강제로 끄집어 내
        int size = Marshal.SizeOf(typeof(T));
        byte[] result = new byte[size];

        // 메모리중 하나를 카리키는 포인터 : 여기다가 저장
        // struct를 byte로 바꾸기 위해 저장할 공간을 할당
        IntPtr ptr = Marshal.AllocHGlobal(size);
        // 원본 구조체는 어디에 있나 찾아 ptr에 넣기
        Marshal.StructureToPtr(target, ptr, false);
        // 복사해서 배열에 넣기
        Marshal.Copy(ptr, result, 0, size);
        // 메모리 해제
        Marshal.FreeHGlobal(ptr);
        return result;
    }

    public static T ToStruct<T>(this byte[] target) where T : struct
    {
        // 스트럭트로 바꿔서 형변환 해주는 함수
        return (T)target.ToStruct(typeof(T));

    }

    public static object ToStruct(this byte[] target, System.Type type)
    {
        int size = Marshal.SizeOf(type);
        // 예외처리
        if (size != target.Length)
        {
            throw new InvalidCastException("잘못된 바이트 변환 시도");
        }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(target, 0, ptr, size);
        object result = Marshal.PtrToStructure(ptr, type);
        Marshal.FreeHGlobal(ptr);
        return result;

    }

    // 벡터 회전
    public static Vector3 GetHorizontalRotate(this Vector3 target, float angle)
    {
        // 벡터의 회전 공식
        // 벡터 변환 행렬    ┌cosA -sinA┐┌x┐
        //                  └sinA  cosA┘└z┘
        // ┌cosA    0   -sinA┐┌x┐
        // │0       1       0││y│
        // └sinA    0    cosA┘└z┘
        /// x' = xCosA - zSinA
        /// z' = xSinA + zCosA
        float theta = angle * Mathf.Deg2Rad;
        return new Vector3(
            target.x * Mathf.Cos(theta) - target.z * Mathf.Sin(theta),
            target.y,
            target.x * Mathf.Sin(theta) + target.z * Mathf.Cos(theta)
        );
    }

    public static Vector3 GetVerticalRotate(this Vector3 target, float angle)
    {
        float theta = angle * Mathf.Deg2Rad;

        float horizontalLength = Mathf.Sqrt(target.x * target.x + target.z * target.z);
        return new Vector3
        (
        // ┌cosA -sinA┐┌ y ┐
        // └sinA  cosA┘└x,z┘
        /// y' = y * CosA - Sqrt(x^2+z^2) * SinA
        /// (x' : z=0, z' : x=0)  = y * SinA + Sqrt(x^2+z^2) * CosA
            target.y * Mathf.Sin(theta) + target.x * Mathf.Cos(theta),
            target.y * Mathf.Cos(theta) - horizontalLength * Mathf.Sin(theta),
            target.y * Mathf.Sin(theta) + target.z * Mathf.Cos(theta)
        );

    }

    // 수평각도를 전달 -> 벡터 반환
    public static Vector3 GetAngledVector(this float horizontalAngle)
    {
        float theta = horizontalAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta));
    }

    public static Vector3 GetAngledVector(this float horizontalAngle, float verticalAngle)
    {
        float horizontalTheta = horizontalAngle * Mathf.Deg2Rad;
        float verticalTheta = verticalAngle * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Sin(horizontalTheta) * Mathf.Abs(Mathf.Cos(verticalTheta)),
            Mathf.Sin(horizontalTheta), 
            Mathf.Cos(horizontalTheta) * Mathf.Abs(Mathf.Cos(verticalTheta))
        );
    }

    public static float GetHorizontalAngle(this Vector3 vector)
    {
        // 수평 : x, z
        // atan : 이미 만들어진 원에서 원본 라디안 추출
        return Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg;
    }
    
    public static float GetVerticalAngle(this Vector3 vector)
    {
        // 수평 : y x,z
        return Mathf.Atan2(Mathf.Sqrt(vector.z * vector.z + vector.x * vector.x), vector.y) * Mathf.Rad2Deg;
    }


}
