namespace ProcessDoctor.Backend.Linux.Proc.StatusFile.Enums;

public enum ProcessState
{
    /// <summary>
    /// Running
    /// </summary>
    Running,

    /// <summary>
    /// Sleeping
    /// </summary>
    Sleeping,

    /// <summary>
    /// Sleeping in an uninterruptible wait
    /// </summary>
    UninterruptibleWait,

    /// <summary>
    /// Zombie
    /// </summary>
    Zombie,

    /// <summary>
    /// Traced or stopped
    /// </summary>
    TracedOrStopped
}
