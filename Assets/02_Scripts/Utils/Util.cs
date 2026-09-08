using UnityEngine;

public class Util
{
    // 자식 컴퍼넌트 찾기
    public static T FindChild<T>(GameObject go, string name, bool includeInactive) where T : Component
    {
        foreach (T child in go.GetComponentsInChildren<T>(includeInactive))
        {
            if (child.name == name) 
                return child;
        }

        return null;
    }
}
