using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface ISubscriber<TEventType>
    {
        void OnEvent(TEventType evt);
    }

