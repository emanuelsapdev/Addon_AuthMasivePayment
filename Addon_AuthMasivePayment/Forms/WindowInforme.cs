using Addon_AuthMasivePayment.Addons.Tools;
using Addon_AuthMasivePayment.Common;
using Addon_AuthMasivePayment.Helpers;
using Addon_AuthMasivePayment.Services;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SAPbobsCOM;
using System.Xml.Linq;

namespace Addon_AuthMasivePayment.Forms
{
    public class WindowInforme
    {
        #region Atributos
        public const string frmUID = "AUTHPAY";
        public const string MenuUID = "AUTHPAY";

        private static SAPbouiCOM.Form _oForm;
        private static SAPbobsCOM.Recordset _oRec;
        #endregion


        public void OSAPB1appl_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            if ((pVal.ItemUID == Helpers_WindowInforme.sETDateFrom || pVal.ItemUID == Helpers_WindowInforme.sETDateTo)
                && pVal.ActionSuccess && pVal.EventType == BoEventTypes.et_LOST_FOCUS && pVal.FormMode == Convert.ToInt32(BoFormMode.fm_UPDATE_MODE))
            {
                _oForm = ConnectionSDK.UIAPI.Forms.Item(FormUID);
                Helpers_WindowInforme.RefreshDataAndFilterGrid(_oForm);
                Helpers_WindowInforme.SetLabelTotalFinal(_oForm);
            }

            if (pVal.ItemUID == Helpers_WindowInforme.sBTNSelectAll && pVal.ActionSuccess && pVal.EventType == BoEventTypes.et_ITEM_PRESSED)
            {
                _oForm = ConnectionSDK.UIAPI.Forms.Item(FormUID);
                Helpers_WindowInforme.SelectionOrDeselectionMasiveInGrid(_oForm, 1);
                Helpers_WindowInforme.SetLabelTotalFinal(_oForm);
            }

            if (pVal.ItemUID == Helpers_WindowInforme.sBTNDeselectAll && pVal.ActionSuccess && pVal.EventType == BoEventTypes.et_ITEM_PRESSED)
            {
                _oForm = ConnectionSDK.UIAPI.Forms.Item(FormUID);
                Helpers_WindowInforme.SelectionOrDeselectionMasiveInGrid(_oForm, 0);
                Helpers_WindowInforme.SetLabelTotalFinal(_oForm);
            }

            if (pVal.ItemUID == Helpers_WindowInforme.sBTNRefresh && pVal.ActionSuccess && pVal.EventType == BoEventTypes.et_ITEM_PRESSED)
            {
                _oForm = ConnectionSDK.UIAPI.Forms.Item(FormUID);
                Helpers_WindowInforme.RefreshDataAndFilterGrid(_oForm);
                Helpers_WindowInforme.SetLabelTotalFinal(_oForm);

            }


            if (pVal.ItemUID == Helpers_WindowInforme.sBTNAutorization && pVal.ActionSuccess && pVal.EventType == BoEventTypes.et_ITEM_PRESSED)
            {
                _oForm = ConnectionSDK.UIAPI.Forms.Item(FormUID);
                Helpers_WindowInforme.ProcessAuthorization(_oForm);
            }

            if(pVal.ItemUID == Helpers_WindowInforme.sGData && pVal.ColUID == Helpers_WindowInforme.sColNroInterno && pVal.ActionSuccess && pVal.EventType == BoEventTypes.et_DOUBLE_CLICK)
            {
                _oForm = ConnectionSDK.UIAPI.Forms.Item(FormUID);
                SAPbouiCOM.Grid oGridData = _oForm.Items.Item(Helpers_WindowInforme.sGData).Specific;
                int valueEntry = oGridData.DataTable.GetValue("Nro Interno", pVal.Row);
                ConnectionSDK.UIAPI.OpenForm(BoFormObjectEnum.fo_PurchaseInvoice, "", valueEntry.ToString());
            }

            if (pVal.ItemUID == Helpers_WindowInforme.sGData && pVal.ColUID == Helpers_WindowInforme.sColCardCode && pVal.ActionSuccess && pVal.EventType == BoEventTypes.et_DOUBLE_CLICK)
            {
                _oForm = ConnectionSDK.UIAPI.Forms.Item(FormUID);
                SAPbouiCOM.Grid oGridData = _oForm.Items.Item(Helpers_WindowInforme.sGData).Specific;
                string valueCardCode = oGridData.DataTable.GetValue(Helpers_WindowInforme.sColCardCode, pVal.Row);
                ConnectionSDK.UIAPI.OpenForm(BoFormObjectEnum.fo_BusinessPartner, "", valueCardCode);
            }

            if (pVal.ItemUID == Helpers_WindowInforme.sGData && pVal.ColUID == Helpers_WindowInforme.sColSelect && pVal.ActionSuccess && pVal.EventType == BoEventTypes.et_CLICK)
            {
                _oForm = ConnectionSDK.UIAPI.Forms.Item(FormUID);
                Helpers_WindowInforme.SetLabelTotalFinal(_oForm);
            }
        }

        public void OSAPB1appl_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        public void OSAPB1appl_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.MenuUID == MenuUID && !pVal.BeforeAction)
            {
                try
                {
                    _oForm  = Helpers_WindowInforme.LoadWindow();
                    Helpers_WindowInforme.RefreshDataAndFilterGrid(_oForm);

                    _oForm.Visible = true;
                    
                }
                catch(Exception ex)
                {
                    NotificationService.Error(ex.Message);
                }
                finally
                {
                    MarshalGC.ReleaseComObjects(_oForm);
                    GC.WaitForPendingFinalizers();
                }
            }
        }
    }
}
