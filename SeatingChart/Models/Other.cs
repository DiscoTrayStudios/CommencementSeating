using System.ComponentModel.DataAnnotations;

namespace SeatingChart.Models
{
    public class Other
    { 
        public int ID { get; set; } 
        [Display(Name ="First Name")]  
        public string FirstName { get; set; }  
        
        [Display(Name = "Middle Name")]
        public string MiddleName {get; set;} 
        
        [Display(Name = "Last Name")] 
         public string LastName { get; set; }  

        [Display(Name = "Order")] 
         public int Order { get; set; }  
    }
}
 
    
