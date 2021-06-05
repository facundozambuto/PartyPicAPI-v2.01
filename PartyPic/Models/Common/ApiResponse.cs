using System.Collections.Generic;

namespace PartyPic.Models.Common
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            Errors = new List<Error>();
        }

        public bool Success { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public Error(string code, string description)
        {
            Code = code;
            Description = description;
        }
        public Error(string description)
        {
            Description = description;
        }
        public Error()
        { }
    }
}
