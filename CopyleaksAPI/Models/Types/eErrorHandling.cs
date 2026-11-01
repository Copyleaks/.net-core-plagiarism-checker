namespace Copyleaks.SDK.V3.API.Models.Types
{
    /// <summary>When some scan ids fail: Cancel the entire request, or Ignore them and start the rest.</summary>
    public enum eErrorHandling
    {
        Cancel = 0,
        Ignore = 1
    }
}
