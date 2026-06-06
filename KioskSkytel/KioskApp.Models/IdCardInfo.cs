using System.Collections.Generic;
using System.Text;

namespace KioskApp.Models
{
    public class IdCardInfo
    {
        public string? Atr { get; set; }
        public List<string> Applications { get; set; } = new();
        public string? SelectedApplication { get; set; }
        public string? RegisterNumber { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? RawData { get; set; }
        public string Log { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public string ToDisplayString()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(Atr))
                sb.AppendLine($"ATR: {Atr}");

            if (Applications.Count > 0)
            {
                sb.AppendLine("Applications:");
                foreach (var app in Applications)
                    sb.AppendLine($"  - {app}");
            }

            if (!string.IsNullOrEmpty(SelectedApplication))
                sb.AppendLine($"Selected: {SelectedApplication}");

            if (!string.IsNullOrEmpty(RegisterNumber))
                sb.AppendLine($"Register No: {RegisterNumber}");

            if (!string.IsNullOrEmpty(LastName))
                sb.AppendLine($"Last name: {LastName}");

            if (!string.IsNullOrEmpty(FirstName))
                sb.AppendLine($"First name: {FirstName}");

            if (!string.IsNullOrEmpty(RawData))
                sb.AppendLine($"Data: {RawData}");

            if (!string.IsNullOrEmpty(ErrorMessage))
                sb.AppendLine($"Note: {ErrorMessage}");

            if (!string.IsNullOrEmpty(Log))
            {
                sb.AppendLine();
                sb.AppendLine(Log);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
