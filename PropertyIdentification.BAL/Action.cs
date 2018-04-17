using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MiniProfiler.Integrations;
using Nest;
using PropertyIdentication.DAL.Entities;

namespace PropertyIdentification.BAL
{
    public class Action
    {
        private string connString = string.Empty;
        private string smartCDRConnString = string.Empty;

        public Action()
        {
            this.smartCDRConnString = ConfigurationManager.ConnectionStrings["SMARTCDR"].ConnectionString;
        }

        public DbConnection GetSmartCDRConnection()
        {
            this.connString = ConfigurationManager.ConnectionStrings["SMARTCDR"].ConnectionString;
            var connectionFactory = new SqlServerDbConnectionFactory(this.connString);
            return DbConnectionFactoryHelper.New(connectionFactory, CustomDbProfiler.Current);
            //return new SqlConnection(this.connString);
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(this.smartCDRConnString);
        }

        public ElasticSearch GetElasticSetting()
        {
            var result = new ElasticSearch();
            result.Uri = ConfigurationSettings.AppSettings["Uri"];
            result.IndexName = ConfigurationSettings.AppSettings["PropertyIndexName"];
            result.TypeName = ConfigurationSettings.AppSettings["PropertyTypeName"];
            result.UserName = ConfigurationSettings.AppSettings["UserName"];
            result.Password = ConfigurationSettings.AppSettings["Password"];
            return result;
        }

        public static ElasticClient GetElasticConnection(ElasticSearch elastiSetting)
        {
            var node = new Uri(elastiSetting.Uri);
            var connectionPool = new SniffingConnectionPool(new[] { node });
            var elasticConfig = new ConnectionSettings(connectionPool)
                .SniffOnConnectionFault(false)
                .SniffOnStartup(false)
                .DisablePing();
            elasticConfig.DefaultIndex(elastiSetting.IndexName);
            elasticConfig.BasicAuthentication(elastiSetting.UserName, elastiSetting.Password);
            return new ElasticClient(elasticConfig);
        }
    }
}