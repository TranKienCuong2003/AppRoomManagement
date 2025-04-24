using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CustomerManagementBLL
    {
        QLPhongTroDataContext db;

        public CustomerManagementBLL()
        {
            db = new QLPhongTroDataContext();
        }

        public List<clsCustomerCustom1> GetListCustomer()
        {
            try
            {
                var cus = from cs in db.Customers
                            select new clsCustomerCustom1()
                            {
                                id = cs.CustomerID.Trim(),
                                fullname = $"{cs.LastName.Trim()} {cs.FirstName.Trim()}",
                                firstname = cs.FirstName.Trim(),
                                lastname = cs.LastName.Trim(),
                                identify = cs.IdentityNumber.Trim(),
                                sdt = cs.PhoneNumber.Trim(),
                                dateofbirth = (DateTime)cs.DateOfBirth,
                                addressfull = $"{cs.Address.Trim()}, {cs.Ward.Trim()}, {cs.District.Trim()}, {cs.City.Trim()}",
                                address = cs.Address.Trim(),
                                ward = cs.Ward.Trim(),
                                district = cs.District.Trim(),
                                city = cs.City.Trim(),
                                created = (DateTime)cs.CreatedAt,
                                updated = (DateTime)cs.UpdatedAt,
                            };
                return cus.ToList();
            }
            catch (Exception)
            {
                return new List<clsCustomerCustom1>();
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

        public string CreateNewIdCustomer()
        {
            try
            {
                string lastCustomerId = db.Customers.OrderByDescending(c => c.CustomerID).Select(c => c.CustomerID.Trim()).FirstOrDefault();

                if (string.IsNullOrEmpty(lastCustomerId))
                {
                    return "KH000001";
                }

                string numericPart = lastCustomerId.Substring(2);
                int currentId = int.Parse(numericPart);

                currentId++;

                string newCustomerId = "KH" + currentId.ToString("D6");

                return newCustomerId;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (nếu có)
                throw new Exception("Error generating new customer ID: " + ex.Message);
            }
        }

        public clsImageIdentityCustomer GetImageCustomer(string idCustomer)
        {
            try
            {
                var cus = (from cs in db.Customers
                           where cs.CustomerID.Trim().Equals(idCustomer)
                           select new clsImageIdentityCustomer
                           {
                               img1 = cs.INDENTIFY_FIRST_IMG != null ? cs.INDENTIFY_FIRST_IMG.ToArray() : null,
                               img2 = cs.INDENTIFY_BACK_IMG != null ? cs.INDENTIFY_BACK_IMG.ToArray() : null
                           }).FirstOrDefault();

                if (cus == null)
                {
                    return new clsImageIdentityCustomer();
                }

                return cus;
            }
            catch (Exception ex)
            {
                return new clsImageIdentityCustomer();
            }
        }

        public bool InsertCustomer(
            string idCustomer, string firstname, string lastname, string identity, string phonenumber, DateTime dateofbirth,
            string address, string ward, string district, string city, byte[] img1, byte[] img2)
        {
            try
            {
                var newCustomer = new Customer
                {
                    CustomerID = idCustomer,
                    FirstName = firstname,
                    LastName = lastname,
                    IdentityNumber = identity,
                    PhoneNumber = phonenumber,
                    DateOfBirth = dateofbirth,
                    Address = address,
                    Ward = ward,
                    District = district,
                    City = city,
                    INDENTIFY_FIRST_IMG = img1,
                    INDENTIFY_BACK_IMG = img2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                db.Customers.InsertOnSubmit(newCustomer);
                db.SubmitChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteCustomer(string idCustomer)
        {
            try
            {
                var cus = db.Customers.SingleOrDefault(cs => cs.CustomerID == idCustomer);

                if (cus != null)
                {
                    db.Customers.DeleteOnSubmit(cus);
                    db.SubmitChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateCustomer(
            string idCustomer, string firstname, string lastname, string identity, string phonenumber, DateTime dateofbirth,
            string address, string ward, string district, string city, byte[] img1, byte[] img2)
        {
            try
            {
                var customer = db.Customers.SingleOrDefault(r => r.CustomerID == idCustomer);

                if (customer == null)
                    return false;

                customer.FirstName = firstname;
                customer.LastName = lastname;
                customer.IdentityNumber = identity;
                customer.PhoneNumber = phonenumber;
                customer.DateOfBirth = dateofbirth;
                customer.Address = address;
                customer.Ward = ward;
                customer.District = district;
                customer.City = city;
                customer.INDENTIFY_FIRST_IMG = img1;
                customer.INDENTIFY_BACK_IMG = img2;
                customer.UpdatedAt = DateTime.Now;

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
