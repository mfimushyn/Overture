//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Overture.Hattusa.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class TeamName
    {
        public short TeamId { get; set; }
        public short Year { get; set; }
        public string Name { get; set; }
    
        public virtual Team Team { get; set; }
    }
}
