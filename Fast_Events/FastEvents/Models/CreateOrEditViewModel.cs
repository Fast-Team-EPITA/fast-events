using FastEvents.dbo;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FastEvents.Models
{
    public class CreateOrEditViewModel
    {
        public EventUi EventUi { get; set; }
        public bool IsCreate { get; set; }
        public IFormFile PictureFile { get; set; }
    }
}