﻿namespace GenerativeCS.Interfaces;

public interface ICompletionProvider<TConversation, TMessage, TFunction>
    where TConversation : IChatConversation<TMessage, TFunction>
    where TMessage : IChatMessage, new()
    where TFunction : IChatFunction, new()
{
    ICollection<TFunction> Functions { get; }

    Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default);

    Task<string> CompleteAsync(TConversation conversation, CancellationToken cancellationToken = default);

    void AddFunction(TFunction function)
    {
        Functions.Add(function);
    }

    void AddFunction(Delegate function)
    {
        var chatFunction = new TFunction
        {
            Name = function.Method.Name,
            Function = function
        };

        Functions.Add(chatFunction);
    }

    void AddFunction(string name, Delegate function)
    {
        var chatFunction = new TFunction
        {
            Name = name,
            Function = function
        };

        Functions.Add(chatFunction);
    }

    void AddFunction(string name, string? description, Delegate function)
    {
        var chatFunction = new TFunction
        {
            Name = name,
            Description = description,
            Function = function
        };

        Functions.Add(chatFunction);
    }

    public void AddFunction(string name, bool requireConfirmation, Delegate function)
    {
        var chatFunction = new TFunction
        {
            Name = name,
            RequireConfirmation = requireConfirmation,
            Function = function
        };

        Functions.Add(chatFunction);
    }

    public void AddFunction(string name, string? description, bool requireConfirmation, Delegate function)
    {
        var chatFunction = new TFunction
        {
            Name = name,
            Description = description,
            RequireConfirmation = requireConfirmation,
            Function = function
        };

        Functions.Add(chatFunction);
    }

    void RemoveFunction(TFunction function)
    {
        Functions.Remove(function);
    }

    void RemoveFunction(string name)
    {
        var functionToRemove = Functions.LastOrDefault(f => f.Name == name);
        if (functionToRemove != null)
        {
            Functions.Remove(functionToRemove);
        }
    }

    void RemoveFunction(Delegate function)
    {
        var functionToRemove = Functions.LastOrDefault(f => f.Function == function);
        if (functionToRemove != null)
        {
            Functions.Remove(functionToRemove);
        }
    }

    void ClearFunctions()
    {
        Functions.Clear();
    }
}
