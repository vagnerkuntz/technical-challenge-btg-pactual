namespace BankKRT.Domain.Validation
{
    public class CpfValidator
    {
        public static bool Validate(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
                return false;

            for (var i = 0; i < 2; i++)
            {
                var sum = 0;
                for (var j = 0; j < 9 + i; j++)
                    sum += (cpf[j] - '0') * (10 + i - j);

                var digitVerifier = sum % 11 < 2 ? 0 : 11 - sum % 11;
                if (cpf[9 + i] != digitVerifier.ToString()[0])
                    return false;
            }

            return true;
        }
    }
}