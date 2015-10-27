using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Domain
{
    public class User
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LastName { get; set; }
        public string Handle { get; set; }
        public string Avatar { get; set; }
        public string AboutMe { get; set; }
        public string Path { get; set; }
        public int Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public int StateProvinceID { get; set; }
        public string StateProvinceCode { get; set; }
        public string CountryRegionCode { get; set; }
        public string Name { get; set; }
        public int TerritoryID { get; set; }
        public string Zip { get; set; }
        public int Country { get; set; }
        public int Age { get; set; }
        public int FinancialConcern { get; set; }
        public int Gender { get; set; }
        public int MaritalStatus { get; set; }
        public int SharesFinances { get; set; }
        public int HasKids { get; set; }
        public int NumberOfChildren { get; set; }
        public int CollegeStudent { get; set; }
        public int LevelOfCollege { get; set; }
        public int EmploymentStatus { get; set; }
        public int HomeStatus { get; set; }
        public int CreditCardDebt { get; set; }
        public int StudentLoanDebt { get; set; }
        public decimal? Income { get; set; }
        public decimal? Expenses { get; set; }
        public decimal? Savings { get; set; }
        public bool OnboardCompleted { get; set; }
        public bool EmailConfirmed { get; set; }
        public int Status { get; set; }
    }
}