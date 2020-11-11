﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentistryManagement.Server.DataTransferObjects
{
    public class AffiliateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AddressDTO AddressDTO { get; set; }
    }
}
