using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Windows.Forms;

namespace Addon_AuthMasivePayment.Common
{
    public class ConnectionSDK
    {
        protected static SAPbouiCOM.Application _UIAPI;
        protected static SAPbobsCOM.Company _DIAPI;
        public static SAPbouiCOM.Application UIAPI => _UIAPI ?? throw new Exception("UIAPI no definido");
        public static SAPbobsCOM.Company DIAPI => _DIAPI ?? throw new Exception("DIAPI no definido");

        public static bool Connected => _UIAPI != null && _DIAPI.Connected;


        public static void Singlenton()
        {
            _UIAPI = GetApplication();

            if (_UIAPI != null)
            {
                _DIAPI = _UIAPI.Company.GetDICompany();
            }
        }



        public static SAPbouiCOM.Application GetApplication()
        {
            SboGuiApi api = new SboGuiApi()
            {
                AddonIdentifier = "Addon_AuthMasivePayment"
            };

            string[] commands = Environment.GetCommandLineArgs();
            string strConnection;

            if (commands.Length == 1) strConnection = commands[0];
            else
                if (commands[0].LastIndexOf("\\") > 0)
            {
                strConnection = commands[1];
            }
            else
            {
                strConnection = commands[0];
            }

            try
            {
                //api.Connect("0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056");
                api.Connect(strConnection);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return api.GetApplication();
        }
    }
}
