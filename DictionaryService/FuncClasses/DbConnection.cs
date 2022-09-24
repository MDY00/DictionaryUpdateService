using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.OleDb;
using Serilog;

namespace DictionaryService.FuncClasses;

public static class DbConnection
{

    public static List<Dictionary> GetDictsDataFromDB()
    {
        var dbDictsData = new List<Dictionary>();

        try
        {
            using (var connection = new SqlConnection(@AppConfig.DbConString))
            {
                var command = new SqlCommand("MDC_Get_Slowniki", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                    dbDictsData.Add(new Dictionary(reader.GetString(0), reader.GetString(1), ""));
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
        return dbDictsData;
    }

    public static void PostDataToDB_TestUpdate(string dictName, string dictPath, string dictVersion)
    {
        try
        {



            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("pInserterDictUpdateTest", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@DictName", SqlDbType.VarChar).Value = dictName;
            command.Parameters.Add("@DictPath", SqlDbType.VarChar).Value = dictPath;
            command.Parameters.Add("@DictVersion", SqlDbType.VarChar).Value = dictVersion;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
    }

    public static void UpdateICD_9DBProcedure(SqlXml xmlString)
    {

        try
        {


            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("NFZ_ICD9Import", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@XML", SqlDbType.Xml).Value = xmlString;

            connection.Open();

            command.ExecuteNonQuery();
            Log.Information("ICD_9 Updated");
            connection.Close();
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
    }

    public static void UpdateModtarDBProcedure(SqlXml xmlString)
    {
        try
        {
            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("NFZ_MODTAR_Import", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@PARAM", SqlDbType.Xml).Value = xmlString;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }


        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }

    }

    public static void UpdateICD_10DBProcedure(SqlXml xmlString, string dictVersion)
    {
        try
        {
            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("NFZ_ICD10Import", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@XML", SqlDbType.Xml).Value = xmlString;
            command.Parameters.Add("@NR_GEN", SqlDbType.VarChar).Value = dictVersion;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }

        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
    }

    public static void UpdateGruppersDBProcedure(SqlXml xmlString)
    {
        try
        {


            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("NFZ_JgpImport", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@PARAM", SqlDbType.Xml).Value = xmlString;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }

        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }

    }

    public static void UpdateSlortDBProcedure(SqlXml xmlString)
    {
        try
        {
            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("NFZ_SlortImport", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@PARAM", SqlDbType.Xml).Value = xmlString;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
    }

    public static void UpdateSp_RozDBProcedure(SqlXml xmlString)
    {
        try
        {
            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("NFZ_Sp_RozImport", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@PARAM", SqlDbType.Xml).Value = xmlString;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }

        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
    }

    public static void UpdateBAZYLDBProcedure(SqlXml xmlString, string dbfName)
    {
        try
        {
            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("APT_BazylUpd_V2", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@PARAM", SqlDbType.Xml).Value = xmlString;
            command.Parameters.Add("@DBFNAME", SqlDbType.VarChar).Value = dbfName;

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }

    }

    public static void DelBAZYLDBProcedure()
    {
        try
        {
            using var connection = new SqlConnection(@AppConfig.DbConString);
            var command = new SqlCommand("APT_BazylDel", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 120
            };

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }

        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
    }

    public static List<string> GetXmlFromDBFProvider(string bazylFolderPath, string dbfFileName)
    {
        var dataStrings = new List<string>();
        try
        {
            string strAccessConn = "Provider=Microsoft Office 16.0 Access Database Engine OLE DB Provider;Data Source="
                                     + bazylFolderPath + ";Extended Properties=dBASE IV;";
            string strAccessSelect = "SELECT * FROM " + dbfFileName;
            var myDataTable = new DataTable();
            var myAccessConn = new OleDbConnection(strAccessConn);
            var myAccessCommand = new OleDbCommand(strAccessSelect, myAccessConn);
            var myDataAdapter = new OleDbDataAdapter(myAccessCommand);

            myAccessConn.Open();
            myDataAdapter.Fill(myDataTable);
            myAccessConn.Close();

            var tables = myDataTable.AsEnumerable().ToChunks(1000).Select(rows => rows.CopyToDataTable());
            var dataSets = new List<DataSet>();
       
            foreach (var dataTable in tables)
            {
                var dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);
                var xmlDataSet = dataSet.GetXml();
                dataStrings.Add(xmlDataSet.Replace('\'', '\"').Replace("&", ";").Replace("00 000", "000000").Replace("0000 0", "000000"));
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
        return dataStrings;
    }

    private static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> enumerable, int chunkSize)
    {
        int itemsReturned = 0;
        var list = enumerable.ToList();
        int count = list.Count;

        while (itemsReturned < count)
        {
            int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
            yield return list.GetRange(itemsReturned, currentChunkSize);
            itemsReturned += currentChunkSize;
        }
    }
}
