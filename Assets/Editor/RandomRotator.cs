using UnityEditor;
using UnityEngine;

public class RandomRotator : Editor
{
    [MenuItem("Tools/Random Rotation/All Axes")]
    static void ApplyRandomRotationAll()
    {
        Apply(go => go.transform.rotation = Random.rotation);
    }

    [MenuItem("Tools/Random Rotation/Y Only")]
    static void ApplyRandomRotationY()
    {
        Apply(go => go.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0));
    }

    [MenuItem("Tools/Random Rotation/X Only")]
    static void ApplyRandomRotationX()
    {
        Apply(go => go.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), 0, 0));
    }

    [MenuItem("Tools/Random Rotation/Z Only")]
    static void ApplyRandomRotationZ()
    {
        Apply(go => go.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
    }

    static void Apply(System.Action<GameObject> action)
    {
        foreach (var go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, "Random Rotation");
            action(go);
        }
    }
}