﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Eshop
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<box> boxes { get; set; }
        public virtual DbSet<cart> carts { get; set; }
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<Connection> Connections { get; set; }
        public virtual DbSet<Contactform> Contactforms { get; set; }
        public virtual DbSet<cpu> cpus { get; set; }
        public virtual DbSet<Desktop> Desktops { get; set; }
        public virtual DbSet<favourite> favourites { get; set; }
        public virtual DbSet<gpu> gpus { get; set; }
        public virtual DbSet<hardDisc> hardDiscs { get; set; }
        public virtual DbSet<monitor> monitors { get; set; }
        public virtual DbSet<motherboard> motherboards { get; set; }
        public virtual DbSet<Online> Onlines { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<order> orders { get; set; }
        public virtual DbSet<payment> payments { get; set; }
        public virtual DbSet<product> products { get; set; }
        public virtual DbSet<psu> psus { get; set; }
        public virtual DbSet<ram> rams { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Shipping> Shippings { get; set; }
        public virtual DbSet<SupportMessage> SupportMessages { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UserRoleMapping> UserRoleMappings { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
