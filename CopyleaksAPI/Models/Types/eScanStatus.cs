namespace Copyleaks.SDK.V3.API.Models.Types
{
    /// <summary>
    /// Scan status: CompletedSuccessfully, Error (completed in error), CreditsChecked (credit check completed),
    /// Indexed (content indexed in the Copyleaks internal database) or InProgress.
    /// </summary>
    public enum eScanStatus
    {
        CompletedSuccessfully = 0,
        Error = 1,
        CreditsChecked = 2,
        Indexed = 3,
        InProgress = 4
    }
}
