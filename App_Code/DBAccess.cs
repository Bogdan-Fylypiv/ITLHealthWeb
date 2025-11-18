using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ITLHealthWeb
{
    /// <summary>
    /// Database access class for Web Forms (adapted from WinForms DBAccess)
    /// </summary>
    public class DBAccess : IDisposable
    {
        public enum PacketSizes
        {
            TinyPacket = 512,
            xSmallPacket = 1024,
            SmallPacket = 2048,
            DefaultPacket = 4096,
            LargePacket = 8192,
            XLargePacket = 16384
        };

        internal SqlConnection m_Conn;
        internal PacketSizes m_PacketSize = PacketSizes.DefaultPacket;
        internal bool m_IsOpen = false;
        internal string m_ConnString = "";
        internal string m_Logon = "";
        internal string m_PWD = "";
        internal string m_EncryptKey = "";
        internal string m_DBName = "";
        internal string m_SvrName = "";
        internal int m_ConnTimeout = 10;
        internal int m_CmdTimeout = 30;

        #region Constructor
        public DBAccess(string sKey)
        {
            try
            {
                m_EncryptKey = sKey;

                // Read from Web.config instead of app.config
                var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"];
                if (connString != null)
                {
                    m_ConnString = connString.ConnectionString;
                    ParseConnStrParms(m_ConnString);
                }
                else
                {
                    throw new ConfigurationErrorsException("DefaultConnection connection string not found in Web.config");
                }

                m_IsOpen = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DBAccess Error: {ex.Message}");
                m_IsOpen = false;
                throw;
            }
        }
        #endregion

        #region Properties
        public bool IsOpen
        {
            get { return m_IsOpen; }
        }

        public string ConnectionString
        {
            get { return m_ConnString; }
            set
            {
                try
                {
                    ParseConnStrParms(value);
                    using (SqlConnection testConn = new SqlConnection(value))
                    {
                        testConn.Open();
                        m_SvrName = testConn.DataSource.ToString();
                        m_DBName = testConn.Database.ToString();
                    }
                    m_IsOpen = true;
                    m_ConnString = value;
                }
                catch
                {
                    ParseConnStrParms(m_ConnString);
                    throw;
                }
            }
        }

        public int ConnectionTimeout
        {
            get { return m_ConnTimeout; }
            set { m_ConnTimeout = value; }
        }

        public int CommandTimeout
        {
            get { return m_CmdTimeout; }
            set { m_CmdTimeout = value; }
        }

        public string ServerName
        {
            get { return m_SvrName; }
        }

        public string DatabaseName
        {
            get { return m_DBName; }
        }
        #endregion

        #region Methods
        public SqlConnection GetConnection()
        {
            try
            {
                if (m_Conn == null || m_Conn.State == ConnectionState.Closed)
                {
                    m_Conn = new SqlConnection(m_ConnString);
                    m_Conn.Open();
                }
                return m_Conn;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetConnection Error: {ex.Message}");
                throw;
            }
        }

        private void ParseConnStrParms(string connStr)
        {
            try
            {
                string[] parts = connStr.Split(';');
                foreach (string part in parts)
                {
                    string[] keyValue = part.Split('=');
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim().ToLower();
                        string value = keyValue[1].Trim();

                        switch (key)
                        {
                            case "data source":
                            case "server":
                                m_SvrName = value;
                                break;
                            case "initial catalog":
                            case "database":
                                m_DBName = value;
                                break;
                            case "user id":
                            case "uid":
                                m_Logon = value;
                                break;
                            case "password":
                            case "pwd":
                                m_PWD = value;
                                break;
                            case "connection timeout":
                                int.TryParse(value, out m_ConnTimeout);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ParseConnStrParms Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            try
            {
                if (m_Conn != null)
                {
                    if (m_Conn.State != ConnectionState.Closed)
                    {
                        m_Conn.Close();
                    }
                    m_Conn.Dispose();
                    m_Conn = null;
                }
                m_IsOpen = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dispose Error: {ex.Message}");
            }
        }
        #endregion
    }
}
