using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class TinyCoro
{
    public static TinyCoro Current { get; private set; }

    /// <summary>
    /// Adds an new TinyCoro to the pool after the currently exceutiny one (or last if none are running) 
    /// </summary>
    /// <param name="threadedOperation"></param>
    /// <returns></returns>
    public static TinyCoro SpawnNext(Func<IEnumerator> operation, string name = null)
    {
        var coro = new TinyCoro(operation);
        coro.Name = name;
        var index = _allCoros.Count;
        if (Current != null)
            index = _allCoros.IndexOf(Current) + 1;
        _allCoros.Insert(index, coro);

        Debug.Log(coro + " created at index " + index);
        return coro;
    }

    public static void StepAllCoros()
    {
        for (int i = 0; i < _allCoros.Count; )
        {
            //Step normal
            Current = _allCoros[i];
            if (Current.Alive)
                Current.Step();
            if (!Current.Alive)
            {
                _allCoros.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
        Current = null;
    }


    /// <summary>
    /// Human readable version of "yield return of Func<bool>"
    /// Any half decent compiler should optimize this :P
    /// </summary>
    /// <param name="conditionMet"></param>
    /// <returns></returns>
    public static Func<bool> WaitUntil(Func<bool> conditionMet)
    {
        return conditionMet;
    }

    public static Func<bool> Wait(float seconds)
    {
        var destinationTime = Time.time + seconds;
        return () => Time.time >= destinationTime;
    }

    public static Func<bool> Join(params TinyCoro[] waitFor)
    {
        return () => waitFor.All(c => !c.Alive);
    }


    public static IEnumerable<TinyCoro> AllCoroutines { get { return _allCoros; } }

    private static List<TinyCoro> _allCoros = new List<TinyCoro>();
}