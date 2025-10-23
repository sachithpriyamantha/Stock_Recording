using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Stock_Recording.DAL.DataSource;

namespace Stock_Recording.BLL
{
    class BLL_CS_StockRecoding
    {
        DAL_CS_StockRecording dal = new DAL_CS_StockRecording();
        public DataTable LoadStockRecording(String type, string year)
        {
         return dal.LoadStockRecording(type, year);

        }
        public DataTable LoadStockRecording_C(string year)
        {
            return dal.LoadStockRecording_C(year);
        }



        internal object LoadDetails(object test , object lac ,object docno, object doct, object line)
        {
            return dal.LoadDetails(lac,test, docno , doct, line);
            
        }
        internal object LoadDetails1(object lac1, object whcode, object docno1, object doct1)
        {
            return dal.LoadDetails1(lac1, whcode, docno1, doct1);
        }

 

        internal bool Insertstock1(string locationCode, string documentType, string documentNo, string materialCode, string line, string engScheduleNo, string receivedQty, string date, string remarks)
        {
            return dal.Insertstock1( locationCode,  documentType,  documentNo,  materialCode,  line,  engScheduleNo,  receivedQty,  date, remarks);
        }

        internal DataSet printReport(string locationCode, string documentType, string documentNo, string materialCode, string line, string engScheduleNo, List<string> documentNumbers, List<string> lineNumbers)
        {
            return dal.printReport(locationCode, documentType, documentNo, materialCode, line, engScheduleNo, documentNumbers, lineNumbers);
        }
        

         internal DataSet pntReport(string locationCode, string documentType, string documentNo, string materialCode, string line, string engScheduleNo, List<string> documentNumbers, List<string> lineNumbers)
         {
            return dal.pntReport(locationCode, documentType, documentNo, materialCode, line, engScheduleNo, documentNumbers, lineNumbers);
         }

        
    }
}
