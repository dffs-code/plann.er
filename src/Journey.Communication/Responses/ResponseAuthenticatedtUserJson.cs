namespace Journey.Communication.Responses
{
    public class ResponseAuthenticatedUser
    {
        public Guid Id {  get; set; }

        public string Username { get; set; } =  string.Empty;

        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
