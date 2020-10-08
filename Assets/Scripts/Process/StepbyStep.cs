using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace UIManagerProcess
{
    class TickTock
    {
        public float currentTime = 0;
        float totalTime;

        public bool intervalPassed =false;

        public void SetInterval(float t)
        {
            currentTime = 0;
            totalTime = t;
            intervalPassed = false;
        }

        public void Update()
        {
            intervalPassed = false;
            currentTime += Time.unscaledDeltaTime;
            if(currentTime >= totalTime)
            {
                currentTime -= totalTime;
                intervalPassed = true;
            }
        }
    }
    
    class StepbyStep
    {
        struct QueuedStep
        {
            public Func<bool> func;
            public string name;

            public QueuedStep(Func<bool> fn, string n)
            {
                func = fn;
                name = n;
            }
        }

        Queue<QueuedStep> actions = new Queue<QueuedStep>();
        System.Action<Exception> exceptionHandler = (e)=> { };
        System.Action intervalAction = null;
        TickTock ticktock = new TickTock();

        public void Update()
        {
            
            try
            {
                if (actions.Count == 0)
                {
                    return;
                }

                if(intervalAction != null)
                {
                    ticktock.Update();
                    if (ticktock.intervalPassed)
                    {
                        intervalAction();
                    }
                }

                var action = actions.Peek();
                if (action.func())
                {
                    actions.Dequeue();
                }
            }
            catch(Exception e)
            {
                exceptionHandler(e);
            }
        }

        public void Catch(System.Action<Exception> handler)
        {
            exceptionHandler = handler;
        }

        public void ClearQueue()
        {
            actions.Clear();
        }

        public StepbyStep Do(System.Action action)
        {
            return Do(action, "");
        }

        public StepbyStep Do(System.Action action, string name)
        {
            actions.Enqueue(new QueuedStep(
                () =>
                {
                    action();
                    return true;
                }, name
                ));

            return this;
        }

        public StepbyStep Wait(System.Func<bool> action)
        {
            return Wait(action, "");
        }

        public StepbyStep Wait(System.Func<bool> action, string name)
        {
            actions.Enqueue(new QueuedStep(action, name));
            return this;
        }

        public StepbyStep Wait(int loops)
        {
            return Wait(loops, "");
        }

        public StepbyStep Wait(int loops, string name)
        {
            int currentLoop = 0;
            actions.Enqueue(new QueuedStep(
                () =>
                {
                    if (currentLoop >= loops)
                    {
                        return true;
                    }
                    ++currentLoop;
                    return false;
                }, name
                ));
            return this;
        }

        public void SkipTo(string str)
        {
            while(actions.Count != 0)
            {
                var action = actions.Peek();
                if(action.name == str)
                {
                    return;
                }
                actions.Dequeue();
            }
        }

        public void Interval(System.Action action, float time, bool executeImmdiately = false)
        {
            ticktock.SetInterval(time);
            if (executeImmdiately)
            {
                ticktock.currentTime = time;
            }
            intervalAction = action;
        }

        public void ClearInterval()
        {
            intervalAction = null;
        }
    }
}
