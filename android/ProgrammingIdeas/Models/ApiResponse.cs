namespace ProgrammingIdeas.Models
{
    internal class ApiResponse<T>
    {
        public T Payload { get; set; }
        public string ErrorMessage { get; set; }

        public ApiResponse(T payload, string errorMessage)
        {
            Payload = payload;
            ErrorMessage = errorMessage;
        }
    }
}