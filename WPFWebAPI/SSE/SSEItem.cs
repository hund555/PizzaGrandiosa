namespace WPFWebAPI.SSE
{
    public class SSEItem
    {
        public HttpContext Context { get; set; }
        public CancellationToken Token { get; set; }
    }
}
