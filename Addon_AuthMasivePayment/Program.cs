using Addon_AuthMasivePayment.Common;
using SAPbouiCOM;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            ConnectionSDK.Singlenton();

            if (ConnectionSDK.Connected)
            {
                ConnectionSDK.UIAPI?.StatusBar.SetText("Addon_AuthMasivePayment Connected", BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Success);
                var oEvents = new Events();
                var oExecutions = new ExecutionsApp();

                GC.KeepAlive(oEvents);
                GC.KeepAlive(oExecutions);

                System.Windows.Forms.Application.Run();
            }
        }
        catch (Exception ex)
        {
            ConnectionSDK.UIAPI?.MessageBox($"Fatal error in Addon_AuthMasivePayment: {ex.Message}");
        }
    }
}