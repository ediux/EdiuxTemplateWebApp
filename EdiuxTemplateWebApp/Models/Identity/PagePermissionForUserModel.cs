using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdiuxTemplateWebApp.Models.Identity
{
    [Serializable]
    public class PagePermissionForUserModel
    {
        public PagePermissionForUserModel()
        {
            ReadData = true;
            WriteData = false;
            ExecuteFeature = true;
            SharedView = false;
            IsErrorPage = false;
            CanUpload = false;
        }

        public bool ReadData { get; set; }

        public bool WriteData { get; set; }

        public bool ExecuteFeature { get; set; }

        public bool SharedView { get; set; }

        public bool IsErrorPage { get; set; }

        public bool CanUpload { get; set; }
    }
}