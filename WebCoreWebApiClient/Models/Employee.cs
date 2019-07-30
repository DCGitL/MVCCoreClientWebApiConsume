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
        [Required(ErrorMessage ="This is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "This is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This is required")]
        public string  Gender { get; set; }
        [Required(ErrorMessage = "This is required")]
        public Nullable<decimal> Salary { get; set; }
    }
}
