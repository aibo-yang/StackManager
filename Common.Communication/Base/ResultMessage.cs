namespace Common.Communication
{
    public enum ResultCode : int
    {
        Succeed = 0,
        Error = -1,
        NotConnected = -2,
        Pending = -3,
        ReadFailed = -4,
        WriteFailed = -5,
        Timeout = -6,
        ArgumentError = -7,
        Unknown = -100,
    }

    public class ResultMessage
    {
        public ResultCode Code { get; set; }
        public string  Message { get; set; }
    }
}
