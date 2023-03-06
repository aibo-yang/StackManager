namespace StackManager.Context.Domain
{
    /// <summary>
    /// 报警
    /// </summary>
    class DeviceAlarm : IEntity
    {
        public AlarmCategory AlarmCategory { get; set; }
        public bool IsRaised { get; set; }
    }
}
