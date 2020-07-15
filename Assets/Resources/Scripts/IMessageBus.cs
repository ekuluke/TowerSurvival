using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IMessageBus
    {

        void PublishEvent<TEventType>(TEventType evt);
        void Subscribe(object subscriber);
    }
