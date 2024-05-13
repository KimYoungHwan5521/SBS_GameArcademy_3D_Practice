using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : Manager
{
    OptionManager.OptionData loadedOptionData;
    public OptionManager.OptionData LoadedOptionData => loadedOptionData;

    public override IEnumerator Initiate()
    {
        Save<OptionManager.OptionData>($"{Application.persistentDataPath}/SaveData", $"{Application.persistentDataPath}/SaveData/OptionData.json", OptionManager.defaultOptionData);
        loadedOptionData = Load<OptionManager.OptionData>($"{Application.persistentDataPath}/SaveData/OptionData.json", ref OptionManager.defaultOptionData);

        yield return null;
    }

    // 무슨 자료형이든 로드할 수 있는 함수
    // 세이브와 로드를 제네릭 함수로 만들어주시고, 원하는 변수에 넣어주기
    // 성공/실패도 같이 넣기
    public bool Save<T>(string folderPath, string filePath, T data)
    {
        // 1. 데이터가 null 인지 확인
        if (data == null) return false;
        // 2. 이미 있는지 확인
        // 폴더가 있어야 파일이 있다.
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        if (!File.Exists(filePath))
        {
            // 3. 파일을 만듦
            // 파일 쓰고있는 상태를 받아온것 -> 다시 닫아야됨
            File.Create(filePath).Close();
        }
        // 4. 데이터를 JSON으로 변환
        string jsonData = JsonUtility.ToJson(data);
        // 5. 변환한 데이터를 파일에 넣어줌
        File.WriteAllText(filePath, jsonData);
        return true;
    }

    public T Load<T>(string filePath, ref T defaultData)
    {
        /*
         * 파일 검사할 때 어차피 확인되므로 생략가능
        // 1. 폴더가 있는지 확인
        if(!Directory.Exists(folderPath))
        {
            // 2. 폴더가 없으면 null값을 리턴하고 종료
            return default;
        }
         */
        // 3. 파일이 있는지 확인
        if(!File.Exists(filePath))
        {
            // 4. 파일이 없으면 null값을 리턴하고 종료
            Debug.LogWarning($"Data Loading Fails : \'{filePath}\' Not Found");
            return defaultData;
        }
        // 5. 데이터를 저장할 data를 선언
        T data;
        // 6. 파일의 데이터를 전부 가져옴 ReadAllText
        string jsonData = File.ReadAllText(filePath);
        // 7. 받은 json 데이터를 data에 파싱
        data = JsonUtility.FromJson<T>(jsonData);
        // 8. data를 반환
        return data;
    }

}
