using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class GlobalMessageBus
    {
        private static MessageBus instance;

        public static MessageBus Instance
        {
            get { return instance ?? (instance = new MessageBus()); 
            }
        }



}
