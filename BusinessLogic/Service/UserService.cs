using BusinessLogic.Interface;
using Repositories.Interface;
using Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using OfficeOpenXml;
using DataAccess.DAO;

namespace BusinessLogic.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task AddUserAsync(User item)
        {

                await userRepository.AddUserAsync(item);
        }

        public async Task DeleteUserAsync(int id)
        {
            await userRepository.DeleteUserAsync(id);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await userRepository.GetUserByIdAsync(id);
        }

        public async Task<IEnumerable<User>> GetListAllUsersAsync()
        {
            return await userRepository.GetAllUsersAsync();
        }

        public async Task UpdateUserAsync(User item)
        {
            await userRepository.UpdateUserAsync(item);
        }

        public async Task<List<User>> ImportFromJsonAsync(string filePath)
        {
            string json = await File.ReadAllTextAsync(filePath);
            var users = JsonConvert.DeserializeObject<List<User>>(json);
            var importedUsers = new List<User>();

            foreach (var user in users)
            {
                try
                {
                    var existingUser = await userRepository.GetUserByUsernameAsync(user.Username);
                    if (existingUser != null)
                    {
                        Console.WriteLine($"Duplicate username {user.Username} found. Skipping...");
                        continue; 
                    }

                    await userRepository.AddUserAsync(user);
                    importedUsers.Add(user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving user {user.Username}: {ex.Message}");
                }
            }

            return importedUsers;
        }
        public async Task<List<User>> ImportFromExcelAsync(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var users = new List<User>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet.Dimension == null)
                    {
                        throw new Exception("The Excel file appears to be empty.");
                    }

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var username = worksheet.Cells[row, 1].Value?.ToString();
                        var fullName = worksheet.Cells[row, 2].Value?.ToString();
                        var email = worksheet.Cells[row, 3].Value?.ToString();
                        var phone = worksheet.Cells[row, 4].Value?.ToString();
                        var role = worksheet.Cells[row, 5].Value?.ToString();
                        role = role.ToLower() == "admin" ? "Admin" : "User";
                        var isEnabledString = worksheet.Cells[row, 6].Value?.ToString();
                        var password = worksheet.Cells[row, 7].Value?.ToString();
                        bool isEnabled = isEnabledString == "Active";

                        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
                        {
                            continue;
                        }

                        var existingUser = await userRepository.GetUserByUsernameAsync(username);
                        if (existingUser != null)
                        {
                            Console.WriteLine($"Duplicate username {username} found. Skipping...");
                            continue; 
                        }

                        var user = new User
                        {
                            Username = username,
                            FullName = fullName,
                            Email = email,
                            Phone = phone,
                            Role = role,
                            Password = password,
                            IsEnabled = isEnabled
                        };

                        await userRepository.AddUserAsync(user);
                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing users from Excel file: {ex.Message}", ex);
            }

            return users;
        }


        public async Task ExportToJsonAsync(List<User> users, string filePath)
        {
            string json = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task ExportToExcelAsync(List<User> users, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                worksheet.Cells[1, 1].Value = "Username";
                worksheet.Cells[1, 2].Value = "Full Name";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Role";
                worksheet.Cells[1, 6].Value = "IsEnabled";

                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = users[i].Username;
                    worksheet.Cells[i + 2, 2].Value = users[i].FullName;
                    worksheet.Cells[i + 2, 3].Value = users[i].Email;
                    worksheet.Cells[i + 2, 4].Value = users[i].Phone;
                    worksheet.Cells[i + 2, 5].Value = users[i].Role;
                    worksheet.Cells[i + 2, 6].Value = users[i].IsEnabled == true ? "Active" : "Inactive";
                }

                await package.SaveAsync();
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            return await userRepository.AuthenticateUserAsync(username, password);
        }
        public async Task UpdatePasswordAsync(int userId, string newPassword)
        {
            await userRepository.UpdatePasswordAsync(userId, newPassword);
        }

        public async Task<User?> AuthenticateUserLoginAsync(string username, string password)
        {
            return await userRepository.AuthenticateUserLoginAsync(username, password);
        }

        public async Task<List<User>> GetAllCourtOwnerAsync()
        {
            return await userRepository.GetAllCourtOwnerAsync();
        }
    }
}
