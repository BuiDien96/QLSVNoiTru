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
    
    public partial class PhiPhong
    {
        public int PhiPhongId { get; set; }
        public string MaSinhVien { get; set; }
        public System.DateTime Thang { get; set; }
        public bool TrangThai { get; set; }
        public double SoTien { get; set; }
        public string GhiChu { get; set; }
        public System.DateTime DenThang { get; set; }
    
        public virtual SinhVien SinhVien { get; set; }
    }
}
