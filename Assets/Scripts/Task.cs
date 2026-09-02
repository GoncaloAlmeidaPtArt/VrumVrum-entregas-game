using System;
using UnityEngine;

/// <summary>
/// Dados de uma task de entrega.
/// Pode ser preenchida manualmente na lista do TaskManager, ou você pode
/// converter isso num ScriptableObject depois se quiser criar tasks como assets.
/// </summary>
[Serializable]
public class Task
{
    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private GameObject deliveryPoint;
    [SerializeField] private int reward;
    
    public GameObject GetDeliveryPoint => deliveryPoint;
    public Task(string title, string description, GameObject deliveryPoint, int reward)
    {
        this.title = title;
        this.description = description;
        this.deliveryPoint = deliveryPoint;
        this.reward = reward;
    }
}