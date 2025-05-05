using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Common
{
    public static class ServiceCommon
    {
        private static readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();
        public static IHttpContextAccessor? HttpContextAccessor { get; set; }
        public static IConfiguration? Configuration { get; set; }

        public static string? GetCurrentUserClaim(string claim)
        {
            return HttpContextAccessor?.HttpContext?.User?.FindFirst(claim)?.Value;
        }

        public static string? GetConfiguration(string configuration)
        {
            return Configuration?[configuration];
        }

        public static string GenerateJwtToken(User user, RoleUser roleUser)
        {
            var jwtSection = Configuration?.GetSection("Jwt");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSection["Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role, roleUser?.RoleType.ToString()) // 1. Admin; 2. User
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string HashPassword(User user, string plainPassword)
        {
            return _hasher.HashPassword(user, plainPassword);
        }

        public static bool VerifyPassword(User user, string hashedPassword, string plainPassword)
        {
            var result = _hasher.VerifyHashedPassword(user, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }

        public static bool IsCompletelyOutsideRange(DateTime startDate, DateTime endDate, DateTime fromDate, DateTime toDate)
        {
            return endDate < fromDate || startDate > toDate;
        }

        public static string GenerateReceiptNo(string lastReceiptNo)
        {
            int lastNumber = 0;

            if (!string.IsNullOrEmpty(lastReceiptNo))
            {
                var numberPart = new string(lastReceiptNo.SkipWhile(c => !char.IsDigit(c)).ToArray());
                int.TryParse(numberPart, out lastNumber);
            }

            string newCode = $"{(lastNumber + 1).ToString("D5")}";
            return newCode;
        }

        public static byte[] GenerateReceiptPdf(ReceiptModel model)
        {
            var document = new ReceiptDocument(model);
            return document.GeneratePdf();
        }

        private static readonly string[] Units = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

        public static string NumberToWords(decimal number)
        {
            if (number == 0)
                return "Không đồng";

            var sNumber = ((long)number).ToString();
            string result = "";
            int group = 0;
            bool isFirstGroup = true;

            while (sNumber.Length > 0)
            {
                int take = sNumber.Length >= 3 ? 3 : sNumber.Length;
                var part = sNumber.Substring(sNumber.Length - take, take);
                sNumber = sNumber.Substring(0, sNumber.Length - take);

                string partWord = ReadGroup(int.Parse(part));
                if (!string.IsNullOrEmpty(partWord))
                {
                    if (!string.IsNullOrEmpty(result))
                        result = partWord + " " + GetUnit(group) + " " + result;
                    else
                        result = partWord + " " + GetUnit(group);
                }
                else
                {
                    if (!isFirstGroup && result != "")
                        result = "không " + GetUnit(group) + " " + result;
                }

                group++;
                isFirstGroup = false;
            }

            result = result.Trim();
            result = char.ToUpper(result[0]) + result.Substring(1);
            return result + " đồng chẵn";
        }

        private static string ReadGroup(int number)
        {
            int hundreds = number / 100;
            int tens = (number % 100) / 10;
            int units = number % 10;

            string result = "";

            if (hundreds > 0)
            {
                result += Units[hundreds] + " trăm";
                if (tens == 0 && units > 0)
                    result += " linh";
            }

            if (tens > 0)
            {
                if (tens == 1)
                    result += " mười";
                else
                    result += " " + Units[tens] + " mươi";
            }

            if (units > 0)
            {
                if (tens == 0 && hundreds == 0)
                    result += Units[units];
                else if (tens == 0 && hundreds != 0)
                    result += " " + Units[units];
                else if (tens == 1)
                {
                    if (units == 5)
                        result += " lăm";
                    else
                        result += " " + Units[units];
                }
                else
                {
                    if (units == 1)
                        result += " mốt";
                    else if (units == 5)
                        result += " lăm";
                    else
                        result += " " + Units[units];
                }
            }

            return result.Trim();
        }

        private static string GetUnit(int group)
        {
            switch (group)
            {
                case 1: return "nghìn";
                case 2: return "triệu";
                case 3: return "tỷ";
                case 4: return "nghìn tỷ";
                case 5: return "triệu tỷ";
                case 6: return "tỷ tỷ";
                default: return "";
            }
        }
    }
}
