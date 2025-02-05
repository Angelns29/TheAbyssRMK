using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableDialogues", menuName = "Scriptable Objects/ScriptableDialogues")]
public class ScriptableDialogues : ScriptableObject
{
    [TextArea(3, 10)]
    public string textIngles;
    [TextArea(3, 10)]
    public string textEspanol;
    [TextArea(3, 10)]
    public string textCatalan;
}
