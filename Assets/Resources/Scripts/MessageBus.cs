using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

public class MessageBus : IMessageBus
{
    private readonly Dictionary<Type, List<WeakReference>> _subscribers =
    new Dictionary<Type, List<WeakReference>>();
    private Dictionary<Guid, GameObject> tiles = new Dictionary<Guid, GameObject>();

    private readonly object _lockObj = new object();

    public Dictionary<Guid, GameObject> Tiles { get => tiles; set => tiles = value; }

    public void PublishEvent<TEventType>(TEventType evt)
    {
        var subscriberType = typeof(ISubscriber<>).MakeGenericType(typeof(TEventType));

        var subscribers = GetSubscriberList(subscriberType);

        List<WeakReference> subsToRemove = new List<WeakReference>();

        foreach (var weakSubscriber in subscribers)
        {
            if (weakSubscriber.IsAlive)
            {
                var subscriber = (ISubscriber<TEventType>)weakSubscriber.Target;
                InvokeSubscriberEvent(evt, subscriber);
            }
            else
            {
                subsToRemove.Add(weakSubscriber);
            }
        }

        // Remove any dead subscribers.
        if (subsToRemove.Count > 0)
        {
            lock (_lockObj)
            {
                foreach (var remove in subsToRemove)
                {
                    subscribers.Remove(remove);
                }
            }
        }
    }

    public void Subscribe(object subscriber)
    {
        lock (_lockObj)
        {
            var subscriberTypes =
            subscriber.GetType()
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISubscriber<>));

            WeakReference weakRef = new WeakReference(subscriber);

            foreach (var subscriberType in subscriberTypes)
            {
                List<WeakReference> subscribers = GetSubscriberList(subscriberType);
                subscribers.Add(weakRef);
            }
        }
    }

    private void InvokeSubscriberEvent<TEventType>(TEventType evt, ISubscriber<TEventType> subscriber)
    {
        subscriber.OnEvent(evt);
    }

    private List<WeakReference> GetSubscriberList(Type subscriberType)
    {
        List<WeakReference> subscribersList = null;

        lock (_lockObj)
        {
            bool found = _subscribers.TryGetValue(subscriberType, out subscribersList);

            if (!found)
            {
                // Create the list.
                subscribersList = new List<WeakReference>();
                _subscribers.Add(subscriberType, subscribersList);
            }
        }

        return subscribersList;
    }
}
