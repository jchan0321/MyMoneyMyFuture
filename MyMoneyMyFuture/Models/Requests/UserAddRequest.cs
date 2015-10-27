using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMoneyMyFuture.Models.Requests
{
    public class UserAddRequest
    {
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Zip { get; set; }

        [Required]
        public int Income { get; set; }

        [Required]
        public int Expenses { get; set; }

        public int Savings { get; set; }

        [Required]
        public int Gender { get; set; }

        [Required]
        public int Age { get; set; }

        public int HasKids { get; set; }

        public int CollegeStudent { get; set; }

        //public int? NumberOfChildren { get; set; }

        [Required]
        public int MaritalStatus { get; set; }

        [Required]
        public int SharesFinances { get; set; }

        [Required]
        public int EmploymentStatus { get; set; }

        [Required]
        public int CreditCardDebt { get; set; }

        [Required]
        public int StudentLoanDebt { get; set; }

        [Required]
        public int HomeStatus { get; set; }

        [Required]
        public int FinancialConcern { get; set; }
    }
}