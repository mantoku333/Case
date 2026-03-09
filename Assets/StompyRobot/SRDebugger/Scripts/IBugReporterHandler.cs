using System;
using System.Collections.Generic;
using SRDebugger.Services;

namespace SRDebugger
{
    public class BugReport
    {
        public List<ConsoleEntry> ConsoleLog;

        /// <summary>
        /// User-entered email address
        /// </summary>
        public string Email;

        /// <summary>
        /// Raw data of the captured screenshot (png)
        /// </summary>
        public byte[] ScreenshotData;

        public Dictionary<string, Dictionary<string, object>> SystemInformation;

        public string UserDescription;
    }

    public sealed class BugReportSubmitResult
    {
        public static BugReportSubmitResult Success
        {
            get { return new BugReportSubmitResult(true, null); }
        }

        public static BugReportSubmitResult Error(string errorMessage)
        {
            return new BugReportSubmitResult(false, errorMessage);
        }

        public bool IsSuccessful { get; }

        public string ErrorMessage { get; }

        private BugReportSubmitResult(bool successful, string error)
        {
            IsSuccessful = successful;
            ErrorMessage = error;
        }
    }

    public interface IBugReporterHandler
    {
        /// <summary>
        /// Returns true if this bug reporter handler is able to be used.
        /// If false, the bug reporter tab will be hidden.
        /// </summary>
        bool IsUsable { get; }

        /// <summary>
        /// Message presented to the user to describe the privacy policy.
        /// Should be one or two sentences, and include markup that presents the visual of a link (see example below).
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// "By submitting this form you agree to Your Company Ltd processing your data and agree to our <color=#57B3F4>privacy policy</color>."
        /// ]]>
        /// </example>
        string PrivacyPolicyMessage { get; }

        /// <summary>
        /// Return the URL to the privacy policy for the user to visit if they choose to read it.
        /// </summary>
        string PrivacyPolicyUrl { get; }

        /// <summary>
        /// Submit a new bug report to the handler.
        /// </summary>
        /// <param name="report">The report to be submitted.</param>
        /// <param name="onComplete">Action to be invoked when the bug report is completed.</param>
        /// <param name="progress">Callback to set the current submit progress.</param>
        void Submit(BugReport report, Action<BugReportSubmitResult> onComplete, IProgress<float> progress);
    }
}