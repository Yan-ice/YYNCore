public abstract class Event
{
}
public abstract class AnimatedEvent : Event
{
    public AnimQueue anim_queue { get; private set; } = new AnimQueue();

    /// <summary>
    /// 调用该函数后，将无法再往队列中写入任务。
    /// </summary>
    public void EndEnqueue()
    {
        anim_queue.EnqueueSimpleAction(anim_queue.DestroyImmediate, AnimType.After);
        anim_queue = null;
    }
}