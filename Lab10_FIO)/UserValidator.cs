using System.Text.RegularExpressions;

namespace Lab10_FIO_
{

    public class UserValidator
    {
        private static readonly HashSet<string> _reservedLogins = new HashSet<string>
        {
            "admin",
            "root",
            "system",
            "administrator",
            "user",
            "test",
            "guest",
            "moderator",
            "support",
            "info"
        };

       
        private static readonly Regex _phoneRegex = new Regex(@"^\+\d-\d{3}-\d{3}-\d{4}$");

      
        private static readonly Regex _emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

       
        private static readonly Regex _simpleLoginRegex = new Regex(@"^[a-zA-Z0-9_]+$");

        
        public (bool success, string message) ValidateUser(string login, string password, string passwordConfirm)
        {
           
            if (string.IsNullOrWhiteSpace(login))
                return (false, "Ошибка 1: Логин не может быть пустым или состоять только из пробелов.");

            if (string.IsNullOrWhiteSpace(password))
                return (false, "Ошибка 2: Пароль не может быть пустым или состоять только из пробелов.");

            if (string.IsNullOrWhiteSpace(passwordConfirm))
                return (false, "Ошибка 3: Подтверждение пароля не может быть пустым или состоять только из пробелов.");

          
            bool isPhone = _phoneRegex.IsMatch(login);
            bool isEmail = _emailRegex.IsMatch(login);
            bool isSimpleLogin = false;

            if (!isPhone && !isEmail)
            {
                
                if (login.Length < 5)
                    return (false, "Ошибка 4: Простой логин должен содержать минимум 5 символов.");

                if (!_simpleLoginRegex.IsMatch(login))
                    return (false, "Ошибка 5: Простой логин может содержать только латиницу, цифры и знак подчеркивания (_).");

                isSimpleLogin = true;
            }

            if (_reservedLogins.Contains(login.ToLower()))
                return (false, "Ошибка 6: Данный логин зарезервирован и не может быть использован.");

           
            if (password.Length < 7)
                return (false, "Ошибка 7: Пароль должен содержать минимум 7 символов.");

       
            bool hasCyrillic = Regex.IsMatch(password, @"[а-яА-Я]");
            bool hasLatin = Regex.IsMatch(password, @"[a-zA-Z]");

            if (hasLatin)
                return (false, "Ошибка 8: Пароль должен содержать только кириллицу, цифры и спецсимволы. Латиница запрещена.");

            if (!hasCyrillic)
                return (false, "Ошибка 9: Пароль должен содержать хотя бы одну букву кириллицы.");

            bool hasUpper = Regex.IsMatch(password, @"[А-Я]");
            bool hasLower = Regex.IsMatch(password, @"[а-я]");

            if (!hasUpper)
                return (false, "Ошибка 10: Пароль должен содержать минимум одну букву в верхнем регистре (А-Я).");

            if (!hasLower)
                return (false, "Ошибка 11: Пароль должен содержать минимум одну букву в нижнем регистре (а-я).");

           
            bool hasDigit = Regex.IsMatch(password, @"\d");
            if (!hasDigit)
                return (false, "Ошибка 12: Пароль должен содержать минимум одну цифру.");

            bool hasSpecialChar = Regex.IsMatch(password, @"[^\w\sа-яА-Я\d]");
            if (!hasSpecialChar)
                return (false, "Ошибка 13: Пароль должен содержать минимум один специальный символ (например, !@#$%^&* и т.д.).");

         
            if (password != passwordConfirm)
                return (false, "Ошибка 14: Пароль и подтверждение пароля не совпадают.");

            return (true, string.Empty);
        }

     
        public static IReadOnlySet<string> GetReservedLogins() => _reservedLogins;
    }
}