﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DB : DbContext
    {
        public DB()
            : base("name=DB")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<GiaDien> GiaDiens { get; set; }
        public virtual DbSet<GiaNuoc> GiaNuocs { get; set; }
        public virtual DbSet<HoaDonDienNuoc> HoaDonDienNuocs { get; set; }
        public virtual DbSet<Khoa> Khoas { get; set; }
        public virtual DbSet<KyLuat> KyLuats { get; set; }
        public virtual DbSet<KyTucXa> KyTucXas { get; set; }
        public virtual DbSet<LoaiMauBieu> LoaiMauBieux { get; set; }
        public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }
        public virtual DbSet<Lop> Lops { get; set; }
        public virtual DbSet<MauBieu> MauBieux { get; set; }
        public virtual DbSet<PhiPhong> PhiPhongs { get; set; }
        public virtual DbSet<Phong> Phongs { get; set; }
        public virtual DbSet<PhongThietBi> PhongThietBis { get; set; }
        public virtual DbSet<Quyen> Quyens { get; set; }
        public virtual DbSet<SinhVien> SinhViens { get; set; }
        public virtual DbSet<SinhVienChuyenPhong> SinhVienChuyenPhongs { get; set; }
        public virtual DbSet<SinhVienHoatDong> SinhVienHoatDongs { get; set; }
        public virtual DbSet<SinhVienKyLuat> SinhVienKyLuats { get; set; }
        public virtual DbSet<SinhVienOLai> SinhVienOLais { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Tang> Tangs { get; set; }
        public virtual DbSet<ThietBi> ThietBis { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<BieuMau> BieuMaus { get; set; }
        public virtual DbSet<SinhVienTienPhong> SinhVienTienPhongs { get; set; }
    }
}
