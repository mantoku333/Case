using System;
using SRF;
using SRF.Service;

namespace SRDebugger.Internal
{
    /// <summary>
    /// The default bug report handler - this submits to the SRDebugger API using the API key configured in the SRDebugger
    /// settings window.
    /// </summary>
    internal class InternalBugReporterHandler : IBugReporterHandler
    {
        public bool IsUsable
        {
            get { return Settings.Instance.EnableBugReporter && !string.IsNullOrWhiteSpace(Settings.Instance.ApiKey); }
        }

        public string PrivacyPolicyMessage        
        {
            get
            {
                return "By submitting this bug report you agree to our <color=#57B3F4>privacy policy</color>.";
            }
        }

        public string PrivacyPolicyUrl
        {
            get { return SRDebugApi.PrivacyPolicyUrl; }
        }

        public void Submit(BugReport report, Action<BugReportSubmitResult> onComplete, IProgress<float> progress)
        {
            BugReportApi.Submit(report, Settings.Instance.ApiKey, onComplete, progress);
        }
    }
}