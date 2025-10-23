using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OracleClient;

namespace Stock_Recording.DAL.DataSource
{
    class DAL_CS_StockRecording
    {


        public DataTable LoadStockRecording(String type, string year)
        {

 
            DataSource.DAL_DS_StockRecording ds = new DataSource.DAL_DS_StockRecording();
            DataTable dt = ds.tbl_StockRecording;
            DBconnect.connect();

            string getstock = @"SELECT
                                        mvt_loc_code,
                                        mvt_document_type,
                                        mvt_document_no,
                                        to_char(mvt_date, 'RRRR-MM-DD')                                         AS mvtdate,
                                        mvt_material_code,
                                        mmspack.get_material_description(mvt_material_code)                     AS description,
                                        mmspack.get_material_unit(mvt_material_code)                            AS unit,
                                        TO_CHAR(mvt_quantity, '999G999G999D00') AS mvt_quantity,
                                        (SELECT   Max(prd_remarks) AS umr_remarks
                                          FROM     mms_transaction_details,
                                                   pms_requisition_details
                                          WHERE    mtd_loc_code = prd_wh_code
                                          AND      mtd_material_code = prd_material_code
                                          AND      mtd_jcat = prd_jcat
                                          AND      mtd_jmain = prd_jmain
                                          AND      mtd_sub = prd_sub
                                          AND      mtd_spec = prd_spec
                                          AND      mtd_extc = prd_extc
                                          AND      mtd_document_type = prd_document_type
                                          AND      mtd_document_no = prd_document_no
                                          AND      mtd_loc_code = mvt_loc_code
                                          AND      mtd_material_code = mvt_material_code
                                          AND      mtd_line = mvt_line
                                          AND      prd_request_no = mvt_reference_no
                                          AND      prd_serial_no = mvt_engschedule_no
                                          GROUP BY prd_remarks) AS umr_remarks,
                                        (
                                            SELECT
                                                TO_CHAR(NVL(SUM(mkt_quantity), 0), '999G999G999G990D00') AS mvt_accepted_quantity
                                            FROM
                                                mms_krvirtual_transactions
                                            WHERE
                                                    mkt_loc_code = mvt_loc_code
                                                AND mkt_document_type = mvt_document_type
                                                AND mkt_document_no = mvt_document_no
                                                AND mkt_material_code = mvt_material_code
                                                AND mkt_line = mvt_line
                                        )                                                                       AS mvt_accepted_quantity,
                                        TO_CHAR(NVL(mvt_quantity, 0.00) - NVL(mvt_accepted_quantity, 0.00), '9999990.00') AS bal_qty,
                                        mvt_reference_no,
                                        mvt_engschedule_no,
                                        created_by,
                                        mvt_line,
                                        to_char(created_date, 'RRRR-MM-DD')                                     AS createddate,
                                        hrmpack.get_vname(created_by)                                           AS createdname,
                                        mmspack.get_material_description(mvt_material_code)                     AS material_description,
                                        mvt_remarks,
                                        (
                                            SELECT
                                                mdd_jcat
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS jcat,
                                        (
                                            SELECT
                                                mdd_jmain
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS jmain,
                                        (
                                            SELECT
                                                mdd_sub
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS sub,
                                        (
                                            SELECT
                                                mdd_spec
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS spec,
                                        (
                                            SELECT
                                                mdd_extc
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS extc,

                                        (SELECT pul.pul_location_name  
                                        FROM pms_requisition_details prd
                                        JOIN pms_umrdelivery_location pul
                                          ON prd.prd_delivery_location = pul.pul_serial_no
                                        WHERE prd.prd_wh_code = mvt_loc_code
                                          AND prd.prd_document_type = mvt_document_type
                                          AND prd.prd_document_no = mvt_document_no
                                          AND prd.prd_material_code = mvt_material_code
                                          AND prd.prd_request_no = mvt_reference_no
                                          AND prd.prd_serial_no = mvt_engschedule_no
                                        GROUP BY pul.pul_location_name)                                        AS deliver_location_name,
                                          (SELECT CASE  prd_surftreat_status 
                                            WHEN 'BP' THEN 'Blasting and Painting'
                                            WHEN 'OB' THEN 'Only Blasting'
                                            WHEN 'WT' THEN 'Without Treatment'
                                            ELSE 'Unknown'
                                                  END
                                          FROM     pms_requisition_details
                                          WHERE    prd_material_code = mvt_material_code
                                          AND      prd_request_no = mvt_reference_no
                                          AND      prd_serial_no = mvt_engschedule_no
                                          AND      prd_serial_no IS NOT NULL
                                          GROUP BY prd_request_no,
                                                   prd_material_code,
                                                   prd_serial_no,
                                                   prd_surftreat_status)                                       AS surftreat_status

                                    FROM
                                        mms_virtual_transactions
                                    WHERE
                                            mvt_loc_code = 'STK'
                                        AND mvt_document_type = 'MRQ'
                                        AND TO_CHAR(mvt_date, 'YYYY') = '" + year + @"'
                                        AND nvl(mvt_quantity, 0.00) <> nvl(mvt_accepted_quantity, 0.00)";



            OracleDataReader odrYr = DBconnect.readtable(getstock);
            while (odrYr.Read())
            {
                DataRow r = dt.NewRow();
                r["WH_Code"] = odrYr["mvt_loc_code"].ToString();
                r["Doc_Type"] = odrYr["mvt_document_type"].ToString();
                r["GRN_No"] = odrYr["mvt_document_no"].ToString();
                r["GRN_Date"] = odrYr["mvtdate"].ToString();
                r["Material_Code"] = odrYr["mvt_material_code"].ToString();
                r["Description"] = odrYr["description"].ToString();
                r["Unit"] = odrYr["Unit"].ToString();
                r["GRN_Qty"] = odrYr["mvt_quantity"].ToString();
                r["Accepted_Qty"] = odrYr["mvt_accepted_quantity"].ToString();
                r["Umr_no"] = odrYr["mvt_reference_no"].ToString();
                r["Schedule_No"] = odrYr["mvt_engschedule_no"].ToString();
                r["Created_name"] = odrYr["createdname"].ToString();
                r["KRY_Remark"] = odrYr["mvt_remarks"].ToString();
                r["Desciption_Material"] = odrYr["material_description"].ToString();
                r["Created_Date"] = odrYr["createddate"].ToString();
                r["Bal_Qty"] = odrYr["Bal_Qty"].ToString();
                r["UMR_Remark"] = odrYr["umr_remarks"].ToString();
                r["jcat"] = odrYr["jcat"].ToString();
                r["jmain"] = odrYr["jmain"].ToString();
                r["sub"] = odrYr["sub"].ToString();
                r["spec"] = odrYr["spec"].ToString();
                r["extc"] = odrYr["extc"].ToString();
                r["Delivery_loc"] = odrYr["deliver_location_name"].ToString();
                r["Surftreat_Status"] = odrYr["surftreat_status"].ToString();
                r["line"] = odrYr["mvt_line"].ToString();

                dt.Rows.Add(r);
            }
            odrYr.Close();
            DBconnect.disconnect();
            return dt;
        }

        internal DataSet printReport(string locationCode, string documentType, string documentNo, string materialCode, string line, string engScheduleNo, List<string> documentNumbers, List<string> lineNumbers)
        {
            DataSource.DAL_DS_StockRecording ds = new DataSource.DAL_DS_StockRecording();

            ds.tbl_report.Clear();
            DBconnect.connect();

            string docNoList = string.Join(",", documentNumbers.Select(d => $"'{d}'"));
            string lineNoList = string.Join(",", lineNumbers.Select(l => $"'{l}'"));

            
            string query2 = $@"SELECT mvt_material_code,
                                mvt_document_no, mvt_line
                                FROM MMS_VIRTUAL_TRANSACTIONS
                                WHERE mvt_loc_code = 'STK'
                                AND mvt_document_type = 'MRQ'
                                AND mvt_document_no IN ({docNoList})
                                AND mvt_line IN ({lineNoList})";



            OracleDataReader odr2 = DBconnect.readtable(query2);
            while (odr2.Read())
            {


                string getreport = @"SELECT to_char(mvt_date, 'RRRR-MM-DD') AS ""Date"",
                                                   mvt_engschedule_no AS ""Schedule No"",
                                                   mvt_material_code AS ""Mat Code"",
                                                   mmspack.Get_material_description(mvt_material_code) AS ""Mat Desc"",
                                                   mmspack.Get_material_unit(mvt_material_code) AS ""UM"",
                                                   TO_CHAR(Nvl(mvt_quantity, 0.00), '9999990.00') AS ""Qty"",
                                                   (SELECT mdd_jcat 
                                                    FROM   mms_document_details
                                                    WHERE  mdd_document_no = mvt_document_no
                                                    AND mdd_document_type = mvt_document_type
                                                    ) AS ""Job no"",
                                                   (SELECT pul_location_name
                                                    FROM   pms_umrdelivery_location
                                                    WHERE  pul_serial_no = (SELECT prd_delivery_location
                                                                            FROM   pms_requisition_details
                                                                            WHERE  prd_wh_code = mvt_loc_code
                                                                        AND prd_document_type = mvt_document_type
                                                                        AND prd_document_no = mvt_document_no
                                                                        AND prd_material_code = mvt_material_code
                                                                        AND prd_request_no = mvt_reference_no
                                                                        AND prd_serial_no = mvt_engschedule_no
                                                                        GROUP  BY prd_delivery_location))
                                                   AS ""delivery location"",
                                                   (SELECT prd_surftreat_status

                                                    FROM   pms_requisition_details
                                                    WHERE  prd_jcat
                                                           || prd_jmain
                                                           || prd_sub
                                                           || prd_spec
                                                           || prd_extc = (SELECT mdd_jcat
                                                                                 || mdd_jmain
                                                                                 || mdd_sub
                                                                                 || mdd_spec
                                                                                 || mdd_extc
                                                                          FROM   mms_document_details
                                                                          WHERE  mdd_document_no = mvt_document_no
                                                                        AND mdd_document_type = mvt_document_type)
                                                                        AND prd_material_code = mvt_material_code
                                                           AND prd_request_no = mvt_reference_no
                                                           AND prd_serial_no = mvt_engschedule_no)
                                                   AS ""surftreat_status"",
                                                   mvt_reference_no AS ""UMR No"",
                                                   mvt_reference_type AS ""Ref Type"",
                                                   (SELECT ' '
                                                           || hrmpack.Get_bname(mdd_originator)
                                                    FROM   mms_document_details
                                                    WHERE  mdd_loc_code = mvt_loc_code
                                                    AND mdd_document_type = mvt_document_type
                                                    AND mdd_document_no = mvt_document_no) AS ""Requster"",
                                            TO_CHAR(mmspack.Get_balance_quantity('STK', mvt_material_code), '9999990.00') AS ""Bal Qty""
                                            FROM   mms_virtual_transactions
                                            WHERE  mvt_loc_code = 'STK'
                                                   AND mvt_document_type = 'MRQ'
                                                   AND mvt_date >= ADD_MONTHS(SYSDATE, -60)
                                                   AND Nvl(mvt_quantity, 0.00) <> Nvl(mvt_accepted_quantity, 0.00) 
                                                   AND   MVT_DOCUMENT_NO  = '" + odr2["mvt_document_no"] + @"'   
                                                   AND  mvt_material_code = '" + odr2["mvt_material_code"] + @"' 
                                                   AND mvt_line =  '" + odr2["mvt_line"] + @"' ";

                OracleDataReader odr = DBconnect.readtable(getreport);
                while (odr.Read())
                {
                    DataRow r = ds.Tables["tbl_report"].NewRow();
                    r["Date"] = odr["Date"].ToString();
                    r["Sch_No"] = odr["Schedule No"].ToString();
                    r["Mat_Code"] = odr["Mat Code"].ToString();
                    r["Description"] = odr["Mat Desc"].ToString();
                    r["Unit"] = odr["UM"].ToString();
                    r["Qty"] = odr["Qty"].ToString();
                    r["Job_No"] = odr["Job no"].ToString();
                    r["Delivery_Loc"] = odr["delivery location"].ToString();
                    r["Surf_Status"] = odr["surftreat_status"].ToString();
                    r["UMR_No"] = odr["UMR No"].ToString();
                    r["Requester"] = odr["Requster"].ToString();
                    r["Bal_Qty"] = odr["Bal Qty"].ToString();
                    ds.Tables["tbl_report"].Rows.Add(r);
                }

            }
            return ds;
        }

        internal bool Insertstock1(object locationCode, object documentType, object documentNo, object materialCode, string line, object engScheduleNo, object receivedQty, object date, object remarks)
        {

            Boolean update = false;
            Boolean insert = false;
            string serial_no = "";
            string quantity = "";
            DBconnect.connect();
            bool answer = false;


            if (receivedQty != null && decimal.TryParse(receivedQty.ToString(), out decimal qty) && qty > 0)
            {


                string mem_serial_no = @"SELECT (Nvl(Max(mkt_serial_no),0) + 1) AS ttt
                                              FROM   mms_krvirtual_transactions
                                              WHERE  mkt_loc_code = '" + locationCode + @"'
                                              AND    mkt_document_type = '" + documentType + @"'
                                              AND    mkt_document_no = '" + documentNo + @"'
                                              AND    mkt_material_code = '" + materialCode + @"'
                                              AND    mkt_line = '" + line + @"'";

                OracleDataReader odr = DBconnect.readtable(mem_serial_no);
                while (odr.Read())
                {
                    serial_no = odr["ttt"].ToString();
                }
                odr.Close();

                string insertQuery = @"  INSERT INTO mms_krvirtual_transactions(mkt_serial_no,
                                                                                            mkt_loc_code,
                                                                                            mkt_document_type,
                                                                                            mkt_document_no,
                                                                                            mkt_material_code,
                                                                                            mkt_line,
                                                                                            mkt_date,
                                                                                            mkt_quantity,
                                                                                            mkt_remarks,
                                                                                            created_by,
                                                                                            created_date,
                                                                                            mkt_engschedule_no
                                                                                )
                                                                                VALUES
                                                                                               ('" + serial_no + "', " +
                                                                                    "'" + locationCode + "'," +
                                                                                    "'" + documentType + "'," +
                                                                                    "'" + documentNo + "' ," +
                                                                                    "'" + materialCode + "', " +
                                                                                    "'" + line + "', " +
                                                                                    " TO_DATE('" + date + "', 'YYYY-MM-DD')," +
                                                                                    "'" + receivedQty + "', " +
                                                                                    "'" + remarks + "', " +
                                                                                    "'" + Connection.UserName + "' ," +
                                                                                    " SYSDATE ," +
                                                                                    "'" + engScheduleNo + "')";


                if (!DBconnect.AddEditDel(insertQuery))
                    throw new Exception("Insert failed");
            }

            string queryupdate = @"UPDATE mms_virtual_transactions
                                      SET MVT_ACCEPTED_QUANTITY = '" + receivedQty + @"',
                                      UPDATED_DATE = SYSDATE
                                      WHERE MVT_LOC_CODE = '" + locationCode + @"'
                                      AND MVT_DOCUMENT_TYPE = '" + documentType + @"'
                                      AND MVT_DOCUMENT_NO = '" + documentNo + @"'
                                      AND MVT_MATERIAL_CODE = '" + materialCode + @"'
                                      AND MVT_LINE = '" + line + @"'";

            answer = DBconnect.AddEditDel(queryupdate);
            return answer;
        }

        internal object LoadDetails(object test, object lac, object doct, object docno, object line)
        {
            DataSource.DAL_DS_StockRecording ds = new DataSource.DAL_DS_StockRecording();
            DataTable dt = ds.tbl_krvirtual;
            DBconnect.connect();

            string getdetails = @"SELECT mkt_serial_no,
                                   TO_CHAR(mkt_quantity, '9999990.00') AS mkt_quantity,
                                   TO_CHAR(mkt_date, 'RRRR-MM-DD') AS mkt_date,
                                   mkt_remarks
                                   FROM   mms_krvirtual_transactions
                                   WHERE  mkt_material_code = '" + lac + @"'
                                   AND  mkt_loc_code = '" + test + @"'
                                   AND  mkt_document_type = '" + docno + @"'
                                   AND  mkt_document_no = '" + doct + @"'
                                   AND  mkt_line = '" + line + @"'
                                   ORDER BY mkt_serial_no";

            OracleDataReader odrYr = DBconnect.readtable(getdetails);
            while (odrYr.Read())
            {
                DataRow r = dt.NewRow();
                r["Serial_no"] = odrYr["mkt_serial_no"].ToString();
                r["Recieved_Qty"] = odrYr["mkt_quantity"].ToString();
                r["Date"] = odrYr["mkt_date"].ToString();
                r["Remark"] = odrYr["mkt_remarks"].ToString();

                dt.Rows.Add(r);
            }
            odrYr.Close();
            DBconnect.disconnect();
            return dt;
        }

        public DataTable LoadStockRecording_C(string year)
        {
            DataSource.DAL_DS_StockRecording ds = new DataSource.DAL_DS_StockRecording();
            DataTable dt = ds.tbl_StockRecoding_C;
            DBconnect.connect();

            string getstock_c = @"SELECT   mvt_loc_code,             
                                           mvt_document_type,         
                                           mvt_document_no,           
                                           mvt_material_code,
                                           mvt_line,
                                           TO_CHAR(updated_date, 'YYYY-MM-DD HH24:MI:SS') AS updated_date,
                                           mmspack.get_material_description(mvt_material_code) As description,
                                           mmspack.get_material_unit(mvt_material_code) As Unit,
                                           hrmpack.get_vname(created_by) As CreateName,
                                           mmspack.get_material_description(mvt_material_code) As MatDes,
                                           TO_CHAR(mvt_quantity, '9999990.00') AS mvt_quantity,
                                           (
                                            SELECT
                                                mdd_jcat
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS jcat,
                                        (
                                            SELECT
                                                mdd_jmain
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS jmain,
                                        (
                                            SELECT
                                                mdd_sub
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS sub,
                                        (
                                            SELECT
                                                mdd_spec
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS spec,
                                        (
                                            SELECT
                                                mdd_extc
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                AND mdd_document_type = 'MRQ'
                                        )                                                                       AS extc,

                                        (SELECT pul.pul_location_name  
                                            FROM pms_requisition_details prd
                                            JOIN pms_umrdelivery_location pul
                                              ON prd.prd_delivery_location = pul.pul_serial_no
                                            WHERE prd.prd_wh_code = mvt_loc_code
                                              AND prd.prd_document_type = mvt_document_type
                                              AND prd.prd_document_no = mvt_document_no
                                              AND prd.prd_material_code = mvt_material_code
                                              AND prd.prd_request_no = mvt_reference_no
                                              AND prd.prd_serial_no = mvt_engschedule_no
                                            GROUP BY pul.pul_location_name)                                        AS deliver_location_name,

                                            (SELECT 
   
                                               cdlpack.Get_location_name(mdd_originator_location)
                                              FROM   mms_document_details
                                              WHERE  mdd_loc_code = mvt_loc_code
                                              AND    mdd_document_type = mvt_document_type
                                              AND    mdd_document_no = mvt_document_no)                             AS originator_location_name,

                                            (SELECT 
                                                     mdd_invoice_no  
                                              FROM   mms_document_details
                                              WHERE  mdd_document_no = mvt_document_no
                                              AND    mdd_document_type = mvt_document_type
                                              AND    mdd_document_type = 'MRQ')                                     AS invoice_no,

                                        

                                           TO_CHAR(mvt_date, 'RRRR-MM-DD') AS mvt_date,
                                           mvt_reference_type,        
                                           mvt_reference_no,          
                                           mvt_engschedule_no,
                                           mvt_remarks
                                  FROM     mms_virtual_transactions 
                                  WHERE    mvt_document_type = 'MRQ'    
                                  AND      nvl(mvt_quantity,0.00) = nvl(mvt_accepted_quantity,0.00)
                                  AND TO_CHAR(mvt_date, 'YYYY') = '" + year + @"'

                                  ORDER BY mvt_document_no DESC";

            OracleDataReader odrYr = DBconnect.readtable(getstock_c);
            while (odrYr.Read())
            {
                DataRow r = dt.NewRow();
                r["WH_Code"] = odrYr["mvt_loc_code"].ToString();
                r["Doc_Type"] = odrYr["mvt_document_type"].ToString();
                r["Document_No"] = odrYr["mvt_document_no"].ToString();
                r["Material_Code"] = odrYr["mvt_material_code"].ToString();
                r["Description"] = odrYr["description"].ToString();
                r["Unit"] = odrYr["Unit"].ToString();
                r["Name"] = odrYr["CreateName"].ToString();
                r["Quantity"] = odrYr["mvt_quantity"].ToString();
                r["Date"] = odrYr["mvt_date"].ToString();
                r["Ref_Type"] = odrYr["mvt_reference_type"].ToString();
                r["Ref_No "] = odrYr["mvt_reference_no"].ToString();
                r["Schedule_No"] = odrYr["mvt_engschedule_no"].ToString();
               // r["Refeence_No"] = odrYr["mvt_remarks"].ToString();
                r["Material_Description"] = odrYr["MatDes"].ToString();
                r["KRY_Remark"] = odrYr["mvt_remarks"].ToString();
                r["Invoice_No"] = odrYr["invoice_no"].ToString();
                r["Upated_Date"] = odrYr["updated_date"].ToString();
                
                r["Requester_Barcode"] = odrYr["mvt_remarks"].ToString();
                r["Originator_Location"] = odrYr["originator_location_name"].ToString();
                r["Delivery_Loc"] = odrYr["deliver_location_name"].ToString();
                r["Project_No"] = odrYr["mvt_remarks"].ToString();
                r["Project_Name"] = odrYr["mvt_remarks"].ToString();

                r["jmain"] = odrYr["jmain"].ToString();
                r["sub"] = odrYr["sub"].ToString();
                r["spec"] = odrYr["spec"].ToString();
                r["extc"] = odrYr["extc"].ToString();
                r["jcat"] = odrYr["jcat"].ToString();
                r["line"] = odrYr["mvt_line"].ToString();

                string mc = odrYr["mvt_material_code"].ToString();
                string dtype = odrYr["mvt_document_type"].ToString();
                string dn = odrYr["mvt_document_no"].ToString();
                string mlc = odrYr["mvt_loc_code"].ToString();

                string getstatus = @" SELECT   CASE prd_surftreat_status
                                        WHEN 'BP' THEN 'Blasting and Painting'
                                        WHEN 'OB' THEN 'Only Blasting'
                                        WHEN 'WT' THEN 'Without Treatment'
                                        ELSE 'Unknown'
                                        END AS prd_surftreat_status " +
                    "                   FROM     pms_requisition_details" +
                    "                   WHERE    prd_wh_code = '" + mlc + "'" +
                    "                   AND prd_document_type = '" + dtype + "'" +
                    "                   AND prd_document_no = '" + dn + "'" +
                    "                   AND prd_material_code ='" + mc + "'" +
                    "                   GROUP BY prd_surftreat_status ";


                OracleDataReader odYr = DBconnect.readtable(getstatus);
                while (odYr.Read())
                {
                    r["Surface_Treatment_Status"] = odYr["prd_surftreat_status"].ToString();
                }
                
                dt.Rows.Add(r);
            }
            odrYr.Close();
            DBconnect.disconnect();
            return dt;
        }

        internal object LoadDetails1(object lac1, object whcode, object docno1, object doct1)
        {
            DataSource.DAL_DS_StockRecording ds = new DataSource.DAL_DS_StockRecording();
            DataTable dt = ds.tbl_krvirtual;
            DBconnect.connect();



            string getdetails = @"SELECT mkt_serial_no,
                                               TO_CHAR(mkt_quantity, '9999990.00') AS mkt_quantity,
                                               TO_CHAR(mkt_date, 'RRRR-MM-DD') AS mkt_date,
                                               mkt_remarks
                                               FROM   mms_krvirtual_transactions
                                               WHERE  mkt_material_code = '" + lac1 + @"'
                                               AND  mkt_loc_code = '" + whcode + @"'
                                               AND  mkt_document_type = '" + doct1 + @"'
                                               AND  mkt_document_no = '" + docno1 + @"'
                                               ORDER BY mkt_serial_no";
            
            OracleDataReader odrYr = DBconnect.readtable(getdetails);
            while (odrYr.Read())
            {
                DataRow r = dt.NewRow();
                r["Serial_no"] = odrYr["mkt_serial_no"].ToString();
                r["Recieved_Qty"] = odrYr["mkt_quantity"].ToString();
                r["Date"] = odrYr["mkt_date"].ToString();
                r["Remark"] = odrYr["mkt_remarks"].ToString();

                dt.Rows.Add(r);
            }
            odrYr.Close();
            DBconnect.disconnect();
            return dt;
        }


        internal DataSet pntReport(string locationCode, string documentType, string documentNo, string materialCode, string line, string engScheduleNo, List<string> documentNumbers, List<string> lineNumbers)
        {
            DataSource.DAL_DS_StockRecording ds = new DataSource.DAL_DS_StockRecording();

            ds.tbl_report.Clear();
            DBconnect.connect();

            string docNoList = string.Join(",", documentNumbers.Select(d => $"'{d}'"));
            string lineNoList = string.Join(",", lineNumbers.Select(l => $"'{l}'"));


            string query2 = $@"SELECT mvt_material_code,
                                  mvt_document_no, mvt_line
                                  FROM MMS_VIRTUAL_TRANSACTIONS
                                  WHERE mvt_loc_code = 'STK'
                                  AND mvt_document_type = 'MRQ'
                                  AND mvt_document_no IN ({docNoList})
                                  AND mvt_line IN ({lineNoList})";



            OracleDataReader odr2 = DBconnect.readtable(query2);
            while (odr2.Read())
            {
                string getreport = @"SELECT   mvt_loc_code,             
                                           mvt_document_type,         
                                           mvt_document_no,           
                                           mvt_material_code,
                                            mvt_line,
                                            updated_date,
                                            (SELECT prd_surftreat_status

                                                  FROM   pms_requisition_details
                                                  WHERE  prd_jcat
                                                         || prd_jmain
                                                         || prd_sub
                                                         || prd_spec
                                                         || prd_extc = (SELECT mdd_jcat
                                                                               || mdd_jmain
                                                                               || mdd_sub
                                                                               || mdd_spec
                                                                               || mdd_extc
                                                                        FROM   mms_document_details
                                                                        WHERE  mdd_document_no = mvt_document_no
                                                                      AND mdd_document_type = mvt_document_type)
                                                                      AND prd_material_code = mvt_material_code
                                                         AND prd_request_no = mvt_reference_no
                                                        AND prd_serial_no = mvt_engschedule_no)
                                                 AS ""surftreat_status"",
                                           mmspack.get_material_description(mvt_material_code) As description,
                                           mmspack.get_material_unit(mvt_material_code) As Unit,
                                           hrmpack.get_vname(created_by) As CreateName,
                                           mmspack.get_material_description(mvt_material_code) As MatDes,
                                           TO_CHAR(mvt_quantity, '9999990.00') AS mvt_quantity,
                                            (SELECT 
                                                        mdd_jcat 
                                                    FROM 
                                                        mms_document_details
                                                    WHERE 
                                                        mdd_document_no = mvt_document_no
                                                        AND mdd_document_type = mvt_document_type ) AS job_No,

                                           (
                                            SELECT
                                                mdd_jcat
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                               
                                        )                                                                       AS jcat,
                                        (
                                            SELECT
                                                mdd_jmain
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                
                                        )                                                                       AS jmain,
                                        (
                                            SELECT
                                                mdd_sub
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                               
                                        )                                                                       AS sub,
                                        (
                                            SELECT
                                                mdd_spec
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                                
                                        )                                                                       AS spec,
                                        (
                                            SELECT
                                                mdd_extc
                                            FROM
                                                mms_document_details
                                            WHERE
                                                    mdd_document_no = mvt_document_no
                                                AND mdd_document_type = mvt_document_type
                                               
                                        )                                                                       AS extc,

                                        (SELECT pul.pul_location_name  
                                            FROM pms_requisition_details prd
                                            JOIN pms_umrdelivery_location pul
                                              ON prd.prd_delivery_location = pul.pul_serial_no
                                            WHERE prd.prd_wh_code = mvt_loc_code
                                              AND prd.prd_document_type = mvt_document_type
                                              AND prd.prd_document_no = mvt_document_no
                                              AND prd.prd_material_code = mvt_material_code
                                              AND prd.prd_request_no = mvt_reference_no
                                              AND prd.prd_serial_no = mvt_engschedule_no
                                            GROUP BY pul.pul_location_name)                                        AS deliver_location_name,

                                            (SELECT 
                                               cdlpack.Get_location_name(mdd_originator_location)
                                              FROM   mms_document_details
                                              WHERE  mdd_loc_code = mvt_loc_code
                                              AND    mdd_document_type = mvt_document_type
                                              AND    mdd_document_no = mvt_document_no)                             AS originator_location_name,

                                            (SELECT 
                                                     mdd_invoice_no  
                                              FROM   mms_document_details
                                              WHERE  mdd_document_no = mvt_document_no
                                              AND    mdd_document_type = mvt_document_type
                                              )                                     AS invoice_no,
                                                    (SELECT ' '
                                                          || hrmpack.Get_bname(mdd_originator)
                                                   FROM   mms_document_details
                                                   WHERE  mdd_loc_code = mvt_loc_code
                                                    AND mdd_document_type = mvt_document_type
                                                 AND mdd_document_no = mvt_document_no) AS ""Requster"",
                                            TO_CHAR(mvt_date, 'RRRR-MM-DD') AS mvt_date,
                                           mvt_reference_type,     
                                           mvt_reference_no AS ""UMR No"",
                                           mvt_engschedule_no,
                                           mvt_remarks,
                                           (SELECT ' '
                                                        || hrmpack.Get_bname(mdd_originator)
                                                   FROM   mms_document_details
                                                  WHERE  mdd_loc_code = mvt_loc_code
                                                   AND mdd_document_type = mvt_document_type
                                                    AND mdd_document_no = mvt_document_no) AS ""Requster"",
                                              TO_CHAR(mmspack.Get_balance_quantity('STK', mvt_material_code), '9999990.00') AS ""Bal Qty""
                                              FROM     mms_virtual_transactions 
                                              WHERE    mvt_document_type = 'MRQ'    
                                              AND      nvl(mvt_quantity,0.00) = nvl(mvt_accepted_quantity,0.00)
                                              AND      mvt_date >= ADD_MONTHS(SYSDATE, -60)                          
                                              AND mvt_material_code = '" + odr2["mvt_material_code"] + @"'
                                              AND mvt_line =  '" + odr2["mvt_line"] + @"'
                                              AND mvt_loc_code = 'STK'
                                              ORDER BY mvt_document_no DESC";


                OracleDataReader odr = DBconnect.readtable(getreport);
                while (odr.Read())
                {
                    DataRow r = ds.Tables["tbl_report"].NewRow();
                    r["Date"] = odr["mvt_date"].ToString();
                    r["Sch_No"] = odr["mvt_engschedule_no"].ToString();
                    r["Mat_Code"] = odr["mvt_material_code"].ToString();
                    r["Description"] = odr["description"].ToString();
                    r["Unit"] = odr["Unit"].ToString();
                    r["Qty"] = odr["mvt_quantity"].ToString();
                    r["Job_No"] = odr["job_No"].ToString();
                    r["Delivery_Loc"] = odr["deliver_location_name"].ToString();
                    r["Surf_Status"] = odr["surftreat_status"].ToString();
                    r["UMR_No"] = odr["UMR No"].ToString();
                    r["Requester"] = odr["Requster"].ToString();
                    r["Bal_Qty"] = odr["Bal Qty"].ToString();
                    ds.Tables["tbl_report"].Rows.Add(r);
                }
            }
            return ds;
        }
    }
}
