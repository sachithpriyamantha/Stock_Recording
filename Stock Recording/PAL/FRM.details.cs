using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Stock_Recording.PAL
{
    public partial class FRM : DevExpress.XtraEditors.XtraForm
    {
        BLL.BLL_CS_StockRecoding bll = new BLL.BLL_CS_StockRecoding();


        private object test;
        private object lac;
        private object docno;
        private object doct;
        private object line;
        private object lac1;
        private object whcode;
        private object docno1;
        private object doct1;


        private bool isFirstCase;
        public FRM(string test, string lac, string docno, string doct, string line)
        {
            InitializeComponent();
            this.test = test;
            this.lac = lac;
            this.docno = docno;
            this.doct = doct;
            this.line = line;
            this.isFirstCase = true;
        }

        

        public FRM(string lac1, string whcode, string docno1, string doct1)
        {
            InitializeComponent();
            this.lac1 = lac1;
            this.whcode = whcode;
            this.docno1 = docno1;
            this.doct1 = doct1;
            this.isFirstCase = false;
        }

        

        private void FRM_Load(object sender, EventArgs e)
        {
            //gc_krvirtual.DataSource = bll.LoadDetails(test, lac, docno, doct, line);
            //gc_krvirtual.DataSource = bll.LoadDetails1(lac1, whcode, docno1, doct1);

            if (isFirstCase)
            {
                gc_krvirtual.DataSource = bll.LoadDetails(test, lac, docno, doct, line);
            }
            else
            {
                gc_krvirtual.DataSource = bll.LoadDetails1(lac1, whcode, docno1, doct1);
            }
        }

        private void gc_krvirtual_Click(object sender, EventArgs e)
        {

            
        }

        private void gc_krvirtual_Click_1(object sender, EventArgs e)
        {

        }
    }
}