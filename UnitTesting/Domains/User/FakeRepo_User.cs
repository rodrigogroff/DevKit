using Master.Infra.Entity.Database;
using Master.Repository;
using Npgsql;
using System.Collections.Generic;

namespace UnitTesting
{
    public class FakeRepo_User : IDapperUserRepository
    {
        #region - code - 

        User IDapperUserRepository.GetUser(NpgsqlConnection db, long? id)
        {
            return id switch
            {
                1 => new UTstUserBase().Base_Ut_user,
                2 => new UTstUserBase().Base_Ut_user_not_active,
                3 => new UTstUserBase().Base_Ut_user_not_tokenized,
                _ => null,
            };
        }

        User IDapperUserRepository.GetUserByEmail(NpgsqlConnection db, string email)
        {
            var _base = new UTstUserBase();

            if (email == _base.Base_Ut_user.stEmail) return new UTstUserBase().Base_Ut_user;
            if (email == _base.Base_Ut_user_not_active.stEmail) return new UTstUserBase().Base_Ut_user_not_active;
            if (email == _base.Base_Ut_user_not_tokenized.stEmail) return new UTstUserBase().Base_Ut_user_not_tokenized;

            return null;
        }

        User IDapperUserRepository.GetUserBySocial(NpgsqlConnection db, string sID)
        {
            if (sID == "1") return new UTstUserBase().Base_Ut_user;
            
            return null;
        }

        List<User> IDapperUserRepository.GetUsers(NpgsqlConnection db, string search)
        {
            return new List<User>
            {
                new User
                {

                }
            };
        }

        void IDapperUserRepository.InsertUser(NpgsqlConnection db, User usr)
        {

        }

        void IDapperUserRepository.UpdateUser(NpgsqlConnection db, User usr)
        {

        }

        #endregion
    }
}
