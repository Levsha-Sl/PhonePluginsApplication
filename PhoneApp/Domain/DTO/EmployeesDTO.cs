﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneApp.Domain.DTO
{
  public partial class EmployeesDTO : DataTransferObject
  {
    public string Name { get; set; }
    public string Phone {
      get { return _phones.Any() ? _phones.LastOrDefault().Value : "-"; } 
      set { AddPhone(value); }
    }
    private void AddPhone(string phone)
    {
      if(String.IsNullOrEmpty(phone))
      {
        throw new ArgumentNullException("Phone must be provided!");
      }

      _phones.Add(DateTime.Now, phone);
    }

    private Dictionary<DateTime, string> _phones = new Dictionary<DateTime, string>();
  }
}
