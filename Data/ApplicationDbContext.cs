using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaMVC.Models;

namespace WaMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<FuncionarioTecnologia> FuncionarioTecnologias { get; set; }
        public DbSet<GFT> GFTs { get; set; }
        public DbSet<Tecnologia> Tecnologias { get; set; }
        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<VagaTecnologia> VagaTecnologias { get; set; }
        public DbSet<Alocar> Aloc { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
