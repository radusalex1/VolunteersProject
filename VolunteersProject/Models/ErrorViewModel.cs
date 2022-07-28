namespace VolunteersProject.Models
{
    /// <summary>
    /// Error model.
    /// </summary>
    public class ErrorViewModel
    {
        private int requestId;
        private bool showRequestId;
        private string errMsg;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <param name="showRequestId">Show request id.</param>
        /// <param name="errMsg">Error message.</param>
        public ErrorViewModel(int requestId = 0, bool showRequestId = false, string errMsg="")
        {
            this.requestId = requestId;
            this.showRequestId = showRequestId;
            this.errMsg = errMsg;
        }

        /// <summary>
        /// Gets or sets request id.
        /// </summary>
        public string RequestId {
            get
            {
                return requestId.ToString();
            }
            set
            {
                _ = requestId;
            }
        }

        /// <summary>
        /// Get or sets show request id.
        /// </summary>
        public bool ShowRequestId
        {
            get
            {
                return showRequestId;
            }
            set
            {
                _ = requestId != 0 && showRequestId;
            }
        }

        /// <summary>
        /// Gets or sets error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return errMsg;
            }
            set
            {
                _ = errMsg;
            }
        }
    }
}
