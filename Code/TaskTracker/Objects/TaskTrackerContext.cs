using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using TaskTracker.Models;

namespace TaskTracker.Objects
{
    public class TaskTrackerContext : DbContext
    {

        public TaskTrackerContext() : base("TaskTrackerContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        public DbSet<TaskClaim> Tasks { get; set; }
        public DbSet<TaskState> TaskStates { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<Task2TaskState> TaskStateHistory { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskImportant> TaskImportants { get; set; }
        public DbSet<TaskQuickly> TaskQuicklies { get; set; }
        public DbSet<TaskCheckpoint> TaskCheckpoints { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TaskClaim>().Property(m => m.SpecialistSid).IsOptional();
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}