using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class EmployeeManagementBLL
    {
        QLPhongTroDataContext db;

        public EmployeeManagementBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsEmployeeCustom1> GetListEmployees()
        {
            try
            {
                var emp = from em in db.Employees
                          select new clsEmployeeCustom1
                          {
                              id = em.EmployeeID,
                              firstName = em.FirstName,
                              lastName = em.LastName,
                              fullName = $"{em.LastName} {em.FirstName}",
                              identity = em.IdentityNumber,
                              phoneNumber = em.PhoneNumber,
                              dateOfBirth = (DateTime)em.DateOfBirth,
                              address = em.Address,
                              ward = em.Ward,
                              district = em.District,
                              city = em.City,
                              fullAddress = $"{em.Address}, {em.Ward}, {em.District}, {em.City}",
                              createdAt = (DateTime)em.CreatedAt,
                              updatedAt = (DateTime)em.UpdatedAt,
                          };
                return emp.ToList();
            }
            catch (Exception)
            {
                return new List<clsEmployeeCustom1>();
            }
        }

        public List<string> GetListProvinces()
        {
            try
            {
                var provinces = from pr in db.Provinces select pr.NameProvinces;
                return provinces.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public int GetIdProvinces(string provinces)
        {
            try
            {
                var provin = from pr in db.Provinces where pr.NameProvinces.Trim().Equals(provinces) select pr.IdProvinces;
                return provin.SingleOrDefault();
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public List<string> GetListDistricts(int provinces)
        {
            try
            {
                var districts = from di in db.Districts where di.IdProvinces.Equals(provinces) select di.NameDistricts;
                return districts.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public int GetIdDistrict(string districts)
        {
            try
            {
                var distr = from pr in db.Districts where pr.NameDistricts.Trim().Equals(districts) select pr.IdDistricts;
                return distr.SingleOrDefault();
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public List<string> GetListWard(int districts)
        {
            try
            {
                var ward = from di in db.Wards where di.IdDistricts.Equals(districts) select di.NameWards;
                return ward.ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public string CreateNewIdEmployee()
        {
            try
            {
                string lastEmployeeId = db.Employees.OrderByDescending(c => c.EmployeeID).Select(c => c.EmployeeID.Trim()).FirstOrDefault();

                if (string.IsNullOrEmpty(lastEmployeeId))
                {
                    return "NV001";
                }

                string numericPart = lastEmployeeId.Substring(2);
                int currentId = int.Parse(numericPart);

                currentId++;

                string newEmployeeId = "NV" + currentId.ToString("D3");

                return newEmployeeId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating new ID: " + ex.Message);
            }
        }

        public clsImageIdentityEmployee GetImageEmployee(string idEmployee)
        {
            try
            {
                var cus = (from cs in db.Employees
                           where cs.EmployeeID.Trim().Equals(idEmployee)
                           select new clsImageIdentityEmployee
                           {
                               img1 = cs.INDENTIFY_FIRST_IMG != null ? cs.INDENTIFY_FIRST_IMG.ToArray() : null,
                               img2 = cs.INDENTIFY_BACK_IMG != null ? cs.INDENTIFY_BACK_IMG.ToArray() : null
                           }).FirstOrDefault();

                if (cus == null)
                {
                    return new clsImageIdentityEmployee();
                }

                return cus;
            }
            catch (Exception)
            {
                return new clsImageIdentityEmployee();
            }
        }

        public bool InsertData(string id, string firstName, string lastName, string identity, string phone, DateTime dateTime, string address,
            string ward, string district, string city, byte[] img1, byte[] img2)
        {
            try
            {
                var newEmployee = new Employee
                {
                    EmployeeID = id,
                    FirstName = firstName,
                    LastName = lastName,
                    IdentityNumber = identity,
                    PhoneNumber = phone,
                    DateOfBirth = dateTime,
                    Address = address,
                    Ward = ward,
                    District = district,
                    City = city,
                    INDENTIFY_FIRST_IMG = img1,
                    INDENTIFY_BACK_IMG = img2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.Employees.InsertOnSubmit(newEmployee);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteEmployee(string id)
        {
            try
            {
                var emp = db.Employees.SingleOrDefault(cs => cs.EmployeeID == id);

                if (emp != null)
                {
                    db.Employees.DeleteOnSubmit(emp);
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateEmployee(
            string id, string firstname, string lastname, string identity, string phonenumber, DateTime dateofbirth,
            string address, string ward, string district, string city, byte[] img1, byte[] img2)
        {
            try
            {
                var employee = db.Employees.SingleOrDefault(r => r.EmployeeID == id);

                if (employee == null)
                    return false;

                employee.FirstName = firstname;
                employee.LastName = lastname;
                employee.IdentityNumber = identity;
                employee.PhoneNumber = phonenumber;
                employee.DateOfBirth = dateofbirth;
                employee.Address = address;
                employee.Ward = ward;
                employee.District = district;
                employee.City = city;
                employee.INDENTIFY_FIRST_IMG = img1;
                employee.INDENTIFY_BACK_IMG = img2;
                employee.UpdatedAt = DateTime.Now;

                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
