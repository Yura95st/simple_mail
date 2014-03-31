﻿using data_models.Exceptions;
using data_models.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace data_models.Validations
{
    public static class MyValidation
    {
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
              + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
              + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }

        public static string Hash(string value, string salt)
        {
            return Hash(Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(salt));
        }

        public static string Hash(string value, byte[] salt)
        {
            return Hash(Encoding.UTF8.GetBytes(value), salt);
        }

        public static string Hash(byte[] value, byte[] salt)
        {
            var saltedValue = new byte[value.Length + salt.Length];
            value.CopyTo(saltedValue, 0);
            salt.CopyTo(saltedValue, value.Length);

            return BitConverter.ToString(new SHA256Managed().ComputeHash(saltedValue));
        }

        public static bool IsValidFirstName(string firstName)
        {
            return true;
        }

        public static void CheckValidUserFields(User user)
        {
            if (!IsValidEmail(user.Email)) 
            {
                throw new InvalidEmailException(user.Email);
            }

            if (!IsValidFirstName(user.FirstName))
            {
                throw new InvalidFirstNameException(user.FirstName);
            }

            if (!IsValidPassword(user.Password))
            {
                throw new InvalidPasswordException(user.FirstName);
            }
        }

        public static bool IsValidPassword(string password)
        {
            return true;
        }

        public static bool AreTwoPasswordsEqual(string pass1, string pass2)
        {
            return String.Equals(pass1, pass2);
        }

        public static void CheckValidMessageFields(Message message)
        {
            if (string.Equals(message.Subject, ""))
            {
                throw new EmtpyMessageSubjectException();
            }

            if (string.Equals(message.Text, ""))
            {
                throw new EmptyMessageTextException();
            }

            if (message.Author.Id <=0)
            {
                throw new InvalidMessageAuthorException();
            }

            if (message.Recipient.Id <= 0)
            {
                throw new InvalidMessageRecipientException();
            }
        }
    }
}
