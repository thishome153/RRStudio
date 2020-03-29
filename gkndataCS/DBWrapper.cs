using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netFteo.Spatial;

namespace GKNData
{
    public static class DBWrapper

        
    {
        public static TAppCfgRecord Config;

        /// <summary>
        /// Logging application activity to db
        /// </summary>
        /// <param name="item_type"></param>
        /// <param name="item_id"></param>
        /// <param name="Status"></param>
        /// <param name="Comment"></param>
        /// <param name="conn"></param>
        /// <param name="conn2"></param>
        /// <param name="distr_id"></param>
        /// <returns></returns>
        public static int DB_AppendHistory(ItemTypes item_type, long item_id, int Status, string Comment,//;Config:TAppCfgRecord);
            MySqlConnection conn)
        {
            if (conn == null) return -1; if (conn.State != System.Data.ConnectionState.Open) return 1;
            //StatusLabel_AllMessages.Text = "write log.... ";
            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            MySqlCommand cmd = new MySqlCommand(
                "INSERT INTO history(history_id,hi_disrtict_id,hi_item_type,hi_item_id," +
                 "hi_data,hi_status_id,hi_rid_id, hi_comment, hi_host, hi_ip,hi_systemusername,hi_dbusername)" +
                 " VALUES(NULL, ?hi_disrtict_id, ?hi_item_type,?hi_item_id," +
                 "?hi_data,?hi_status_id,?hi_rid_id, ?hi_comment, ?hi_host, ?hi_ip, ?hi_systemusername, ?hi_dbusername)", conn);
            cmd.Parameters.Add("?hi_item_type", MySqlDbType.Int32).Value = item_type;
            cmd.Parameters.Add("?hi_item_id", MySqlDbType.Int32).Value = item_id;
            cmd.Parameters.Add("?hi_disrtict_id", MySqlDbType.Int32).Value = Config.District_id;
            cmd.Parameters.Add("?hi_status_id", MySqlDbType.Int32).Value = Status; //int(11)
            cmd.Parameters.Add("?hi_comment", MySqlDbType.VarChar).Value = Comment + ". App v " + ver; //varchar(128)
            cmd.Parameters.Add("?hi_ip", MySqlDbType.VarChar).Value = netFteo.NetWork.NetWrapper.HostIP;
            cmd.Parameters.Add("?hi_host", MySqlDbType.VarChar).Value = netFteo.NetWork.NetWrapper.Host;
            cmd.Parameters.Add("?hi_data", MySqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("?hi_rid_id", MySqlDbType.Int32).Value = null;
            cmd.Parameters.Add("?hi_systemusername", MySqlDbType.VarChar).Value = netFteo.NetWork.NetWrapper.UserName;
            cmd.Parameters.Add("?hi_dbusername", MySqlDbType.VarChar).Value = Config.UserName;
            cmd.ExecuteNonQuery();
            return 0; // fake res
        }


        /// <summary>
        /// Add parcel record to table LOTTABLE
        /// </summary>
        public static long DB_AppendParcel(TMyParcel parcel, MySqlConnection conn)
        {
            if (conn == null) return -1; if (conn.State != System.Data.ConnectionState.Open) return 1;
            //StatusLabel_AllMessages.Text = "Adding parcel.... ";
            string lot_small_kn = parcel.CN.Split(':').Last().ToString();
            MySqlCommand cmd = new MySqlCommand(

            "INSERT INTO lottable(lottable_id,lot_kn, lot_small_kn, lotname, Code_KLADR, block_id)" + "" +
            "              VALUES(NULL,      ?lot_kn,?lot_small_kn,?lotname,?Code_KLADR,?block_id)", conn);

            //cmd.Parameters.Add("?lottable_id", MySqlDbType.Int32).Value = item_type; - set to NULL due autoIncrement by MySQL server
            cmd.Parameters.Add("?lot_kn", MySqlDbType.VarChar).Value = parcel.CN;
            cmd.Parameters.Add("?lot_small_kn", MySqlDbType.VarChar).Value = lot_small_kn;
            cmd.Parameters.Add("?lotname", MySqlDbType.VarChar).Value = "-";
            cmd.Parameters.Add("?Code_KLADR", MySqlDbType.VarChar).Value = "-";
            cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = parcel.CadastralBlock_id;
            cmd.ExecuteNonQuery();
            long last_id = cmd.LastInsertedId;
            DBWrapper.DB_AppendHistory(ItemTypes.it_Lot, last_id, 50, last_id.ToString() + " " + parcel.CN + "++", conn);
            return last_id;
        }

        public static bool DB_UpdateParcel(TMyParcel parcel, MySqlConnection conn)
        {
            if (conn == null) return false; if (conn.State != System.Data.ConnectionState.Open) return false;
            //StatusLabel_AllMessages.Text = "Update parcel.... ";
            string lot_small_kn = parcel.CN.Split(':').Last().ToString();
            MySqlCommand cmd = new MySqlCommand(

            "update lottable set " +
            "lot_kn = ?lot_kn," +
            "lot_small_kn= ?lot_small_kn," +
            "lotname = ?lotname," +
            "lot_comment = ?lot_comment" +
            //"Code_KLADR = ?Code_KLADR"+
            // "block_id = ?block_id "+
            " where lottable_id = ?lottable_id", conn);

            cmd.Parameters.Add("?lottable_id", MySqlDbType.Int32).Value = parcel.id;//
            cmd.Parameters.Add("?lot_kn", MySqlDbType.VarChar).Value = parcel.CN;
            cmd.Parameters.Add("?lot_small_kn", MySqlDbType.VarChar).Value = lot_small_kn;
            cmd.Parameters.Add("?lotname", MySqlDbType.VarChar).Value = parcel.Name;
            cmd.Parameters.Add("?lot_comment", MySqlDbType.VarChar).Value = parcel.SpecialNote;

            //cmd.Parameters.Add("?Code_KLADR", MySqlDbType.VarChar).Value = parcel.;
            //cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = parcel.CadastralBlock_id;
            cmd.ExecuteNonQuery();
            //long last_id = cmd.LastInsertedId;
            //DB_AppendHistory(ItemTypes.it_Lot, last_id, 50, last_id.ToString() + " " + parcel.CN + "++", conn, conn2, CF.Cfg.District_id, CF.Cfg);
            return true;
        }


    }
}
