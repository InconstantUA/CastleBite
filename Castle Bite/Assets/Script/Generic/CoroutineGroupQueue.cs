using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Coroutine group may contain one more more coroutines, which are executed simultaneously
public class CoroutineGroup
{
    private string coroutineGroupID;
    private uint numberOfCoroutinesToExecute;
    private readonly Func<IEnumerator, Coroutine> coroutineStarter;
    private List<IEnumerator> coroutines;
    private bool isRunning;

    public string CoroutineGroupID
    {
        get
        {
            return coroutineGroupID;
        }
    }

    public uint NumberOfCoroutinesToExecute
    {
        get
        {
            return numberOfCoroutinesToExecute;
        }
    }

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }
    }

    public CoroutineGroup(string coroutineGroupID, Func<IEnumerator, Coroutine> coroutineStarter)
    {
        // set group id
        this.coroutineGroupID = coroutineGroupID;
        // set starter
        this.coroutineStarter = coroutineStarter;
        // int list
        coroutines = new List<IEnumerator>();
        // reset is running flag
        isRunning = false;
        // reset number of coroutines to execute
        numberOfCoroutinesToExecute = 0;
    }

    public void Add(IEnumerator coroutine)
    {
        // verify if the same coroutine is already in the list (not sure about consequences)
        if (coroutines.Contains(coroutine))
        {
            Debug.LogWarning(coroutineGroupID + " already contains " + coroutine.ToString() + " coroutine");
        }
        // increment number of coroutines to execute
        numberOfCoroutinesToExecute++;
        // verify if group is already running
        if (isRunning)
        {
            // run this coroutine
            Run(coroutine);
        }
        else
        {
            // save this coroutine to the list
            coroutines.Add(coroutine);
        }
    }

    // coroutine: Coroutine to run
    private void Run(IEnumerator coroutine)
    {
        Debug.Log("Run " + coroutine.ToString());
        // create new coroutine runner
        var runner = CoroutineRunner(coroutine);
        // run new coroutine
        coroutineStarter(runner);
    }

    // Runs a coroutine
    // Decrements numOfCoroutinesToExecute after coroutine has finished.
    // returns values yielded by the given coroutine
    // coroutine: Coroutine to run
    private IEnumerator CoroutineRunner(IEnumerator coroutine)
    {
        // Run coroutine manually. This way we can be notified when the coroutine is finished
        // Coroutines are just an IEnumerator so it’s easy to loop over them and yield return their values:
        // Each time we call MoveNext the user’s coroutine is resumed and a bool is returned to indicate if it yielded anything. 
        // If it did, we can get it from the Current property. 
        // This means that the while loop basically just runs the user’s coroutine and yields all its values.
        while (coroutine.MoveNext())
        {
            yield return coroutine.Current;
        }
        // coroutine has finished
        // decrement number of number of coroutines left to execute
        numberOfCoroutinesToExecute--;
        // verify if number of numberOfCoroutinesToExecute is 0, means we are done
        if (numberOfCoroutinesToExecute == 0)
        {
            // reset is running flag
            isRunning = false;
        }
    }

    // Run coroutine group
    // Sets isRunning flag
    // Run all coroutines in a list
    public void Run()
    {
        // set is running flag
        isRunning = true;
        // loop through the list of coroutines
        foreach(IEnumerator coroutine in coroutines)
        {
            // run coroutine
            Run(coroutine);
        }
    }
}

// Imposes a limit on the maximum number of coroutine groups that can be running at the same time. 
// Runs coroutine groups until the limit is reached and then begins queueing coroutine groups. 
// When all coroutines in a group has finished, runs new coroutine group from a queue if any is present
// Can be used to run individual coroutines, by wrapping them into a unique random group
// Non-Group version is taken from Jackson Dunstan, http://JacksonDunstan.com/articles/3241
public class CoroutineGroupQueue
{
    // Maximum number of coroutine groups to run at once
    private readonly uint maxActive;

    // Delegate to start coroutines with
    private readonly Func<IEnumerator, Coroutine> coroutineStarter;

    // Queue of coroutine groups waiting to start
    private readonly Queue<CoroutineGroup> queue;

    // Number of currently active coroutines
    private uint numActive;

    private CoroutineGroup runningCoroutineGroup;

    // Create the queue, initially with no coroutine groups
    // maxActive:
    // Maximum number of coroutines to run at once. This must be at least one.
    // coroutineStarter:
    // Delegate to start coroutines with. Normally you'd pass
    // MonoBehaviour.StartCoroutinefor this.
    // Exception ArgumentException if maxActive is zero.
    public CoroutineGroupQueue(uint maxActive, Func<IEnumerator, Coroutine> coroutineStarter)
    {
        if (maxActive == 0)
        {
            throw new ArgumentException("Must be at least one", "maxActive");
        }
        this.maxActive = maxActive;
        this.coroutineStarter = coroutineStarter;
        queue = new Queue<CoroutineGroup>();
        runningCoroutineGroup = null;
    }

    public CoroutineGroup GetCoroutineGroupFromQueueByID(string coroutineGroupID)
    {
        // loop through all coroutine groups in a queue
        foreach (CoroutineGroup coroutineGroup in queue)
        {
            // verify if CoroutineGroupID matches
            if (coroutineGroupID == coroutineGroup.CoroutineGroupID)
            {
                return coroutineGroup;
            }
        }
        return null;
    }

    public CoroutineGroup GetCoroutineGroupByID(string coroutineGroupID)
    {
        // verify if coroutine group ID is empty
        if (coroutineGroupID == null)
        {
            // create new random group id
            coroutineGroupID = Guid.NewGuid().ToString();
            // create and return new coroutine group
            return new CoroutineGroup(coroutineGroupID, coroutineStarter);
        }
        else
        {
            // verify if running coroutine group is not null
            if (runningCoroutineGroup != null)
            {
                // verify if this not already running coroutin group
                // Note running coroutine group is already removed from queue, so you will not be able to find it there
                if (runningCoroutineGroup.CoroutineGroupID == coroutineGroupID)
                {
                    // return reference to the running coroutine group
                    return runningCoroutineGroup;
                }
            }
            // if we are still here..
            // try to get coroutine group by its ID from a queue
            CoroutineGroup coroutineGroup = GetCoroutineGroupFromQueueByID(coroutineGroupID);
            // verify if coroutine group is null
            if (coroutineGroup == null)
            {
                // create new coroutine group with defined ID
                return new CoroutineGroup(coroutineGroupID, coroutineStarter);
            }
            else
            {
                // return reference to existing coroutine group from queue
                return coroutineGroup;
            }
        }
    }

    // Run corotine with defined or not defined group ID
    public void Run(IEnumerator coroutine, string coroutineGroupID = null)
    {
        if (coroutineGroupID != null)
        {
            Debug.Log("Running " + coroutine.ToString() + " coroutine with " + coroutineGroupID + " group ID");
        }
        else
        {
            Debug.Log("Running " + coroutine.ToString() + " coroutine ");
        }
        // init coroutineGroup variable
        CoroutineGroup coroutineGroup = GetCoroutineGroupByID(coroutineGroupID);
        // add coroutine to the group
        coroutineGroup.Add(coroutine);
        // verify if coroutine group is not already running
        if (!coroutineGroup.IsRunning)
        {
            // try to run Coroutine group
            Run(coroutineGroup);
        }
    }

    // If the number of active coroutines is under the limit specified in the constructor, run the
    // given coroutine. Otherwise, queue it to be run when other coroutines finish.
    // coroutine: Coroutine group to run or place in a queue
    public void Run(CoroutineGroup coroutineGroup)
    {
        // verify if number of running coroutine groups is less then max allowed number
        if (numActive < maxActive)
        {
            Debug.Log("Run " + coroutineGroup.CoroutineGroupID);
            // create new coroutine group runner
            var runner = CoroutineGroupRunner(coroutineGroup);
            // run new coroutine
            coroutineStarter(runner);
        }
        else
        {
            // verify if group is not already in a queue
            if (GetCoroutineGroupFromQueueByID(coroutineGroup.CoroutineGroupID) == null)
            {
                Debug.Log("Queued " + coroutineGroup.CoroutineGroupID);
                queue.Enqueue(coroutineGroup);
            }
            else
            {
                Debug.Log(coroutineGroup.CoroutineGroupID + " group is already in a queue");
            }
        }
    }

    // Runs a coroutine group then runs the next queued coroutine group if available.
    // Increments numActive before running the coroutine and decrements it after.
    // returns values yielded by the given coroutine
    // coroutineGroup: Coroutine Group to run
    private IEnumerator CoroutineGroupRunner(CoroutineGroup coroutineGroup)
    {
        // increment number of active coroutines
        numActive++;
        // save a reference to currently running corouting group
        runningCoroutineGroup = coroutineGroup;
        // Run coroutine group
        coroutineGroup.Run();
        // wait while all coroutines in the group are finished
        while (coroutineGroup.NumberOfCoroutinesToExecute > 0)
        {
            // Debug.Log("Coroutine group NumberOfCoroutinesToExecute: " + coroutineGroup.NumberOfCoroutinesToExecute);
            // skip to the next frame
            yield return null;
        }
        // coroutine is finished
        // reset running corouting group reference
        runningCoroutineGroup = null;
        // decrement number of active coroutines
        numActive--;
        // verify if there still coroutine groups left in a queue
        if (queue.Count > 0)
        {
            // get next coroutine group from the queue
            var nextCoroutineGroup = queue.Dequeue();
            // run next coroutine group
            Run(nextCoroutineGroup);
        }
    }
}