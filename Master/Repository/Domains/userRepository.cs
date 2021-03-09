using Dapper;
using Master.Infra.Entity.Database;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Master.Repository
{
    public interface IDapperUserRepository
    {
        User GetUserByEmail(NpgsqlConnection db, string email);
        List<User> GetUsers(NpgsqlConnection db, string search);
        User GetUser(NpgsqlConnection db, long? id);
        void InsertUser(NpgsqlConnection db, User usr);
        void UpdateUser(NpgsqlConnection db, User usr);
        User GetUserBySocial(NpgsqlConnection db, string sID);
    }

    public class DapperUserRepository : IDapperUserRepository
    {
        public void InsertUser(NpgsqlConnection db, User usr)
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO \"User\"(\"stEmail\",\"stName\",\"bActive\",\"bAdmin\",\"bTokenized\",\"stSocialID\",\"stPassword\",\"dtLastLogin\",\"dtCreation\",\"stToken\",\"dtTokenExpires\"," +
                "\"nuTrig1_Email\",\"nuTrig2_Email\",\"nuTrig3_Email\",\"nuTrig4_Email\",\"nuTrig5_Email\",\"nuTrig6_Email\",\"nuTrig7_Email\",\"nuTrig8_Email\",\"nuTrig9_Email\",\"nuTrig10_Email\"," +
                "\"nuTrig1_SocialID\",\"nuTrig2_SocialID\",\"nuTrig3_SocialID\",\"nuTrig4_SocialID\") " +
                "VALUES(@stEmail,@stName,@bActive,@bAdmin,@bTokenized,@stSocialID,@stPassword,@dtLastLogin,@dtCreation,@stToken,@dtTokenExpires," +
                "@nuTrig1_Email,@nuTrig2_Email,@nuTrig3_Email,@nuTrig4_Email,@nuTrig5_Email,@nuTrig6_Email,@nuTrig7_Email,@nuTrig8_Email,@nuTrig9_Email,@nuTrig10_Email," +
                "@nuTrig1_SocialID,@nuTrig2_SocialID,@nuTrig3_SocialID,@nuTrig4_SocialID);", db))
            {
                cmd.Parameters.AddWithValue("stEmail", ((object)usr.stEmail) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stName", ((object)usr.stName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bActive", ((object)usr.bActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bAdmin", ((object)usr.bAdmin) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bTokenized", ((object)usr.bTokenized) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stSocialID", ((object)usr.stSocialID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stPassword", ((object)usr.stPassword) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtLastLogin", ((object)usr.dtLastLogin) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtCreation", ((object)usr.dtCreation) ?? DBNull.Value); 
                cmd.Parameters.AddWithValue("stToken", ((object)usr.stToken) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtTokenExpires", ((object)usr.dtTokenExpires) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig1_Email", ((object)usr.nuTrig1_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig2_Email", ((object)usr.nuTrig2_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig3_Email", ((object)usr.nuTrig3_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig4_Email", ((object)usr.nuTrig4_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig5_Email", ((object)usr.nuTrig5_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig6_Email", ((object)usr.nuTrig6_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig7_Email", ((object)usr.nuTrig7_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig8_Email", ((object)usr.nuTrig8_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig9_Email", ((object)usr.nuTrig9_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig10_Email", ((object)usr.nuTrig10_Email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig1_SocialID", ((object)usr.nuTrig1_SocialID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig2_SocialID", ((object)usr.nuTrig2_SocialID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig3_SocialID", ((object)usr.nuTrig3_SocialID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("nuTrig4_SocialID", ((object)usr.nuTrig4_SocialID) ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateUser(NpgsqlConnection db, User usr)
        {
            using (var cmd = new NpgsqlCommand("update \"User\" " +
                "set \"stEmail\" = @stEmail, " +
                "\"stName\" = @stName, " +
                "\"bActive\" = @bActive, " +
                "\"bAdmin\" = @bAdmin, " +
                "\"bTokenized\" = @bTokenized, " +
                "\"stSocialID\" = @stSocialID, " +
                "\"stPassword\" = @stPassword, " +
                "\"dtLastLogin\" = @dtLastLogin, " +
                "\"dtCreation\" = @dtCreation, " +
                "\"dtTokenExpires\" = @dtTokenExpires, " +
                "\"stToken\" = @stToken " +
                "where id=@id", db))
            {
                cmd.Parameters.AddWithValue("stEmail", ((object)usr.stEmail) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stName", ((object)usr.stName) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bActive", ((object)usr.bActive) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bAdmin", ((object)usr.bAdmin) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("bTokenized", ((object)usr.bTokenized) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stSocialID", ((object)usr.stSocialID) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stPassword", ((object)usr.stPassword) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtLastLogin", ((object)usr.dtLastLogin) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtCreation", ((object)usr.dtCreation) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("dtTokenExpires", ((object)usr.dtTokenExpires) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("stToken", ((object)usr.stToken) ?? DBNull.Value);

                cmd.Parameters.AddWithValue("id", usr.id);

                cmd.ExecuteNonQuery();
            }
        }

        public User GetUserByEmail(NpgsqlConnection db, string email)
        {
            var u = new User { stEmail = email };
            u.compile_email_trigs();

            return db.QueryFirstOrDefault<User>("select * from \"User\" where \"stEmail\" = '" + email + "' and " + u.GetEmailTrigsWhere() );
        }

        public User GetUserBySocial(NpgsqlConnection db, string sID)
        {
            var u = new User { stSocialID = sID };
            u.compile_socialID_trigs();

            return db.QueryFirstOrDefault<User>("select * from \"User\" where \"stSocialID\" = '" + sID + "' and " + u.GetSocialIDTrigsWhere());
        }

        public List<User> GetUsers(NpgsqlConnection db, string search)
        {
            if (search == null)
                return db.Query<User>("select * from \"User\" order by \"stLogin\"").ToList();
            else
                return db.Query<User>("select * from \"User\" order by \"stLogin\"").Where(y => y.stName.Contains(search)).ToList();
        }

        public User GetUser(NpgsqlConnection db, long? id)
        {
            return db.Query<User>("select * from \"User\"").
                    Where(y => y.id == id).
                    FirstOrDefault();
        }
    }
}
