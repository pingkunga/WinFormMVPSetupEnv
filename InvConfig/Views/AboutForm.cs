using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InvConfig.Views;
using InvConfig.Presenters;

namespace InvConfig.Views
{
    public partial class AboutForm : Form, IAboutView
    {
        //======== EVENT SECITON ========
        public event VoidEventHandler OnViewInitialize;
        public event VoidEventHandler OnViewFinalizeClose;
        //public event VoidEventHandler OnViewClose;

        //public event Action CloseDialog;
        //======== EVENT SECITON ========
        //VERSION | CHANGELOG | DETAIL
        private string changeLogFormat = "{0,-14} {1,-12} {2,-70} {3}";
        private const string bulletString = " \u2022";
        public AboutForm()
        {
            InitializeComponent();
            BindComponent();
            GetChangeLog();
        }

        public void BindComponent()
        {
            this.btnAboutClose.Click += AboutClose_Click;
        }

        private void GetChangeLog()
        {
            StringBuilder sbChangeLog = new StringBuilder();
            sbChangeLog.AppendFormat(changeLogFormat, "VERSION 3.0.0.0", "09-JUN-2018", "Big Change", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Change Target Framework from .NET 4.0 to .NET 4.7.1", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", "Tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Add ability to write buc.properties file", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Add Field Database Port / check box write buc config", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Load BNZDBVersion", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Quick lunch application for Developer", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Improve Validation Error", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", "Tab Cryptography", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Sort Username by Ascending order", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "VERSION 2.0.0.2", "16-JAN-2015", "Tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มหน้าจอ และจัดเก็บ Windows Username & Password ลงฐานข้อมูล และ Registry", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มเมนู Validate Enviroment แต้ยังไม่มี Process การทำงาน", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION 2.0.0.1", "14-JAN-2015", "Fix Bug", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Search ข้อมูลในหน้า List Config แล้ว Error ", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Cryptography ข้อมูลโดยมีการใช้ Algorithm Enigma ", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", "Tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่ม Field RPT ODBC REPORT ", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", "Tab Invest Reg", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มเงื่อนไขในการตรวจสอบ DLL ของ TSY ว่ามีในเครื่อง หรือไม่ ", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "14-SEP-2014", "Refactor Code, Enviroment Config", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการ ImportConfig และ แก้ไข Export Config โดยใช้ Library CSVHelper ", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "13-SEP-2014", "Refactor Code, Tab Run Script[SQL SERVER]", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการทำงานของการ Run Script ในแบบ QA ", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " แก้ไข Message การแสดงผล ", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "10-SEP-2014", "Refactor Code, Tab Enviroment Setup[List Config]", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มปุ่ม Delete ไว้สำหรับลบ Config เพื่อสะดวกแก่ user  ", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "09-SEP-2014", "Refactor Code, Tab Cryptography", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เก็บ Config ของการ Encrypt ว่าเป็นแบบเก่่า หรือใหม่ลงใน Database  ", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "07-SEP-2014", "Refactor Code, แก้ไขทั่วไป", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการ Validate ข้อมูลใน Tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการแสดง Tab Run Script[SQL SERVER] สำหรับ Database SQL Server ส่วน DB2 ให้ซ่อนไว้", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการเชื่อมต่อ Database ให้รองรับการเชื่อมต่อแบบ ODBC", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มเมนูการเปิด Folder LOG/SQLScript เผื่ออยากเอาไปใช้งาน", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มเมนูการ Export Config ออกเป็นไฟล์(.CSV)", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "04-SEP-2014", "Refactor Code, Tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการ Field IS ODBC สำหรับการ Config ที่ใช้ ODBC Connection", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการ Field Base Registry สำหรับบอกที่อยู่ในการเก็บ Config ลงในส่วนไหนของ Registry", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "31-AUG-2014", "Refactor Code, Tab Run Script[SQL SERVER] ใน menu Run Script#_", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการ Generate BAT File", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการ Checkbox 'Make change on database' เพื่อให้ผู้ใช้เลือกว่า Run Script ลง DB หรือไม่", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "31-AUG-2014", "Refactor Code, Tab Run Script[SQL SERVER]", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มใน menu Run Script in folder", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มใน menu Run Script#_ สำหรับรัน Script ตามเบอร์ที่เรียงใน Last Bulid Package", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เปิด Feature การเก็บ Script ที่เคย Run แล้วลงใน Table", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "28-AUG-2014", "Refactor Code, Tab Run Script[SQL SERVER] ให้มี Performance สูงขึ้น", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "27-AUG-2014", "Refactor Code, แก้ Bug ใน Tab Run Script[SQL SERVER]", Environment.NewLine);
          

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "24-AUG-2014", "Refactor Code, Tab Run Script[SQL SERVER]", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มใน menu Restore Database Username & Password", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มใน menu Blur User Data", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มการ Default ข้อมูลจาก Tab Enviroment Setup ไปยัง Tab อื่นๆ ", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "22-AUG-2014", "Refactor Code, Tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่ม Timer ตรวจสอบสถานะของ Interface Service", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " เพิ่มปุ่ม START/STOP Interface Service", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "21-AUG-2014", "Refactor Code, fix old bug", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "15-AUG-2014", "Refactor Code, ทำในส่วนของ Tab Cryptography", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Encrypt/Decrypt Interface BAT Task", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "14-AUG-2014", "Refactor Code, ทำในส่วนการติดต่อ Database", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " ใน Tab Enviroment Setup เพิ่มการ Test Connection", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " ใน Tab Cryptography เพิ่มการดึงข้อมูล Username + Password", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "12-AUG-2014", "Refactor Code, ทำในส่วนของ Tab Cryptography", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "11-AUG-2014", "Refactor Code, ทำในส่วนของ Tab Invest Reg", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "10-AUG-2014", "Refactor Code, ทำในส่วนของ List Enviroment Config", Environment.NewLine);
            
            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "08-AUG-2014", "Refactor Code, ใช้ binary serialize object เก็บข้อมูลแทน Entity Framework", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " ทำเฉพาะในส่วนของ Tab Enviroment Setup ", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "22-JUN-2014", "Modified Entity framwork 6 ", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Change auto generate code from 'ObjectContext' to 'DbContext'", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "21-JUN-2014", "Modified tab Run Script[SQL Server], add features", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Add Script Log & Result Output", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "14-JUN-2014", "Modified tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Add field: bnzLastUpdateScript เก็บเบอร์ที่ Run Script ล่าสุดไว้", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Add field: bnzLastUpdateScriptDate เก็บวัน เวลาที่ Run Script ล่าสุดไว้", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", "Modified tab Run Script[SQL Server]", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Modified feature Run Script#__(Run script by script number) เก็บ Log และ Scriptที่รันล่าสุดไว้", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "12-JUN-2014", "Modified tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Add field: Config Category", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "05-JUN-2014", "Upgrade Entity framwork 4 to 6", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "03-JUN-2014", "Modified List Enviroment Config, add feature", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Auto Install/Uninstall Interface Service", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "28-MAY-2014", "Modified List Enviroment Config, add feature", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Auto Register/Unregister DLL & OCX", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "26-MAY-2014", "Modified tab Enviroment Setup", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Add field: Config Remark", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "23-MAY-2014", "Modified tab InvReg, add features", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " List of all Product Version in path(ดูเวอร์ชันของ Software ใน path ที่ระบุไว้)", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", "Modified tab InvReg, add features", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " List of all Bonanza Component in registry(ดูเวอร์ชันของ Software ที่ Registry ในของระบบ)", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "21-MAY-2014", "Modified tab Run Script[SQL Server], add features", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Blur user data(เบลอข้อมูล เพื่อป้องกันข้อมูลสำคัญ)", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "16-MAY-2014", "Add tab InvReg", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Register DLLs", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Unregister DLLs", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Register OCX", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " Unregister OCX", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "25-APR-2014", "Modified tab Cryptography, add features", Environment.NewLine);
            sbChangeLog.AppendFormat(changeLogFormat, "", "", bulletString + " แก้ Bug ใน Cryptography ของ Site เก่าๆ", Environment.NewLine);

            sbChangeLog.AppendFormat(changeLogFormat, "VERSION x.x.x.x", "23-APR-2014", "Release first version", Environment.NewLine);

            txtChangeLog.Text = sbChangeLog.ToString();
            this.ActiveControl = btnAboutClose;
        }



        private void AboutClose_Click(object sender, EventArgs e)
        {
            if (this.OnViewFinalizeClose != null)
            {
                this.OnViewFinalizeClose();
            }
            //this.Dispose();
        }

        public void OnShowView() 
        {
            ShowDialog(); 
        }

        public void CloseView()
        {
            this.Dispose();
        }

        public void ShowView()
        {
            this.ShowDialog();
        }
        public void RaiseVoidEvent(VoidEventHandler @event)
        {

        }
    }
}
