using System.Text.RegularExpressions;

namespace Academy.CfdiService.Application.Common.Helpers;

public static class RfcValidator
{
    private static readonly Regex PersonaMoralRegex = new(
        @"^[A-Z&Ñ]{3}\d{6}[A-Z0-9]{3}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PersonaFisicaRegex = new(
        @"^[A-Z&Ñ]{4}\d{6}[A-Z0-9]{3}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool IsValid(string? rfc)
    {
        if (string.IsNullOrWhiteSpace(rfc))
        {
            return false;
        }

        return IsPersonaFisica(rfc) || IsPersonaMoral(rfc);
    }

    public static bool IsPersonaFisica(string? rfc)
    {
        if (string.IsNullOrWhiteSpace(rfc))
        {
            return false;
        }

        return PersonaFisicaRegex.IsMatch(rfc.Trim());
    }

    public static bool IsPersonaMoral(string? rfc)
    {
        if (string.IsNullOrWhiteSpace(rfc))
        {
            return false;
        }

        return PersonaMoralRegex.IsMatch(rfc.Trim());
    }
}
