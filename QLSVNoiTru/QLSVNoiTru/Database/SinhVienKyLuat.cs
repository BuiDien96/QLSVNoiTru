//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLSVNoiTru.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class SinhVienKyLuat
    {
        public int SinhVienKyLuatId { get; set; }
        public int MaKyLuat { get; set; }
        public string MaSinhVien { get; set; }
        public bool Chon { get; set; }
    
        public virtual KyLuat KyLuat { get; set; }
        public virtual SinhVien SinhVien { get; set; }
    }
}
