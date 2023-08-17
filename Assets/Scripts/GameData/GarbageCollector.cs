using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using System;
using System.Diagnostics;

namespace Game {
    public class GarbageCollector : IJob
    {
        static GarbageCollector main;
        static public GarbageCollector GC { get { return main; } }

        static float timeLastClear = 0;
        const float timeToClear = 300;
        static ulong sizeMax = 17179869184;

        static public bool isTimeToClear() {
            main ??= new GarbageCollector();

            bool result = false;

            //Узнаем сколько памяти потребляет программа в данный момент времени
            Process process = Process.GetCurrentProcess();
            ulong nowMemory = (ulong)process.WorkingSet64;
            ulong maxMemory = (ulong)Process.GetCurrentProcess().MaxWorkingSet.ToInt64();

            //Коофицент использования оперативной памяти
            float coofUsedMemory = nowMemory / maxMemory;

            if (Time.unscaledTime <= timeLastClear + timeToClear)
                return false;

            timeLastClear = Time.unscaledTime;
            result = true;

            return result;
        }

        public void Execute()
        {
            ClearMemory();
        }

        static public void ClearMemory() {
            Resources.UnloadUnusedAssets();
            UnityEngine.Debug.Log("Garbage Colector - Collect");
        }
    }
}
