using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EnjuAihara.EntityFramework
{
    public partial class QuanLyPhongMachWibuEntities : DbContext
    {
        //public override int SaveChanges()
        //{
        //    if (GetCurrentUser() != null)
        //    {
        //        ChangeDataLogModels.AddRange(GetLogList());
        //    }
        //    return base.SaveChanges();
        //}

        public Guid? GetCurrentUser()
        {
            string user = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(user))
            {
                return null;
            }
            return Guid.Parse(user);
        }

        public List<ChangeDataLogModel> GetLogList()
        {
            List<ChangeDataLogModel> LogList = new List<ChangeDataLogModel>();
            var changeTrack = ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified);
            foreach (var entry in changeTrack)
            {
                if (entry.Entity != null)
                {
                    string entityName = string.Empty;
                    string state = string.Empty;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entityName = ObjectContext.GetObjectType(entry.Entity.GetType()).Name;
                            state = "Sửa dữ liệu";
                            foreach (string prop in entry.OriginalValues.PropertyNames)
                            {
                                object currentValue = entry.CurrentValues[prop];
                                object originalValue = entry.OriginalValues[prop];
                                if (!currentValue.Equals(originalValue))
                                {
                                    LogList.Add(new ChangeDataLogModel
                                    {
                                        TableName = entityName,
                                        ChangeType = state,
                                        FieldName = prop,
                                        OldData = Convert.ToString(originalValue),
                                        NewData = Convert.ToString(currentValue),
                                        LastEditTime = DateTime.Now,
                                        LogId = Guid.NewGuid(),
                                        LastEditBy = GetCurrentUser()
                                    });
                                }
                            }
                            break;
                        case EntityState.Added:
                            entityName = ObjectContext.GetObjectType(entry.Entity.GetType()).Name;
                            state = "Thêm mới dữ liệu";
                            foreach (string prop in entry.CurrentValues.PropertyNames)
                            {
                                LogList.Add(new ChangeDataLogModel
                                {
                                    TableName = entityName,
                                    ChangeType = state,
                                    FieldName = prop,
                                    OldData = string.Empty,
                                    NewData = Convert.ToString(entry.CurrentValues[prop]),
                                    LastEditTime = DateTime.Now,
                                    LogId = Guid.NewGuid(),
                                    LastEditBy = GetCurrentUser()
                                });

                            }
                            break;
                        case EntityState.Deleted:
                            entityName = ObjectContext.GetObjectType(entry.Entity.GetType()).Name;
                            state = "Xóa dữ liệu";
                            foreach (string prop in entry.OriginalValues.PropertyNames)
                            {
                                LogList.Add(new ChangeDataLogModel
                                {
                                    TableName = entityName,
                                    ChangeType = state,
                                    FieldName = prop,
                                    OldData = Convert.ToString(entry.OriginalValues[prop]),
                                    NewData = string.Empty,
                                    LastEditTime = DateTime.Now,
                                    LogId = Guid.NewGuid(),
                                    LastEditBy = GetCurrentUser()
                                });

                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return LogList;
        }


        public string GetPrimaryKey(DbEntityEntry entry)
        {

            var setBase = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity).EntitySet;
            string[] keyNames = setBase.ElementType.KeyMembers.Select(k => k.Name).ToArray();
            string keyName = string.Empty;
            if (keyNames != null)
                keyName = keyNames.FirstOrDefault().ToString();
            if (!string.IsNullOrEmpty(keyName))
                return keyName;
            return string.Empty;
        }

    }
}
