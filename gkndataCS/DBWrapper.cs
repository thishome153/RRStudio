using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Text;
using netFteo.Spatial;
using netFteo.Cadaster;

namespace GKNData
{
    /*
    public enum ViewMode
    {
        vmExplorer =1,
        vmBlockList =2,
        vmFavorites =3,
        vmHistory =4
    }

    */
    /// <summary>
    /// Enumerate levels of viewing contents
    /// </summary>
    public enum ViewLevel
    {
        /// <summary>
        /// Displaying cadastral subjects collection of Russian Federation 
        /// </summary>
        vlExploreSubRF = 0,
        /// <summary>
        /// Displaying cadastral districts collection of appropriate cadastral subject (subRF)
        /// </summary>
        vlExploreDistricts = 1,
        /// <summary>
        /// Displaying cadastral blocks collection of appropriate district
        /// </summary>
        vlBlocks =2,
        /// <summary>
        /// Displaying cadastral blocks collection from favorites list
        /// </summary>
        vlFavorites = 3,
        /// <summary>
        /// /// Displaying cadastral blocks collection from history list
        /// </summary>
        vlHistory = 4
    }

    public static class DBWrapper

        
    {
        public static TAppCfgRecord Config;
        /// <summary>
        /// Stored messages from exceptions handlers, etc
        /// </summary>
        public static string LastErrorMsg;
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
        public static int DB_AppendHistory(ItemTypes item_type, long item_id, DBLogRecordStatus Status, string Comment,//;Config:TAppCfgRecord);
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
            cmd.Parameters.Add("?hi_comment", MySqlDbType.VarChar).Value = Comment;
            cmd.Parameters.Add("?hi_ip", MySqlDbType.VarChar).Value = netFteo.NetWork.NetWrapper.HostIP;
            cmd.Parameters.Add("?hi_host", MySqlDbType.VarChar).Value = netFteo.NetWork.NetWrapper.Host;
            cmd.Parameters.Add("?hi_data", MySqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("?hi_rid_id", MySqlDbType.Int32).Value = null;
            cmd.Parameters.Add("?hi_systemusername", MySqlDbType.VarChar).Value = netFteo.NetWork.NetWrapper.UserName;
            cmd.Parameters.Add("?hi_dbusername", MySqlDbType.VarChar).Value = Config.UserName;
            cmd.ExecuteNonQuery();
            return 0; // fake res
        }

        public static TFileHistory LoadBlockHistory(MySqlConnection conn, long block_id)
        {
            TFileHistory files = new TFileHistory(block_id);
            if (conn == null) return null; if (conn.State != System.Data.ConnectionState.Open) return null;
             DataTable data  = new DataTable();
             MySqlDataAdapter da = new MySqlDataAdapter("SELECT *" +
                                      " from history where hi_item_id = " + block_id.ToString() +
                                      " order by history_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFileHistoryItem file = new TFileHistoryItem(Convert.ToInt32(row[0])); //id
                /*
                 1`hi_disrtict_id`,
                 2`hi_item_type`, 
                 3`hi_item_id`, 
                 4`hi_data`, 
                 5`hi_status_id`, 
                 6`hi_rid_id`, 
                 7 `hi_comment`, `hi_host`, `hi_ip`, 
                 10 `hi_systemusername`, `hi_dbusername
                 * */
                file.hi_item_id = "Type(" + row[2].ToString() + ").id " + row[3].ToString();
                file.hi_data = Convert.ToString(row[4]).Substring(0, Convert.ToString(row[4]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.hi_comment = row[7].ToString();
                file.hi_host = row[8].ToString();
                file.hi_ip = row[9].ToString();
                file.hi_systemusername = row[10].ToString();
                file.hi_dbusername = row[11].ToString();
                files.Add(file);
            }
            return files;
        }

        /*TODO KILL
        public static TFileHistory LoadParcelHistory(MySqlConnection conn, long item_id)
        {
            TFileHistory files = new TFileHistory(item_id);
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            DataTable data = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT *" +
                                      " from history where hi_item_id = " + item_id.ToString() +
                                      " order by history_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFileHistoryItem file = new TFileHistoryItem(Convert.ToInt32(row[0])); //id
                file.hi_item_id = "Type(" + row[2].ToString() + ").id " + row[3].ToString();
                file.hi_data = Convert.ToString(row[4]).Substring(0, Convert.ToString(row[4]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.hi_comment = row[7].ToString();
                file.hi_host = row[8].ToString();
                file.hi_ip = row[9].ToString();
                file.hi_systemusername = row[10].ToString();
                file.hi_dbusername = row[11].ToString();
                files.Add(file);
            }
            return files;
        }

        */

        public static long DB_AppendSubRF(TCadasterItem SubRF, MySqlConnection conn)
        {
            if (conn == null) return -1; if (conn.State != System.Data.ConnectionState.Open) return 1;
            //long last_id = cmd.LastInsertedId;
            //return last_id;
            throw new System.NotImplementedException();
        }

        public static long DB_AppendDistrict(TCadastralDistrict district, MySqlConnection conn)
        {
            if (conn == null) return -1; if (conn.State != System.Data.ConnectionState.Open) return 1;
            MySqlCommand cmd = new MySqlCommand(

                "INSERT INTO districts(district_id, district_kn, district_Name, subrf_id)" + "" +
                "              VALUES(NULL,  ?district_kn, ?district_Name, ?subrf_id)", conn);

            //cmd.Parameters.Add("?district_id", MySqlDbType.Int32).Value = item_type; - set to NULL due autoIncrement by MySQL server
            cmd.Parameters.Add("?district_kn", MySqlDbType.VarChar).Value = district.CN;
            cmd.Parameters.Add("?district_Name", MySqlDbType.VarChar).Value = district.Name;
            cmd.Parameters.Add("?subrf_id", MySqlDbType.Int32).Value = district.SubRF_id;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return -1;
            }
            long last_id = cmd.LastInsertedId;
            district.id = last_id;
            DBWrapper.DB_AppendHistory(ItemTypes.it_District, last_id, DBLogRecordStatus.it_Insert, last_id.ToString() + " " + district.CN + "++", conn);
            return last_id;
        }

        /// <summary>
        /// Add parcel record to table LOTTABLE
        /// </summary>
        public static long DB_AppendParcel(TParcel parcel, MySqlConnection conn)
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
            if (parcel.SpecialNote != null)
            {
                if (parcel.SpecialNote.Length > 128)
                    cmd.Parameters.Add("?lot_comment", MySqlDbType.VarChar).Value = parcel.SpecialNote.Substring(0, 64) + "...";
                else
                    cmd.Parameters.Add("?lot_comment", MySqlDbType.VarChar).Value = parcel.SpecialNote;
            }
            cmd.Parameters.Add("?Code_KLADR", MySqlDbType.VarChar).Value = "-";
            cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = parcel.CadastralBlock_id;
            cmd.ExecuteNonQuery();
            long last_id = cmd.LastInsertedId;
            DBWrapper.DB_AppendHistory(ItemTypes.it_Lot, last_id, DBLogRecordStatus.it_Insert, last_id.ToString() + " " + parcel.CN + "++", conn);
            return last_id;
        }

        public static long DB_AppendBlock(TCadastralBlock block, MySqlConnection conn)
        {
            if (conn == null) return -1; if (conn.State != System.Data.ConnectionState.Open) return 1;
            
            MySqlCommand cmd = new MySqlCommand(

            "INSERT INTO blocks(block_id, block_kn, block_status, block_name, district_id)" + "" +
            "              VALUES(NULL,  ?block_kn,?block_status,?block_name,?district_id)", conn);

            //cmd.Parameters.Add("?lottable_id", MySqlDbType.Int32).Value = item_type; - set to NULL due autoIncrement by MySQL server
            cmd.Parameters.Add("?block_kn", MySqlDbType.VarChar).Value = block.CN;
            cmd.Parameters.Add("?block_name", MySqlDbType.VarChar).Value = block.Name;
            cmd.Parameters.Add("?district_id", MySqlDbType.Int32).Value = block.Parent_id;
            cmd.Parameters.Add("?block_status", MySqlDbType.Int32).Value = 0;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return -1;
            }

            int last_id = (int)cmd.LastInsertedId; // crop them to INT
            block.id = last_id; // update
            DBWrapper.DB_AppendHistory(ItemTypes.it_Block, last_id, DBLogRecordStatus.it_Insert, last_id.ToString() + " " + block.CN + "++", conn);
            return last_id;
        }

        public static bool DB_UpdateCadastralDistrict(TCurrentItem district, MySqlConnection conn)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Update cadastral block
        /// </summary>
        /// <param name="block"></param>
        /// <param name="Status"></param>
        /// <param name="Color">Color for display</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static bool DB_UpdateCadastralBlock(TCadastralBlock block, int Status, int Color, MySqlConnection conn)
        {
            if (conn == null) return false; if (conn.State != System.Data.ConnectionState.Open) return false;
            MySqlCommand cmd = new MySqlCommand(
            "update blocks set " +
            "block_kn    = ?block_kn," +
            "block_status= ?block_status," +
            "block_color = ?block_color," +
            "block_name  = ?block_name," +
            "block_comment = ?block_comment" +
            " where block_id = ?block_id", conn);

            cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = block.id;//
            cmd.Parameters.Add("?block_kn", MySqlDbType.VarChar).Value = block.CN;
            cmd.Parameters.Add("?block_status", MySqlDbType.Int32).Value = Status;
            cmd.Parameters.Add("?block_color", MySqlDbType.Int32).Value = Color;
            cmd.Parameters.Add("?block_name", MySqlDbType.VarChar).Value = block.Name;
            cmd.Parameters.Add("?block_comment", MySqlDbType.VarChar).Value = block.Comments;
            cmd.ExecuteNonQuery();
            return true;
        }

        public static bool DB_UpdateParcel(TParcel parcel, MySqlConnection conn)
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
            if (parcel.SpecialNote != null)
                if (parcel.SpecialNote.Length > 128)
                    cmd.Parameters.Add("?lot_comment", MySqlDbType.VarChar).Value = parcel.SpecialNote.Substring(0, 64) + "...";
                else
                    cmd.Parameters.Add("?lot_comment", MySqlDbType.VarChar).Value = parcel.SpecialNote;

            //cmd.Parameters.Add("?Code_KLADR", MySqlDbType.VarChar).Value = parcel.;
            //cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = parcel.CadastralBlock_id;
            cmd.ExecuteNonQuery();
            //long last_id = cmd.LastInsertedId;
            //DB_AppendHistory(ItemTypes.it_Lot, last_id, 50, last_id.ToString() + " " + parcel.CN + "++", conn, conn2, CF.Cfg.District_id, CF.Cfg);
            return true;
        }

        public static bool EraseBlock(long block_id, MySqlConnection conn)
        {
            if (conn == null) return false; if (conn.State != System.Data.ConnectionState.Open) return false;
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                 "DELETE FROM blocks WHERE `block_id`= ?block_id", conn);
                cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = block_id;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return false;
            }

            return true;
        }


        public static bool EraseParcel(long parcel_id, MySqlConnection conn)
        {
            if (conn == null) return false; if (conn.State != System.Data.ConnectionState.Open) return false;
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                 "DELETE FROM lottable WHERE `lottable_id`= ?lottable_id", conn);
                cmd.Parameters.Add("?lottable_id", MySqlDbType.Int32).Value = parcel_id;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return false;
            }
            return true;
        }

        public static bool EraseKPT(long KPT_id, MySqlConnection conn)
        {
            if (conn == null) return false; if (conn.State != System.Data.ConnectionState.Open) return false;
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                 "DELETE FROM kpt WHERE `kpt_id`= ?kpt_id", conn);

                cmd.Parameters.Add("?kpt_id", MySqlDbType.Int32).Value = KPT_id;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return false;
            }

            return true;
        }

        public static bool EraseVidimus(long vidimus_id, MySqlConnection conn)
        {
            if (conn == null) return false; if (conn.State != ConnectionState.Open) return false;
            try
            {
                MySqlCommand cmd = new MySqlCommand(
                 "DELETE FROM vidimus WHERE `vidimus_id`= ?vidimus_id", conn);
                cmd.Parameters.Add("?vidimus_id", MySqlDbType.Int32).Value = vidimus_id;//
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return false;
            }
            return true;
        }


        public static long DB_GetMySQLVariable(string VarName, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(
           "show global variables WHERE  Variable_name ='" + VarName + "'", conn);

            try
            {
                //object resO =   cmd.ExecuteScalar();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    long VarValue = Convert.ToInt64(reader["Value"]);
                    reader.Close();
                    return VarValue;
                }
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return -1;
            }

            return -2;
        }

        public static long DB_AddBlock_KPT(long block_id, TFile KPT, MySqlConnection conn)
        {

            if (conn == null) return -1; if (conn.State != System.Data.ConnectionState.Open) return 1;
            // StatusLabel_AllMessages.Text = "Adding KPT file.... ";

            MySqlCommand cmd = new MySqlCommand(

            "INSERT INTO kpt (kpt_id, block_id, " +
                             " kpt_num,  " +
                             "kpt_date," +
                             "xml_file_name, xml_ns, xml_file_body) " +
            /*  xml_file_name,
                xml_file_body,
                pdf_file_name,
                pdf_file_body,
                zip_file_name,
                zip_file_body" + */
            "  VALUES(NULL, ?block_id, ?kpt_num, " +
            "?kpt_date," +
                           "?xml_file_name, ?xml_ns, ?xml_file_body)", conn);

            //cmd.Parameters.Add("?kpt_id", MySqlDbType.Int32).Value = item_type; - set to NULL due autoIncrement by MySQL server
            cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = block_id;
            cmd.Parameters.Add("?kpt_num", MySqlDbType.VarChar).Value = KPT.Number;
            cmd.Parameters.Add("?kpt_date", MySqlDbType.Date).Value = KPT.Doc_Date;
            cmd.Parameters.Add("?xml_file_name", MySqlDbType.VarChar).Value = System.IO.Path.GetFileName(KPT.FileName);
            cmd.Parameters.Add("?xml_ns", MySqlDbType.VarChar).Value = KPT.xmlns;
            cmd.Parameters.Add("?xml_file_body", MySqlDbType.LongBlob).Value = KPT.File_BLOB;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return -1;
            }

            long last_id = cmd.LastInsertedId;
            KPT.id = last_id;
            DBWrapper.DB_AppendHistory(ItemTypes.it_kpt, block_id,DBLogRecordStatus.it_InsertKPT, "kpt++." + last_id.ToString(), conn);
            return last_id;
        }


        public static byte[] FetchKPTBodyToArray(MySqlConnection conn, long kpt_id)
        {
            if (conn == null) return null;
            if (conn.State != ConnectionState.Open) return null;

            DataTable data = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT kpt_id, xml_file_body," +
                                            " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt" +
                                            " where kpt_id = " + kpt_id.ToString(), conn);
            da.Fill(data);
            if (data.Rows.Count == 1)
            {
                DataRow row = data.Rows[0];
                if (row[1].ToString().Length > 0)
                {
                    byte[] outbyte = (byte[])row[1];
                    row.Delete();
                    row = null;
                    data.Dispose();
                    da.Dispose();
                    GC.Collect();
                    return outbyte;
                }
            }
            return null;
        }


        /// <summary>
        /// Read (SELECT) BLOB from the Database and save it in the stream (RAM). KPT11 only
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="kpt_id"></param>
        /// <returns></returns>
        public static byte[] FetchKPT11Body(MySqlConnection conn, long kpt_id)
        {
            if (conn == null) return null;
            if (conn.State != ConnectionState.Open) return null;

            DataTable data = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT kpt_id, xml_file_body," +
                                            " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt11" +
                                            " where kpt_id = " + kpt_id.ToString(), conn);
            try
            {
                da.Fill(data);
                if (data.Rows.Count == 1)
                {
                    DataRow row = data.Rows[0];
                    byte[] outbyte = (byte[])row[1];
                    //MemoryStream res = new MemoryStream(outbyte);
                    da.Dispose();
                    data.Dispose();
                    return outbyte;
                }
                else
                {
                    da.Dispose();
                    return null;
                }
            }
            catch (Exception ex)
            {
                object exept = ex;
                da.Dispose();
                data.Dispose();
                return null;
            }
        }

        public static long DB_AddBlock_KPT11(long block_id, TFile KPT, MySqlConnection conn)
        {
            if (conn == null) return -1; if (conn.State != System.Data.ConnectionState.Open) return 1;
            if (KPT.Type != netFteo.Rosreestr.dFileTypes.KPT11) return -404;
            MySqlCommand cmd = new MySqlCommand(

                 "INSERT INTO kpt11 (kpt_id, " +
                                  " kpt_type, block_id, " +
                                  " kpt_num,  " +
                                  " kpt_date," +
                                  " xml_file_name, " +
                                  //xml_ns,+
                                  "xml_file_body) " +
                 "  VALUES(NULL, ?kpt_type, ?block_id, ?kpt_num, " +
                                "?kpt_date, ?xml_file_name, " +
                                // "?xml_ns, "+
                                "?xml_file_body)", conn);

            //cmd.Parameters.Add("?kpt_id", MySqlDbType.Int32).Value = item_type; - set to NULL due autoIncrement by MySQL server
            cmd.Parameters.Add("?kpt_type", MySqlDbType.UByte).Value = KPT.Type;
            cmd.Parameters.Add("?block_id", MySqlDbType.Int32).Value = block_id;
            cmd.Parameters.Add("?kpt_num", MySqlDbType.VarChar).Value = KPT.Number;
            cmd.Parameters.Add("?kpt_date", MySqlDbType.Date).Value = KPT.Doc_Date;
            cmd.Parameters.Add("?xml_file_name", MySqlDbType.VarChar).Value = System.IO.Path.GetFileName(KPT.FileName);
            //cmd.Parameters.Add("?xml_ns", MySqlDbType.VarChar).Value = KPT.xmlns;
            cmd.Parameters.Add("?xml_file_body", MySqlDbType.LongBlob).Value = KPT.File_BLOB;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return -1;
            }

            long last_id = cmd.LastInsertedId;
            KPT.id = last_id;
            DBWrapper.DB_AppendHistory(ItemTypes.it_kpt, block_id, DBLogRecordStatus.it_InsertKPT, "kpt++." + last_id.ToString(), conn);
            return last_id;
        }

        public static long DB_AddParcel_Vidimus(long parcel_id, TFile Vidimus, MySqlConnection conn)
        {

            if (conn == null) return -1; if (conn.State != System.Data.ConnectionState.Open) return 1;
            // StatusLabel_AllMessages.Text = "Adding KPT file.... ";

            MySqlCommand cmd = new MySqlCommand(

            "INSERT INTO vidimus (vidimus_id, parcel_id, vidimus_type, " +
                             " v_num,  v_date," +
                             "xml_file_name, xml_file_tns,  xml_file_RootName, xml_file_body) " +
            "  VALUES(NULL, ?parcel_id, ?vidimus_type, ?v_num, ?v_date," +
                           "?xml_file_name, ?xml_file_tns, ?xml_file_RootName, ?xml_file_body)", conn);

            //cmd.Parameters.Add("?vidimus_id", MySqlDbType.Int32).Value = id ????; - set to NULL due autoIncrement by MySQL server
            cmd.Parameters.Add("?parcel_id", MySqlDbType.Int32).Value = parcel_id;
            cmd.Parameters.Add("?vidimus_type", MySqlDbType.Int32).Value = (int)Vidimus.Type;
            cmd.Parameters.Add("?v_num", MySqlDbType.VarChar).Value = Vidimus.Number;
            cmd.Parameters.Add("?v_date", MySqlDbType.Date).Value = Vidimus.Doc_Date;
            cmd.Parameters.Add("?xml_file_name", MySqlDbType.VarChar).Value = System.IO.Path.GetFileName(Vidimus.FileName);
            cmd.Parameters.Add("?xml_file_RootName", MySqlDbType.VarChar).Value = Vidimus.RootName;
            cmd.Parameters.Add("?xml_file_tns", MySqlDbType.VarChar).Value = Vidimus.xmlns;
            cmd.Parameters.Add("?xml_file_body", MySqlDbType.LongBlob).Value = Vidimus.File_BLOB;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return -1;
            }

            long last_id = cmd.LastInsertedId;
            Vidimus.id = last_id;
            DBWrapper.DB_AppendHistory(ItemTypes.it_vidimus, parcel_id, DBLogRecordStatus.it_InsertKPT, "xml++. id: " + last_id.ToString(), conn);
            return last_id;
        }

        /// <summary>
        /// Read BLOB from the Database and save it on to stream
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="vidimus_id"></param>
        /// <returns></returns>
        public static byte[] FetchVidimusBody(MySqlConnection conn, long vidimus_id)
        {
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;

            DataTable data = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT vidimus_id, xml_file_body," +
                                            " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from vidimus " +
                                            " where vidimus_id  = " + vidimus_id.ToString(), conn);

            da.Fill(data);

            if (data.Rows.Count == 1)
            {
                DataRow row = data.Rows[0];
                if (row[1].ToString().Length > 0)
                {
                    byte[] outbyte = (byte[])row[1];
                    row.Delete();
                    row = null;
                    data.Dispose();
                    da.Dispose();
                    GC.Collect();
                    return outbyte;
                }
                else return null;
            }
            else return null;
        }

   

        /// <summary>
        /// Выборка записей из kpt + kpt11, без blob поля xml_file_body, только сведения о его размере 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="block_id">id квартала</param>
        /// <returns></returns>
        public static TFiles LoadBlockFiles(MySqlConnection conn, long block_id)
        {

            TFiles files = new TFiles();
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;

            DataTable data = new DataTable();

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT kpt_id,block_id,xml_file_name,kpt_num,kpt_date,kpt_serial,xml_ns,requestnum,acesscode," +
                   " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt where block_id = " + block_id.ToString() +
                                      " order by kpt_id asc", conn);

            //here maybe "MySql.Data.MySqlClient.MySqlException"
            //as server not ready/sleeping
            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFile file = new TFile(); // CN
                file.id = Convert.ToInt32(row[0]);           // id
                file.FileName = row[2].ToString();           // block_name
                file.Number = row[3].ToString();
                if (row[4].ToString().Length > 0)
                    file.Doc_Date = Convert.ToString(row[4]).Substring(0, Convert.ToString(row[4]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.Serial = row[5].ToString();
                file.xmlns = row[6].ToString();
                file.RequestNum = row[7].ToString();
                file.AccessCode = row[8].ToString();
                if (row[9] != DBNull.Value)
                    file.xmlSize_SQL = (Convert.ToInt64(row[9]));
                files.Add(file);
            }
            data.Reset();

            //KPT11 load:
            da = new MySqlDataAdapter("SELECT kpt_id, kpt_type,block_id,GUID,kpt_num,kpt_serial,kpt_date,requestnum,acesscode,	xml_file_name," +
                   " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb from kpt11 where block_id = " + block_id.ToString() +
                                      " order by kpt_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFile file = new TFile(); // CN
                file.id = Convert.ToInt32(row[0]);           // id
                //file.Type = dFileTypes.KPT11; //Convert.ToByte(row[1]);           // kpt type
                file.xmlns = netFteo.Rosreestr.NameSpaces.KPT11; // explicit setuped
                file.FileName = row[9].ToString();           // block_id
                file.Number = row[4].ToString();
                file.Doc_Date = Convert.ToString(row[6]).Substring(0, Convert.ToString(row[6]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.Serial = row[5].ToString();
                file.RequestNum = row[7].ToString();
                file.AccessCode = row[8].ToString();
                if (row[10] != DBNull.Value)
                    file.xmlSize_SQL = Convert.ToInt64(row[10]);  // long is signed 64 bit integer
                files.Add(file);
            }
            return files;
        }

        public static TFiles LoadParcelFiles(MySqlConnection conn, long parcel_id)
        {

            TFiles files = new TFiles();
            if (conn == null) return null; if (conn.State != ConnectionState.Open) return null;
            DataTable data = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT vidimus_id,parcel_id, " +
                                      "xml_file_name,v_num,v_date,v_serial,xml_file_tns,requestnum,acesscode," +
                   " OCTET_LENGTH(xml_file_body)/1024 as xml_size_kb, vidimus_type from vidimus where parcel_id = " + parcel_id.ToString() +
                                      " order by vidimus_id asc", conn);

            da.Fill(data);
            foreach (DataRow row in data.Rows)
            {
                TFile file = new TFile(); // CN
                file.id = Convert.ToInt32(row[0]);           // id
                file.FileName = row[2].ToString();                       // block_name
                file.Number = row[3].ToString();
                file.Doc_Date = row[4].ToString();
                //file.Doc_Date = Convert.ToString(row[4]).Substring(0, Convert.ToString(row[4]).Length - 7);
                // срезать семь нулей времени MySQL "05.04.2016 0:00:00"
                file.Serial = row[5].ToString();
                file.xmlns = row[6].ToString();
                file.RequestNum = row[7].ToString();
                file.AccessCode = row[8].ToString();
                if (row[9].ToString().Length > 0)
                    file.xmlSize_SQL = Convert.ToInt64(row[9]);
                //file.id = Convert.ToInt32(row[10]); // vidimus_type, int
                files.Add(file);
            }
            return files;
        }



    }
}
