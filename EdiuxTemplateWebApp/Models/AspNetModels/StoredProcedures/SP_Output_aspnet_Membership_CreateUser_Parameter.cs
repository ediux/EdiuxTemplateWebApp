﻿using System;

namespace EdiuxTemplateWebApp.Models.AspNetModels
{
    public partial class aspnet_Membership_CreateUser_OutputParameter
    {
        [OutputParameter]
        public virtual Guid Guid { get; set; }
        [OutputParameter]
        public virtual dynamic CreateStatus { get; set; }
    }
}