﻿using GenerativeCS.Events;
using GenerativeCS.Interfaces;

namespace GenerativeCS.Models;

public record ChatConversation<TMessage, TFunction> : IChatConversation<TMessage, TFunction> where TMessage : IChatMessage, new() where TFunction : IChatFunction, new()
{
    public ChatConversation() { }

    public ChatConversation(string systemMessage)
    {
        Messages.Add(IChatMessage.FromSystem<TMessage>(systemMessage));
    }

    public event EventHandler<MessageAddedEventArgs<TMessage>>? MessageAdded;

    public ICollection<TMessage> Messages { get; set; } = new List<TMessage>();

    public ICollection<TFunction> Functions { get; set; } = new List<TFunction>();

    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;

    public void FromSystem(string message)
    {
        var chatMessage = IChatMessage.FromSystem<TMessage>(message);

        Messages.Add(chatMessage);
        MessageAdded?.Invoke(this, new MessageAddedEventArgs<TMessage>(chatMessage));
    }

    public void FromUser(string message)
    {
        var chatMessage = IChatMessage.FromUser<TMessage>(message);

        Messages.Add(chatMessage);
        MessageAdded?.Invoke(this, new MessageAddedEventArgs<TMessage>(chatMessage));
    }

    public void FromUser(string name, string message)
    {
        var chatMessage = IChatMessage.FromUser<TMessage>(name, message);

        Messages.Add(chatMessage);
        MessageAdded?.Invoke(this, new MessageAddedEventArgs<TMessage>(chatMessage));
    }

    public void FromAssistant(string message)
    {
        var chatMessage = IChatMessage.FromAssistant<TMessage>(message);

        Messages.Add(chatMessage);
        MessageAdded?.Invoke(this, new MessageAddedEventArgs<TMessage>(chatMessage));
    }

    public void FromAssistant(IFunctionCall functionCall)
    {
        var chatMessage = IChatMessage.FromAssistant<TMessage>(functionCall);

        Messages.Add(chatMessage);
        MessageAdded?.Invoke(this, new MessageAddedEventArgs<TMessage>(chatMessage));
    }

    public void FromFunction(IFunctionResult functionResult)
    {
        var chatMessage = IChatMessage.FromFunction<TMessage>(functionResult);

        Messages.Add(chatMessage);
        MessageAdded?.Invoke(this, new MessageAddedEventArgs<TMessage>(chatMessage));
    }

    public void AddFunction(TFunction function)
    {
        Functions.Add(function);
    }

   public void AddFunction(Delegate function)
    {
        var chatFunction = new TFunction
        {
            Name = function.Method.Name,
            Function = function
        };

        Functions.Add(chatFunction);
    }

    public void AddFunction(string name, Delegate function)
    {
        var chatFunction = new TFunction
        {
            Name = name,
            Function = function
        };

        Functions.Add(chatFunction);
    }

    public void AddFunction(string name, string? description, Delegate function)
    {
        var chatFunction = new TFunction
        {
            Name = name,
            Description = description,
            Function = function
        };

        Functions.Add(chatFunction);
    }

    public void RemoveFunction(TFunction function)
    {
        Functions.Remove(function);
    }

    public void ClearFunctions()
    {
        Functions.Clear();
    }
}

public record ChatConversation : ChatConversation<ChatMessage, ChatFunction>
{
    public ChatConversation() { }

    public ChatConversation(string systemMessage) : base(systemMessage) { }
}
