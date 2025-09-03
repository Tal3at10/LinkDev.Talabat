namespace LinkDev.Talabat.APIs.Controllers.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        // هنا هنخليها لستة من ValidationError بدل IEnumerable<string>
        public IEnumerable<ValidationError> Errors { get; set; }

        public ApiValidationErrorResponse() : base(400)
        {
            Errors = new List<ValidationError>();
        }

        public class ValidationError
        {
            public required string Field { get; set; }
            public required IEnumerable<string> Errors { get; set; }
        }
    }
}
