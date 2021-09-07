using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace tallerazurefunctions.Functions.Entities
{
    public class ConsolidatedEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime Date { get; set; }

        public int Minutes { get; set; }
    }
}
