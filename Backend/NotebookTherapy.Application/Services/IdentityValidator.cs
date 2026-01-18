namespace NotebookTherapy.Application.Services;

public static class IdentityValidator
{
    public static bool ValidateTcKimlikNo(string? tcno)
    {
        if (string.IsNullOrEmpty(tcno) || tcno.Length != 11 || !tcno.All(char.IsDigit))
            return false;

        if (tcno[0] == '0') return false;

        int[] digits = tcno.Select(c => int.Parse(c.ToString())).ToArray();

        int oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        int evenSum = digits[1] + digits[3] + digits[5] + digits[7];

        int digit10 = (oddSum * 7 - evenSum) % 10;
        int digit11 = (oddSum + evenSum + digit10) % 10;

        return digits[9] == digit10 && digits[10] == digit11;
    }

    public static bool ValidateTaxNumber(string? taxNo)
    {
        if (string.IsNullOrEmpty(taxNo)) return false;
        return (taxNo.Length == 10 || taxNo.Length == 11) && taxNo.All(char.IsDigit);
    }
}
