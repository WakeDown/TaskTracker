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

        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskState> TaskStates { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<Task2TaskState> TaskStateHistory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}