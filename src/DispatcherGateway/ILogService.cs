namespace DispatcherGateway
{
    public interface ILogService
    {
         Task LogRequest(HttpContext context);


    }
}
