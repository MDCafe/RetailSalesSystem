﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RetailManagementSystem
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RMSEntities: DbContext
    {
        public RMSEntities()
            : base("name=RMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ApplicationDetail> ApplicationDetails { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CodeMaster> CodeMasters { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<MeasuringUnit> MeasuringUnits { get; set; }
        public virtual DbSet<PriceDetail> PriceDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<PurchaseReturn> PurchaseReturns { get; set; }
        public virtual DbSet<ReturnDamagedStock> ReturnDamagedStocks { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SaleDetail> SaleDetails { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleTemp> SaleTemps { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<PurchaseFreeDetail> PurchaseFreeDetails { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<ChequePaymentDetail> ChequePaymentDetails { get; set; }
        public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }
        public virtual DbSet<DirectPaymentDetail> DirectPaymentDetails { get; set; }
        public virtual DbSet<SystemData> SystemDatas { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual DbSet<StockTransaction> StockTransactions { get; set; }
        public virtual DbSet<ExpenseDetail> ExpenseDetails { get; set; }
        public virtual DbSet<PurchaseChequePaymentDetail> PurchaseChequePaymentDetails { get; set; }
        public virtual DbSet<PurchasePaymentDetail> PurchasePaymentDetails { get; set; }
        public virtual DbSet<StockAdjustment> StockAdjustments { get; set; }
    }
}
