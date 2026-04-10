namespace WPFWebAPI.SSE
{
    public class SSEPayload
    {
        public string Message { get; set; }
        public bool IsKeepAlive { get; set; }
    }
}
