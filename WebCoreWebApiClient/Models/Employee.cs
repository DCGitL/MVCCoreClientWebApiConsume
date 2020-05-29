using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreWebApiClient.Models
{
    public class Employee
    {

        public int id { get; set; }
        
        [Display(Name="First Name")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name="Last Name")]
        [Required]
        public string LastName { get; set; }

        [Required]
        public string  Gender { get; set; }
        [Required]
        public Nullable<decimal> Salary { get; set; }
    }
}
