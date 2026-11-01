using System.ComponentModel;

namespace Copyleaks.SDK.V3.API.Models.Types
{
	public enum eAlertSeverity
	{
        [Description("DEBUG")]
        Debug = 0,

        [Description("INFO")]
        Info = 1,

        [Description("WARNING")]
        Warning = 2,

        [Description("ERROR")]
        Error = 3,

        [Description("CRITICAL")]
        Critical = 4
    }
}
