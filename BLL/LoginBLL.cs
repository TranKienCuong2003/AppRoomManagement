using DTO;
using System;
using System.Linq;

namespace BLL
{
    public class LoginBLL
    {
        QLPhongTroDataContext db;

        public LoginBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public bool GetLogin(string username, string password)
        {
            try
            {
                string hashedPassword = (from ec in db.EmployeeAccounts
                                         where ec.Username.Trim() == username
                                         select ec.PasswordHash).FirstOrDefault();

                if (hashedPassword == null)
                {
                    return false;
                }

                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                return isPasswordCorrect;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetIdEmployee(string username)
        {
            try
            {
                return (from ac in db.EmployeeAccounts 
                        where ac.Username.Trim().Equals(username)
                        select ac.EmployeeID).FirstOrDefault().ToString().Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public clsGetInfoAfterLogin GetInfoAfterLogin(string username)
        {
            try
            {
                var info = from ac in db.EmployeeAccounts
                           where ac.Username.Trim().Equals(username)
                           select new clsGetInfoAfterLogin
                           {
                               idEmployee = ac.EmployeeID,
                               status = (int)ac.Status,
                               role = ac.Role,
                           };
                return info.FirstOrDefault();
            }
            catch (Exception)
            {
                return new clsGetInfoAfterLogin();
            }
        }
    }
}
