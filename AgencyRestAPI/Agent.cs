//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AgencyRestAPI
{
    using System;
    using System.Collections.Generic;
    
    public partial class Agent
    {
        public int Entry_No { get; set; }
        public string Agent_Code { get; set; }
        public string Bank_No { get; set; }
        public Nullable<System.DateTime> Date_Registered { get; set; }
        public Nullable<System.DateTime> Time_Registred { get; set; }
        public string Registered_By { get; set; }
        public string Location { get; set; }
        public Nullable<bool> Active { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Telephone { get; set; }
        public Nullable<bool> Password_Changed { get; set; }
        public Nullable<bool> Branch { get; set; }
    }
}
