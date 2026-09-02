using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gerencia o ciclo de vida das tasks: quais estão disponíveis, qual está ativa,
/// se o jogador pode pegar outra, e quantas já foram concluídas no total.
///
/// Fluxo esperado:
/// 1) Carro entra no trigger -> chama TaskManager.Instance.IsAvailableForNewTask()
/// 2) Se true, o menu chama TaskManager.Instance.GetRandomTaskOptions(n) pra mostrar opções
/// 3) Jogador escolhe uma -> menu chama TaskManager.Instance.AcceptTask(task)
/// 4) Isso dispara OnTaskAccepted, que quem cria o ponto de entrega no mapa deve escutar
/// 5) Quando o carro chega no ponto de entrega, esse trigger chama
///    TaskManager.Instance.CompleteCurrentTask()
/// </summary>
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("Configuração")]
    [Tooltip("Todas as tasks possíveis do jogo. O gerador sorteia a partir daqui.")]
    [SerializeField] private List<Task> allPossibleTasks = new List<Task>();

    [Tooltip("Quantas opções de task aparecem no menu ao ativar o trigger.")]
    [SerializeField] private int optionsPerMenu = 3;

    [Header("Estado atual (somente leitura, útil pra debug no Inspector)")]
    [SerializeField] private Task currentTask;
    [SerializeField] private bool hasActiveTask = false;
    [SerializeField] private int tasksCompleted = 0;

    // ---------------- Eventos ----------------
    // Quem cuida da UI e do spawn do ponto de entrega deve se inscrever nesses eventos.

    /// <summary>Disparado quando o menu deve ser aberto, com as opções sorteadas.</summary>
    public event Action<List<Task>> OnTaskOptionsGenerated;

    /// <summary>Disparado quando uma task é aceita. Quem cria o ponto no mapa escuta aqui.</summary>
    public event Action<Task> OnTaskAccepted;

    /// <summary>Disparado quando a task ativa é concluída com sucesso.</summary>
    public event Action<Task> OnTaskCompleted;

    /// <summary>Disparado quando uma task ativa é cancelada/abandonada.</summary>
    public event Action<Task> OnTaskCancelled;

    /// <summary>Disparado sempre que o contador total de tasks concluídas muda.</summary>
    public event Action<int> OnTasksCompletedCountChanged;

    private void Awake()
    {
        // Singleton simples. Se você já tem seu próprio padrão de singleton no projeto,
        // pode remover isso e só deixar essa classe como um serviço normal.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ---------------- Consultas de estado ----------------

    /// <summary>
    /// O carro só pode pegar uma task nova se não tiver nenhuma ativa no momento.
    /// Chame isso no OnTriggerEnter antes de abrir o menu.
    /// </summary>
    public bool IsAvailableForNewTask()
    {
        return !hasActiveTask;
    }

    public bool HasActiveTask => hasActiveTask;
    public Task CurrentTask => currentTask;
    public int TasksCompletedCount => tasksCompleted;

    // ---------------- Geração de opções ----------------

    /// <summary>
    /// Sorteia N tasks distintas da lista de possíveis, para exibir no menu.
    /// Não marca nenhuma como ativa ainda — isso só acontece em AcceptTask().
    /// </summary>
    public List<Task> GetRandomTaskOptions(int count = -1)
    {
        if (hasActiveTask)
        {
            Debug.LogWarning("[TaskManager] Já existe uma task ativa. Conclua ou cancele antes de gerar novas opções.");
            return new List<Task>();
        }

        int howMany = count > 0 ? count : optionsPerMenu;
        howMany = Mathf.Min(howMany, allPossibleTasks.Count);

        List<Task> options = allPossibleTasks
            .OrderBy(_ => UnityEngine.Random.value)
            .Take(howMany)
            .ToList();

        OnTaskOptionsGenerated?.Invoke(options);
        return options;
    }

    // ---------------- Aceitar / concluir / cancelar ----------------

    /// <summary>
    /// Chamado quando o jogador escolhe uma task no menu.
    /// Marca essa task como ativa e dispara o evento pra criar o ponto de entrega no mapa.
    /// </summary>
    public bool AcceptTask(Task task)
    {
        if (hasActiveTask)
        {
            Debug.LogWarning("[TaskManager] Não é possível aceitar: já existe uma task em andamento.");
            return false;
        }

        currentTask = task;
        hasActiveTask = true;

        OnTaskAccepted?.Invoke(currentTask);
        return true;
    }

    /// <summary>
    /// Chamado pelo trigger do ponto de entrega quando o carro chega lá.
    /// </summary>
    public void CompleteCurrentTask()
    {
        if (!hasActiveTask || currentTask == null)
        {
            Debug.LogWarning("[TaskManager] Nenhuma task ativa para concluir.");
            return;
        }

        Task finished = currentTask;

        tasksCompleted++;
        currentTask = null;
        hasActiveTask = false;

        OnTaskCompleted?.Invoke(finished);
        OnTasksCompletedCountChanged?.Invoke(tasksCompleted);
    }

    /// <summary>
    /// Caso você queira permitir desistir de uma task no meio do caminho.
    /// Libera o jogador para pegar outra, sem contar como concluída.
    /// </summary>
    public void CancelCurrentTask()
    {
        if (!hasActiveTask || currentTask == null) return;

        Task cancelled = currentTask;
        currentTask = null;
        hasActiveTask = false;

        OnTaskCancelled?.Invoke(cancelled);
    }

    // ---------------- Utilitário pra popular a lista via código ----------------

    /// <summary>
    /// Permite adicionar tasks dinamicamente (ex: geradas por outro sistema),
    /// em vez de preencher tudo pelo Inspector.
    /// </summary>
    public void RegisterTask(Task task)
    {
        allPossibleTasks.Add(task);
    }
}