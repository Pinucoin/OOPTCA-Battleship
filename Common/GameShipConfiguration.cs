//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Common
{
    using System;
    using System.Collections.Generic;
    
    public partial class GameShipConfiguration
    {
        public int configId { get; set; }
        public Nullable<int> playerFK { get; set; }
        public Nullable<int> gameFK { get; set; }
        public Nullable<int> shipFK { get; set; }
        public string coordinate { get; set; }
    
        public virtual Game Game { get; set; }
        public virtual Player Player { get; set; }
        public virtual Ship Ship { get; set; }
    }
}
