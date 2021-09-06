using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace tallerazurefuntions.Functions.Entities
{
    public class RegistrationEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime Date { get; set; }

        public int Type { get; set; }

        public bool Consolidated { get; set; }
    }
}
