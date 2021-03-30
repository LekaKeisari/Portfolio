using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextCollection", menuName = "ScriptableObject/TextCollection", order = 2)]
public class TextCollectionSO : ScriptableObject
{
    [SerializeField] private string task1Text;
    public string Task1Text { get => task1Text; }
    [SerializeField] private string task2Text;
    public string Task2Text { get => task2Text; }
    [SerializeField] private string task3Text;
    public string Task3Text { get => task3Text; }
}
