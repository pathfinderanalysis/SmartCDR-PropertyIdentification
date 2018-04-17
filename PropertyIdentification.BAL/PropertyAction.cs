using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Nest;
using PropertyIdentication.DAL.Entities;
using PropertyIdentification.Common;

namespace PropertyIdentification.BAL
{
    public class PropertyAction : Action
    {
        public IEnumerable<Document> ProcessStart()
        {
            var logger = new NLogger();
            Console.WriteLine("========= Started Process =========");
            var resultDocument = GetDocumentData().ToList();
            while (resultDocument.Any())
            {
                //var cadId = ConfigurationManager.AppSettings["CADId"];
                //var CadIds = cadId.Split(',');

                Parallel.ForEach(resultDocument, new ParallelOptions
                {
                    MaxDegreeOfParallelism = int.Parse(ConfigurationManager.AppSettings["ParallelThreads"])
                }, document =>
                {
                    var filteredResults = GetDocumentFromES(document.Fips.ToString(),
                        document.LegalDescription, document.Lot, document.Block,
                        document.Section).ToList();

                    document.ProcessingState = 1;
                    if (filteredResults.Count() == 1)
                    {
                        if (filteredResults.First().AccountNumber != null)
                        {
                            document.AccountNumber = filteredResults.First().AccountNumber;
                            document.OCALUC = filteredResults.First().OCALUC;
                            //documents.Add(document);
                        }
                    }

                    Console.WriteLine("Document ID - {0} - Records Found : {1}", document.DocumentID, filteredResults.Count);
                });

                // Show the  Count
                Console.WriteLine("Records to be Updated - " + resultDocument.Count);

                UpdateData(resultDocument);

                resultDocument = GetDocumentData().ToList();
            }
            Console.WriteLine("========= Completed Process =========");
            return null;
        }

        public IEnumerable<Document> GetDocumentData()
        {
            var query = "Select top 200 * from TMP_SMARTCDR_PendingAndAssignedDocuments_bala_20180411 with(nolock) where ProcessingState=0";
            using (var connection = this.GetSmartCDRConnection())
            {
                return connection.Query<Document>(query, commandTimeout: connection.ConnectionTimeout);
            }
        }

        public IEnumerable<Property> GetDocumentFromES(string fips, string legalDescription, string lot, string block,
            string section)
        {
            try
            {
                var elasticSetting = this.GetElasticSetting();
                IEnumerable<Property> filterdResult;
                var esClient = GetElasticConnection(elasticSetting);

                string[] selectedFields =
                {
                    "LegalBlock",
                    "LegalSection",
                    "AccountNumber",
                    "OCALUC"
                };
                if (esClient.IndexExists(elasticSetting.IndexName).Exists)
                {
                    filterdResult = (List<Property>)esClient.Search<Property>(q => q
                        .Index(elasticSetting.IndexName)
                        .Type(elasticSetting.TypeName)
                        .Size(100)
                        .Source(s => s.Includes(ii => ii.Fields(selectedFields)))
                        .Query(qu => qu
                            .Bool(b => b
                                    .Must(sh => sh
                                            .Match(c => c
                                                .Field("LegalBriefDescription")
                                                .Query(legalDescription).Operator(Operator.And)),
                                        sh => sh
                                            .Match(c => c
                                                .Field("FIPS")
                                                .Query(fips)),
                                        sh => sh.Match(dd => dd.Field("LegalLotNumber").Query(lot))
                                    )
                            //.MustNot(n => n
                            //        .Match(j => j
                            //            .Field("OCALUC")
                            //            .Query("BPP")))
                            ))).Documents.ToList();

                    if (!string.IsNullOrWhiteSpace(section))
                    {
                        filterdResult = filterdResult.Where(x => x.LegalSection == section);
                    }

                    if (!string.IsNullOrWhiteSpace(block))
                    {
                        filterdResult = filterdResult.Where(x => x.LegalBlock == block);
                    }

                    return filterdResult;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public void UpdateData<T>(List<T> list)
        {
            var dt = new DataTable("SMARTCDR_PendingAndAssignedDocuments");
            dt = list.ToArray().ToDataTable();

            using (var conn = this.GetConnection())
            {
                using (var command = new SqlCommand("", conn))
                {
                    try
                    {
                        conn.Open();

                        //Creating temp table on database
                        command.CommandText = "CREATE TABLE #TmpTable(DocumentId varchar(100),CADId int,Lot varchar(400),Block varchar(400),Section varchar(400),LegalDescription varchar(max),FlimCode varchar(200),Fips int,AccountNumber varchar(100),ProcessingState int,OCALUC varchar(10))";
                        command.ExecuteNonQuery();

                        //Bulk insert into temp table
                        using (var bulkcopy = new SqlBulkCopy(conn))
                        {
                            bulkcopy.BulkCopyTimeout = 660;
                            bulkcopy.DestinationTableName = "#TmpTable";
                            bulkcopy.WriteToServer(dt);
                            bulkcopy.Close();
                        }
                        // Updating destination table, and dropping temp table
                        command.CommandTimeout = 300;
                        command.CommandText = "UPDATE T SET T.AccountNumber=Temp.AccountNumber,T.OCALUC=temp.OCALUC,T.ProcessingState=temp.ProcessingState FROM TMP_SMARTCDR_PendingAndAssignedDocuments_bala_20180411 T INNER JOIN #TmpTable Temp ON t.DocumentId=temp.DocumentId and t.CadId=temp.CadId ; DROP TABLE #TmpTable;";
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        // Handle exception properly
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}