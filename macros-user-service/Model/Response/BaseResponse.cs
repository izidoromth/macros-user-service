namespace macros_user_service.Model.Response
{
    public class BaseResponse<T>
    {
        public string Message { get; set; } = String.Empty;
        public T Content { get; set; }
    }

    public class BaseResponse
    {
        public string Message { get; set; } = String.Empty;
    }
}
