using QLSVNoiTru.Database;
using System.Collections.Generic;

namespace QLSVNoiTru.Models
{
    public class EPhong
    {
        public string SoHieuPhong { get; set; }
        public string MaLoaiPhong { get; set; }
        public int TangId { get; set; }
        public int SucChuaToiDa { get; set; }
        public int SoPhongDaO { get; set; }
        public LoaiPhong LoaiPhong { get; set; }
        public bool TrangThai { get; set; }
    }
    public class ETang
    {
        public int TangId { get; set; }
        public string TenTang { get; set; }
        public List<EPhong> Phongs { get; set; }
    }
}