using FastEvents.dbo;
using System;
using System.ComponentModel.DataAnnotations;

namespace FastEvents.Models
{
    public class CreateOrEditViewModel
    {
        public EventUi EventUi { get; set; }
        public bool IsCreate { get; set; }
        
        public string Error { get; set; }
    }
}