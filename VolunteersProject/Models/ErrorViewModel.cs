using System;

namespace VolunteersProject.Models
{
    /// <summary>
    /// Error model.
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ErrorMessage { get; internal set; }
    }
}
