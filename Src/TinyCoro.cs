using System;
using System.Collections;
using System.Text;
using UnityEngine;

public partial class TinyCoro
{
    public string Name { get; set; }
    public bool Alive { get; private set; }
    public event Action<TinyCoro, TinyCoroFinishReason> OnFinished;

    public virtual void Kill()
    {
        if (!Alive)
            return;
        Debug.Log(this + " are kill " + TinyCoroFinishReason.Killed);
        if (OnFinished != null)
            OnFinished(this, TinyCoroFinishReason.Killed);
        Alive = false;
    }

    protected TinyCoro(Func<IEnumerator> operation)
    {
        Alive = true;
        _operation = operation;
    }

    private void Step()
    {
        if (_waitingOn != null)
        {
            if (!_waitingOn())
                return;
            // Finished waiting
            _waitingOn = null;
        }

        if (_ienumerable == null)
        {
            // Execute first block of code
            _ienumerable = _operation();
        }
        else if (_ienumerable.MoveNext())
        {
            // Executed another block of code
            if (_ienumerable.Current != null)
            {
                if (_ienumerable.Current is Func<bool>)
                {
                    _waitingOn = (Func<bool>)_ienumerable.Current;
                }
                else
                {
                    throw new Exception(this + " expected to yield null or a Func<bool> instead a " + _ienumerable.Current + " of type " + _ienumerable.Current.GetType() + " was returned");
                }
            }
        }
        else
        {
            // Executed final block of code
            Alive = false;
            Debug.Log(this + " are kill " + TinyCoroFinishReason.Finished);
            if (OnFinished != null)
                OnFinished(this, TinyCoroFinishReason.Finished);
        }
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}]", Name, base.ToString());
    }

    private Func<IEnumerator> _operation;
    private IEnumerator _ienumerable;
    private Func<bool> _waitingOn;
}

public enum TinyCoroFinishReason
{
    Finished,
    Killed,
}
