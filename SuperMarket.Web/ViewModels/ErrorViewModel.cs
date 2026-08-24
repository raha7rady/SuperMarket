using System;

namespace SuperMarket.Web.ViewModels
{
    public sealed class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Path { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}