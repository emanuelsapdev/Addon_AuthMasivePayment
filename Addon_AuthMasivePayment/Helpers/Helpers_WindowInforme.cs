using Addon_AuthMasivePayment.Addons.Tools;
using Addon_AuthMasivePayment.Common;
using Addon_AuthMasivePayment.Forms;
using Addon_AuthMasivePayment.Services;
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data.Common;

namespace Addon_AuthMasivePayment.Helpers
{
    public class Helpers_WindowInforme
    {
        #region Atributos

        private static int _countForm = 0;
        public const string sGData = "G_DATA";
        public const string sETDateFrom = "Item_6";
        public const string sETDateTo = "Item_7";
        public const string sBTNSelectAll = "Item_2";
        public const string sBTNDeselectAll = "Item_4";
        public const string sBTNRefresh = "Item_1";
        public const string sBTNAutorization = "Item_0";
        public const string sColNroInterno = "Nro Interno";
        public const string sColCardCode = "Código SN";

        #endregion 

        private static Recordset _oRec;
        private static SAPbouiCOM.Form _oForm;

        public static SAPbouiCOM.Form LoadWindow()
        {
            FormCreationParams p = ConnectionSDK.UIAPI.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
            p.XmlData = File.ReadAllText(Environment.CurrentDirectory + "\\WindowAuthPayments.xml");
            p.FormType = "AUTHPAY";
            p.UniqueID = $"AUTHPAY{_countForm}";
            _countForm++;
            SAPbouiCOM.Form oForm = ConnectionSDK.UIAPI.Forms.AddEx(p);
            return oForm;
        }

        public static void CreateMenu()
        {
            try
            {
                Menus Menus = ConnectionSDK.UIAPI.Menus;
                Menus = Menus.Item("43537").SubMenus;  // MENU PRINCIPAL "GESTION DE BANCOS"
                string menuUID = WindowInforme.MenuUID;
                if (Menus.Exists(menuUID))
                {
                    Menus.Remove(Menus.Item(menuUID));
                }

                SAPbouiCOM.MenuItem oMenu = Menus.Add(menuUID, "Addon - Autorizador de Pagos", BoMenuType.mt_STRING, 0);
                oMenu.Enabled = true;

            }
            catch (Exception ex)
            {
                NotificationService.Error($"CreateMenu Error -> {ex.Message}");
            }

        }

        public static void LoadDataInGridBalanceClients(SAPbouiCOM.Form pForm)
        {
            try
            {
                string nameDataTable = WindowInforme.frmUID + WindowInforme.MenuUID;
                DataTable dataTableGridData;
                DataTables formDataTables = pForm.DataSources.DataTables;

                if (formDataTables.Count != 0)
                {
                    dataTableGridData = formDataTables.Item(nameDataTable);
                    dataTableGridData.Clear();
                }
                else
                {
                    dataTableGridData = formDataTables.Add(nameDataTable);
                }

                EditText ETDateFrom = pForm.Items.Item(sETDateFrom).Specific;
                EditText ETDateTo = pForm.Items.Item(sETDateTo).Specific;

                string valueEtDateFrom = ETDateFrom.Value;
                string valueEtDateTo = ETDateTo.Value;

                string filterDateFrom = string.Empty;
                string filterDateTo = string.Empty;

                if (string.IsNullOrEmpty(valueEtDateFrom))
                {
                    filterDateFrom = "19990101";
                }
                else
                {
                    filterDateFrom = valueEtDateFrom;
                }


                if (string.IsNullOrEmpty(valueEtDateTo))
                {
                    filterDateTo = "20601231";
                }
                else
                {
                    filterDateTo = valueEtDateTo;
                }

                string queryGrid = $@"SELECT * FROM ITPS_ADDON_AUTH_PAYMENTS WHERE ""Fecha Contabilización"" BETWEEN '{filterDateFrom}' AND '{filterDateTo}'";
                dataTableGridData.ExecuteQuery(queryGrid);

                Grid oGridData = pForm.Items.Item(sGData).Specific;
                oGridData.DataTable = dataTableGridData;
                oGridData.AutoResizeColumns();

                ActiveColumns(oGridData);
                ResetRowsBackColors(oGridData);
                SetTextStyleInColumns(oGridData);

            }
            catch
            {
                throw;
            }
            finally
            {
                MarshalGC.ReleaseComObjects(_oForm);
                GC.WaitForPendingFinalizers();
            }
        }

        private static void ResetRowsBackColors(Grid pGrid)
        {
            for (int i = 0; i < pGrid.Rows.Count; i++)
            {
                pGrid.CommonSetting.SetRowBackColor(i + 1, 16777215);
            }
        }

        private static void ActiveColumns(Grid pGrid)
        {
            for (int i = 0; i < pGrid.Columns.Count; i++)
            {
                GridColumn col = pGrid.Columns.Item(i);
                string[] editables = { "Seleccionar" };
                col.Editable = editables.Contains(col.UniqueID);
            }
        }

        private static void SetTextStyleInColumns(Grid pGrid)
        {
            SAPbouiCOM.GridColumn colNroInterno = pGrid.Columns.Item(sColNroInterno);
            SAPbouiCOM.GridColumn colCardCode = pGrid.Columns.Item(sColCardCode);

            colNroInterno.TextStyle = Convert.ToInt32(BoFontStyle.fs_Bold);
            colCardCode.TextStyle = Convert.ToInt32(BoFontStyle.fs_Bold);
        }

        private static void SetRowBackColorSuccess(Grid pGrid, List<int> entriesSuccess)
        {
            for (int i = 0; i < pGrid.Rows.Count; i++)
            {
                int valueEntry = pGrid.DataTable.GetValue("Nro Interno", i);

                if (entriesSuccess.Contains(valueEntry) && valueEntry != 0)
                {
                    pGrid.CommonSetting.SetRowBackColor(i + 1, 65280);
                }
            }
        }

        public static void ConverterColumnGroupOtherToCheckBox(SAPbouiCOM.Form pForm)
        {
            try
            {
                Grid oGridData = pForm.Items.Item(sGData).Specific;
                oGridData.Columns.Item("Seleccionar").Type = BoGridColumnType.gct_CheckBox;
            }
            catch
            {
                throw;
            }
            finally
            {
                MarshalGC.ReleaseComObjects(_oForm);
                GC.WaitForPendingFinalizers();
            }
        }

        public static void SelectionOrDeselectionMasiveInGrid(SAPbouiCOM.Form pForm, int selectOrDeselect)
        {
            try
            {
                pForm.Freeze(true);
                Grid oGridData = pForm.Items.Item(sGData).Specific;
                for (int i = 0; i < oGridData.Rows.Count; i++)
                {
                    oGridData.DataTable.Columns.Item("Seleccionar").Cells.Item(i).Value = selectOrDeselect == 1 ? "Y" : "N";

                }
                pForm.Freeze(false);
            }
            catch
            {
                throw;
            }
            
        }

        public static void RefreshDataAndFilterGrid(SAPbouiCOM.Form pForm)
        {
            try
            {
                pForm.Freeze(true);
                LoadDataInGridBalanceClients(pForm);
                ConverterColumnGroupOtherToCheckBox(pForm);
                pForm.Freeze(false);

        }
            catch
            {
                throw;
            }
        }

        public static void ProcessAuthorization(SAPbouiCOM.Form pForm)
        {
            try
            {
                _oRec = ConnectionSDK.DIAPI.GetBusinessObject(BoObjectTypes.BoRecordset);

                SAPbouiCOM.Grid oGridData = pForm.Items.Item(sGData).Specific;

                var listEntries = new List<int>();
                for (int i = 0; i < oGridData.Rows.Count; i++)
                {
                    string valueSelect = oGridData.DataTable.GetValue("Seleccionar", i);
                    if (valueSelect == "Y")
                    {
                        int valueEntry = oGridData.DataTable.GetValue("Nro Interno", i);
                        listEntries.Add(valueEntry);
                    }
                }

                if (listEntries.Count > 0)
                {
                    string entries = string.Join(", ", listEntries);
                    string query = $@"UPDATE OPCH SET ""U_PagoAutorizado"" = 'Y' WHERE ""DocEntry"" IN ( {entries} ) AND ""U_PagoAutorizado"" = 'N';";
                    _oRec.DoQuery(query);
                }

                SetRowBackColorSuccess(oGridData, listEntries);
            }
            catch
            {
                throw;
            }
            finally
            {
                MarshalGC.ReleaseComObjects(_oForm, _oRec);
                GC.WaitForPendingFinalizers();
            }

        }
    }
}
