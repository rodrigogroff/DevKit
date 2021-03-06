﻿using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DataModel
{
    public partial class TaskType
    {
        public bool Update_NewFlow(DevKitDB db, User user, ref string resp)
        {
            var ent = JsonConvert.DeserializeObject<TaskFlow>(anexedEntity.ToString());

            if (ent.id == 0)
            {
                if ((from e in db.TaskFlow
                     where e.fkTaskCategory == ent.fkTaskCategory
                     where e.fkTaskType == ent.fkTaskType
                     where e.stName.ToUpper() == ent.stName.ToUpper()
                     select e).Any())
                {
                    resp = "Fluxo já adicionado ao tipo de tarefa";
                    return false;
                }

                ent.fkTaskType = id;

                db.Insert(ent);

                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.CategoryAddFlow,
                    nuType = EnumAuditType.TaskType,
                    fkTarget = this.id
                }.
                Create(db, "Novo fluxo: " + ent.stName, "");
            }
            else
            {
                db.Update(ent);

                new AuditLog
                {
                    fkUser = user.id,
                    fkActionLog = EnumAuditAction.CategoryUpdateFlow,
                    nuType = EnumAuditType.TaskType,
                    fkTarget = this.id
                }.
                Create(db, "Edição de fluxo: " + ent.stName, "");
            }

            return true;
        }
    }
}
