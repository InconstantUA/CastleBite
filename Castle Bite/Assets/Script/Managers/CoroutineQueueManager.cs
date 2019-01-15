using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineQueueManager : Singleton<CoroutineQueueManager>
{
    private static CoroutineGroupQueue blockingCoroutineGroupQueue;

    void Awake()
    {
        // create new queue which can execute only one task at a time
        blockingCoroutineGroupQueue = new CoroutineGroupQueue(1, StartCoroutine);
    }

    // doBlockOtherAnimations - means that no other actions can be executed at the same time
    // animation group - all members of animation group are executed at the same time
    public static void Run(IEnumerator coroutine, string coroutineGroupID = null)
    {
        blockingCoroutineGroupQueue.Run(coroutine, coroutineGroupID);
    }

    // idea:
    // doExtendBlock - means that blocking coroutins cannot be executed at the same time, 
    //   but non-blocking coroutines can be executed, but with respect to block time duration,
    //   block duration is set based on maximum coroutine duraiton submitted during current frame
}
