using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCMusicStore.Models
{
    public class Order
    {
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }
        [ScaffoldColumn(false)]
        public DateTime OrderDate { get; set; }
        [ScaffoldColumn(false)]
        public string UserName { get; set; }
        [MaxLength(160), Required(ErrorMessage = "First Name is required"),
            Display(Name = "First Name")]
        public string FirstName { get; set; }
        [MaxLength(160), Required(ErrorMessage = "Last Name is requires"),
            Display(Name = "Last Name")]
        public string LastName { get; set; }
        [MaxLength(70), Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        [MaxLength(40), Required(ErrorMessage = "City is required.")]
        public string City { get; set; }
        [MaxLength(40), Required(ErrorMessage = "State is requires.")]
        public string State { get; set; }
        [MaxLength(10), Required(ErrorMessage = "Postal Code is required."),
            Display(Name = "Postal Code"), RegularExpression(@"^\d{5}(-\d{4})?$", 
            ErrorMessage = "Invalid Postal Code. Use 5 digits or 5 digits dash 4 digits.")]
        public string PostalCode { get; set; }
        [MaxLength(40), Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }
        [MaxLength(24), Required(ErrorMessage = "Phone is required."),
            DataType(DataType.PhoneNumber), RegularExpression(@"^\(?([0-9]{2})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
            ErrorMessage = "Phone number is not valid. Proper format is (XX)-XXX-XXXX")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Email Address is required"), 
            Display(Name = "Email Address"), EmailAddress(ErrorMessage = "Email Address is not valid.")]
        public string Email { get; set; }
        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}
