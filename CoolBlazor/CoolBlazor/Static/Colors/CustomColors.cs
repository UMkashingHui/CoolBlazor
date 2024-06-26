using System.Diagnostics.CodeAnalysis;
using MudBlazor;

namespace CoolBlazor.Static.Colors
{
    [ExcludeFromCodeCoverage]
    public class CustomColors
    {
        public static string LogoColor
        {
            get { return "#070803"; }
        }

        public static Color ButtonColor
        {
            get { return Color.Secondary; }
        }

        public static string AvatarColor
        {
            get { return "rgb(255, 238, 88)"; }
        }
    }
}