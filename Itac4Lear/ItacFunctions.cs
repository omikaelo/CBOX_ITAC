using com.itac.mes.imsapi.client.dotnet;
using com.itac.mes.imsapi.domain.container;
using System;

namespace Itac4Lear
{
    public static class ItacFunctions
    {
        private static IMSApiDotNet _imsapi;
        private static IMSApiSessionContextStruct _sessionContext;
        private static string _stationNumber;

        public static bool Init()
        {
            return Logger.Init();
        }

        public static bool Term()
        {
            return Logger.Term();
        }

        public static string Error
        {
            get; internal set;
        }

        public static bool OpenConnection(string clusternodes, string stationNumber, string stationPassword, string user, string password, string client, string registrationType, string systemIdentifier)
        {
            Logger.Log.Info((object)"OpenConnection(): method called .");
            Logger.Log.Info((object)string.Format("OpenConnection( \n ClusterNodes: {0}, \n stationNumber: {1}, \n stationPassword: {2}, \n user: {3}, \n password: {4}, \n client: {5}, \n registrationType: {6},\n systemIdentifier: {7})", (object)clusternodes, (object)stationNumber, (object)stationPassword, (object)user, (object)password, (object)client, (object)registrationType, (object)systemIdentifier));
            ItacFunctions._imsapi = (IMSApiDotNet)null;
            ItacFunctions._sessionContext = (IMSApiSessionContextStruct)null;
            ItacFunctions._stationNumber = (string)null;
            bool flag;
            try
            {
                IMSApiDotNetBase.setProperty("itac.appid", "StationTestClient");
                IMSApiDotNetBase.setProperty("itac.artes.clusternodes", clusternodes);
                ItacFunctions._imsapi = IMSApiDotNet.loadLibrary();
                string str = "";
                int libraryVersion = ((IMSApiDotNetBase)ItacFunctions._imsapi).imsapiGetLibraryVersion(out str);
                if (libraryVersion != 0)
                    ItacFunctions.printErrorText(libraryVersion);
                Logger.Log.Info((object)str);
                int resultValue = ((IMSApiDotNetBase)ItacFunctions._imsapi).imsapiInit();
                if (resultValue != 0)
                {
                    Logger.Log.Error((object)"OpenConnection():IMSApi init failed!");
                    ItacFunctions.printErrorText(resultValue);
                    flag = false;
                }
                else if (!ItacFunctions.itacLogin(stationNumber, stationPassword, user, password, client, registrationType, systemIdentifier))
                {
                    Logger.Log.Error((object)"OpenConnection(): itacLogin failed!");
                    flag = false;
                }
                else
                {
                    ItacFunctions._stationNumber = stationNumber;
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("OpenConnection(): return with {0}", (object)flag));
            return flag;
        }

        public static bool CloseConnection()
        {
            Logger.Log.Info((object)"CloseConnection() method called .");
            bool flag;
            try
            {
                int resultValue1 = ItacFunctions._imsapi.regLogout(ItacFunctions._sessionContext);
                if (resultValue1 != 0)
                {
                    Logger.Log.Error((object)"CloseConnection(): _imsapi.regLogout error .");
                    ItacFunctions.printErrorText(resultValue1);
                    flag = false;
                }
                else
                {
                    int resultValue2 = ((IMSApiDotNetBase)ItacFunctions._imsapi).imsapiShutdown();
                    if (resultValue2 != 0)
                    {
                        Logger.Log.Error((object)"CloseConnection(): _imsapi.imsapiShutdown error .");
                        ItacFunctions.printErrorText(resultValue2);
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                        ItacFunctions._imsapi = (IMSApiDotNet)null;
                        ItacFunctions._sessionContext = (IMSApiSessionContextStruct)null;
                        ItacFunctions._stationNumber = (string)null;
                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("CloseConnection(): return with {0}", (object)flag));
            return flag;
        }

        public static bool GetSerialNumberHistoryData(string serialNumber, string serialNumberPos, int processLayer, out string[] bookingResultValues)
        {
            Error = "";
            Logger.Log.Info((object)("GetSerialNumberHistoryData(serialNumber: " + serialNumber + " ): method called ."));
            bool flag;
            string workOrderNumber = "";
            string partNumber = "";
            string customerPartNumber = "";
            string partDesc = "";
            string quantity = "";
            long lastReportDate = 0;
            string lotNumber = "";
            int isLocked = 0;
            string[] bookingResultKeys = new string[] { "STATION_DESC", "BOOK_DATE", "BOOK_STATE", "STATION_NUMBER", "SERIAL_NUMBER" };
            bookingResultValues = new string[] { };
            string[] failureDataResultValues = new string[] { };
            string[] failureSlipDataResultValues = new string[] { };
            string[] measureDataResultValues = new string[] { };
            try
            {
                Logger.Log.Info((object)string.Format("Calling: _imsapi.trGetSerialNumberHistoryData (\n serialNumber: {0}, \n serialNumberPos: {1}, \n serialNumberResultKeys: {2}.", (object)serialNumber, (object)serialNumberPos, (object)string.Join(", ", bookingResultKeys)));
                int status = ItacFunctions._imsapi.trGetSerialNumberHistoryData(ItacFunctions._sessionContext, ItacFunctions._stationNumber, serialNumber, serialNumberPos, processLayer, 1, 0, bookingResultKeys, out bookingResultValues, new string[] { }, out failureDataResultValues, new string[] { }, out failureSlipDataResultValues, new string[] { }, out measureDataResultValues, out workOrderNumber, out partNumber, out customerPartNumber, out partDesc, out quantity, out lastReportDate, out lotNumber, out isLocked);
                if (status != 0)
                {
                    Logger.Log.Error((object)"GetSerialNumberHistoryData(): _imsapi.trGetSerialNumberHistoryData failed");
                    ItacFunctions.printErrorText(status);
                    flag = false;
                }
                else
                {
                    flag = true;
                    if (bookingResultValues.Length > 0)
                    {
                        Logger.Log.Info((object)string.Format("_imsapi.trGetSerialNumberHistoryData()) returns value: \n BOOK_STATE: {0}", ""));
                    }
                    else
                        Logger.Log.Info((object)string.Format("_imsapi.trGetSerialNumberHistoryData()) returns value: \n BOOK_STATE: {0}", ""));
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str1 = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str1);
                Error = str1;
            }
            Logger.Log.Info((object)string.Format("GetSerialNumberHistoryData(): return with {0}", (object)flag));
            return flag;
        }

        public static bool GetSerialNumberInfo(string serialNumber, out string SERIAL_NUMBER, out string SERIAL_NUMBER_POS)
        {
            Logger.Log.Info((object)("GetSerialNumberInfo(serialNumber: " + serialNumber + " ): method called ."));
            SERIAL_NUMBER = "";
            SERIAL_NUMBER_POS = "";
            bool flag;
            try
            {
                string str = "-1";
                string[] strArray1 = new string[2]
                {
          "SERIAL_NUMBER",
          "SERIAL_NUMBER_POS"
                };
                string[] strArray2 = (string[])null;
                Logger.Log.Info((object)string.Format("Calling: _imsapi.trGetSerialNumberInfo (\n serialNumber: {0}, \n serialNumberPos: {1}, \n serialNumberResultKeys: {2}.", (object)serialNumber, (object)str, (object)string.Join(", ", strArray1)));
                int serialNumberInfo = ItacFunctions._imsapi.trGetSerialNumberInfo(ItacFunctions._sessionContext, ItacFunctions._stationNumber, serialNumber, str, strArray1, out strArray2);
                if (serialNumberInfo != 0)
                {
                    Logger.Log.Error((object)"GetSerialNumberInfo(): _imsapi.trGetSerialNumberInfo failed");
                    ItacFunctions.printErrorText(serialNumberInfo);
                    flag = false;
                }
                else
                {
                    SERIAL_NUMBER = strArray2[0];
                    SERIAL_NUMBER_POS = strArray2[1];
                    flag = true;
                    Logger.Log.Info((object)string.Format("_imsapi.trGetSerialNumberInfo() returns value: \n SERIAL_NUMBER: {0}, \n SERIAL_NUMBER_POS: {1}", (object)SERIAL_NUMBER, (object)SERIAL_NUMBER_POS));
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("GetSerialNumberInfo(): return with {0}", (object)flag));
            return flag;
        }

        public static bool GetStationSetting(out string BOM_VERSION, out string PART_NUMBER, out string PROCESS_LAYER, out string PROCESS_VERSION, out string WORKORDER_NUMBER)
        {
            Logger.Log.Info((object)"GetStationSetting(): Method called .");
            BOM_VERSION = "";
            PART_NUMBER = "";
            PROCESS_LAYER = "";
            PROCESS_VERSION = "";
            WORKORDER_NUMBER = "";
            bool flag;
            try
            {
                string[] strArray1 = (string[])null;
                string[] strArray2 = new string[5]
                {
          "BOM_VERSION",
          "PART_NUMBER",
          "PROCESS_LAYER",
          "PROCESS_VERSION",
          "WORKORDER_NUMBER"
                };
                Logger.Log.Info((object)string.Format("Calling: _imsapi.trGetStationSetting(\n _stationNumber: {0}, \n myResultKeys: {1}", (object)ItacFunctions._stationNumber, (object)string.Join(", ", strArray2)));
                int stationSetting = ItacFunctions._imsapi.trGetStationSetting(ItacFunctions._sessionContext, ItacFunctions._stationNumber, strArray2, out strArray1);
                if (stationSetting != 0)
                {
                    Logger.Log.Error((object)"GetStationSetting(): _imsapi.trGetStationSetting error .");
                    ItacFunctions.printErrorText(stationSetting);
                    flag = false;
                }
                else
                {
                    BOM_VERSION = strArray1[0];
                    PART_NUMBER = strArray1[1];
                    PROCESS_LAYER = strArray1[2];
                    PROCESS_VERSION = strArray1[3];
                    WORKORDER_NUMBER = strArray1[4];
                    flag = true;
                    Logger.Log.Info((object)string.Format("_imsapi.trGetStationSetting() return values : \n BOM_VERSION: {0}, \n PART_NUMBER: {1}, \n PROCESS_LAYER: {2}, \n PROCESS_VERSION: {3}, \n WORKORDER_NUMBER: {4}", (object)BOM_VERSION, (object)PART_NUMBER, (object)PROCESS_LAYER, (object)PROCESS_VERSION, (object)WORKORDER_NUMBER));
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("GetStationSetting(): return with {0}", (object)flag));
            return flag;
        }

        public static bool GetRecipeMeasurementList(string PART_NUMBER, string BOM_VERSION, string PROCESS_VERSION, ref string[] MEASURE_LIST)
        {
            Logger.Log.Info((object)string.Format("Method called: GetRecipeMeasurementList():"));
            MEASURE_LIST = new string[0];
            string[] OUTPUT_VECTOR;
            bool recipeData = ItacFunctions.getRecipeData("-1", new string[1]
            {
        "MEASURE_NAME"
            }, PART_NUMBER, BOM_VERSION, PROCESS_VERSION, out OUTPUT_VECTOR);
            if (recipeData)
            {
                MEASURE_LIST = OUTPUT_VECTOR;
                Logger.Log.Info((object)string.Format("GetRecipeMeasurementList(): MEASURE_LIST: \n {0} ", (object)string.Join(", \n", MEASURE_LIST)));
            }
            Logger.Log.Info((object)string.Format("GetRecipeMeasurementList(): return with {0}", (object)recipeData));
            return recipeData;
        }

        public static bool GetRecipeData(string MEASURE_NAME, string PART_NUMBER, string BOM_VERSION, string PROCESS_VERSION, out string NOMINAL_VALUE)
        {
            Logger.Log.Info((object)string.Format("Method called: GetRecipeData():"));
            NOMINAL_VALUE = "";
            string[] OUTPUT_VECTOR;
            bool recipeData = ItacFunctions.getRecipeData(MEASURE_NAME, new string[1]
            {
        "NOMINAL"
            }, PART_NUMBER, BOM_VERSION, PROCESS_VERSION, out OUTPUT_VECTOR);
            if (recipeData)
            {
                NOMINAL_VALUE = OUTPUT_VECTOR[0];
                Logger.Log.Info((object)string.Format("GetRecipeData(): MEASURE_NAME:{0} , NOMINAL_VALUE: {1}", (object)MEASURE_NAME, (object)NOMINAL_VALUE));
            }
            Logger.Log.Info((object)string.Format("GetRecipeData(): return with {0}", (object)recipeData));
            return recipeData;
        }

        public static bool GetRecipeName(string PART_NUMBER, string BOM_VERSION, string PROCESS_VERSION, out string PROGRAM_NAME)
        {
            Logger.Log.Info((object)"GetRecipeName(): Method called .");
            PROGRAM_NAME = "";
            string[] OUTPUT_VECTOR;
            bool recipeData = ItacFunctions.getRecipeData("PROGRAM_NAME", new string[1]
            {
        "NOMINAL"
            }, PART_NUMBER, BOM_VERSION, PROCESS_VERSION, out OUTPUT_VECTOR);
            if (recipeData)
            {
                PROGRAM_NAME = OUTPUT_VECTOR[0];
                Logger.Log.Info((object)string.Format("GetRecipeName(): PROGRAM_NAME: {0}", (object)PROGRAM_NAME));
            }
            Logger.Log.Info((object)string.Format("GetRecipeName(): return with {0}", (object)recipeData));
            return recipeData;
        }

        public static bool GetMesOnOff(string PART_NUMBER, string BOM_VERSION, string PROCESS_VERSION, out string MES)
        {
            Logger.Log.Info((object)"GetMesOnOff(): Method called .");
            MES = "";
            string[] OUTPUT_VECTOR;
            bool recipeData = ItacFunctions.getRecipeData("MES", new string[1]
            {
        "NOMINAL"
            }, PART_NUMBER, BOM_VERSION, PROCESS_VERSION, out OUTPUT_VECTOR);
            if (recipeData)
            {
                MES = OUTPUT_VECTOR[0];
                Logger.Log.Info((object)string.Format("GetMesOnOff(): MES: {0}", (object)MES));
            }
            Logger.Log.Info((object)string.Format("GetMesOnOff(): return with {0}", (object)recipeData));
            return recipeData;
        }

        public static bool CheckSerialNumberState(string SERIAL_NUMBER, string SERIAL_NUMBER_POS, string PROCESS_LAYER, out string ERROR_CODE, out string SERIAL_NUMBER_STATE)
        {
            Logger.Log.Info((object)"CheckSerialNumberState(): method called .");
            ERROR_CODE = "";
            SERIAL_NUMBER_STATE = "";
            bool flag;
            try
            {
                int num = int.Parse(PROCESS_LAYER);
                string[] strArray1 = new string[2]
                {
          "ERROR_CODE",
          "SERIAL_NUMBER_STATE"
                };
                string[] strArray2 = (string[])null;
                Logger.Log.Info((object)string.Format("Calling: _imsapi.trCheckSerialNumberState(\n _stationNumber: {0},\n processLayer: {1},\n 0, \n SERIAL_NUMBER: {2},\n SERIAL_NUMBER_POS: {3},\n serialNumberStateResultKeys : {4})", (object)ItacFunctions._stationNumber, (object)num, (object)SERIAL_NUMBER, (object)SERIAL_NUMBER_POS, (object)string.Join(", ", strArray1)));
                int resultValue = ItacFunctions._imsapi.trCheckSerialNumberState(ItacFunctions._sessionContext, ItacFunctions._stationNumber, num, 0, SERIAL_NUMBER, SERIAL_NUMBER_POS, strArray1, out strArray2);
                if (resultValue != 0)
                {
                    Logger.Log.Error((object)"CheckSerialNumberState(): _imsapi.trCheckSerialNumberState failed");
                    ItacFunctions.printErrorText(resultValue);
                    flag = false;
                }
                else
                {
                    ERROR_CODE = strArray2[0];
                    SERIAL_NUMBER_STATE = strArray2[1];
                    flag = true;
                    Logger.Log.Info((object)string.Format("_imsapi.trCheckSerialNumberState() return values: \n ERROR_CODE: {0}, \n SERIAL_NUMBER_STATE: {1}", (object)ERROR_CODE, (object)SERIAL_NUMBER_STATE));
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("CheckSerialNumberState(): return with {0}", (object)flag));
            return flag;
        }

        public static bool VerifyMergeProduct(string SERIAL_NUMBER, string PART_NUMBER, string BOM_VERSION)
        {
            Logger.Log.Info((object)"VerifyMergeProduct(): Method called .");
            bool flag;
            try
            {
                int num = int.Parse(BOM_VERSION);
                Logger.Log.Info((object)string.Format("Calling: _imsapi.trVerifyMergeProduct( \n _stationNumber: {0},\n PART_NUMBER: {1},\n bomVersion: {2},\n -1, \n SERIAL_NUMBER: {3},\n 0 )", (object)ItacFunctions._stationNumber, (object)PART_NUMBER, (object)num, (object)SERIAL_NUMBER));
                int resultValue = ItacFunctions._imsapi.trVerifyMergeProduct(ItacFunctions._sessionContext, ItacFunctions._stationNumber, PART_NUMBER, num, "-1", SERIAL_NUMBER, 0);
                if (resultValue != 0)
                {
                    Logger.Log.Error((object)"VerifyMergeProduct(): _imsapi.trVerifyMergeProduct error .");
                    ItacFunctions.printErrorText(resultValue);
                    flag = false;
                }
                else
                    flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("VerifyMergeProduct(): return with {0}", (object)flag));
            return flag;
        }

        public static bool AssignSerialNumberMergeAndUploadState(string PROCESS_LAYER, string SERIAL_NUMBER, string SERIAL_NUMBER_POS)
        {
            Logger.Log.Info((object)string.Format("AssignSerialNumberMergeAndUploadState(): PROCESS_LAYER: {0}, SERIAL_NUMBER: {1}, SERIAL_NUMBER_POS: {2}", (object)PROCESS_LAYER, (object)SERIAL_NUMBER, (object)SERIAL_NUMBER_POS));
            SerialNumberData serialNumberData = new SerialNumberData();
            serialNumberData.serialNumber = SERIAL_NUMBER;
            serialNumberData.serialNumberPos = SERIAL_NUMBER_POS;
            serialNumberData.serialNumberOld = "0";
            bool flag;
            try
            {
                int num = int.Parse(PROCESS_LAYER);
                Logger.Log.Info((object)string.Format("Calling: _imsapi.trAssignSerialNumberMergeAndUploadState( \n _stationNumber: {0},\n processLayer: {1},\n serialNumber: {2},\n serialNumberRefPos: {3},\n KEY.serialNumber: {4},\n KEY.serialNumberPos: {5},\n KEY.serialNumberOld: {6},\n serialNumberSlave: {7},\n doUploadState: 0,\n serialNumberState; 0,\n multiPanel: 1 )", (object)ItacFunctions._stationNumber, (object)num, (object)SERIAL_NUMBER, (object)SERIAL_NUMBER_POS, (object)serialNumberData.serialNumber, (object)serialNumberData.serialNumberPos, (object)serialNumberData.serialNumberOld, (object)SERIAL_NUMBER));
                int resultValue = ItacFunctions._imsapi.trAssignSerialNumberMergeAndUploadState(ItacFunctions._sessionContext, ItacFunctions._stationNumber, num, SERIAL_NUMBER, SERIAL_NUMBER_POS, new SerialNumberData[1]
                {
          serialNumberData
                }, SERIAL_NUMBER, 0, 0, 0);
                if (resultValue != 0)
                {
                    Logger.Log.Error((object)"AssignSerialNumberMergeAndUploadState(): _imsapi.trAssignSerialNumberMergeAndUploadState failed .");
                    ItacFunctions.printErrorText(resultValue);
                    flag = false;
                }
                else
                    flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("VerifyMergeProduct(): return with {0}", (object)flag));
            return flag;
        }

        public static bool UploadFailureAndResultData(string PROCESS_LAYER, string SERIAL_NUMBER, string SERIAL_NUMBER_POS, int serialNumberState, float cycleTime, string[] measureValues, string[] failureValues, string[] failureSlipValues)
        {
            Logger.Log.Info((object)string.Format("Method called: UploadFailureAndResultData( \n PROCESS_LAYER: {0}, SERIAL_NUMBER: {1}, SERIAL_NUMBER_POS: {2}, serialNumberState: {3}, cycleTime: {4})", (object)PROCESS_LAYER, (object)SERIAL_NUMBER, (object)SERIAL_NUMBER_POS, (object)serialNumberState, (object)cycleTime));
            bool flag = false;
            Logger.Log.Warn((object)"Temporarily: failureValues = new string[0] ; \n failureSlipValues = new string[0] ;");
            failureValues = new string[0];
            failureSlipValues = new string[0];
            try
            {
                if (measureValues == null)
                {
                    measureValues = new string[0];
                    Logger.Log.Warn((object)"UploadFailureAndResultData() measureValues vector null. Cosidered as empty.");
                }
                if (failureValues == null)
                {
                    failureValues = new string[0];
                    Logger.Log.Warn((object)"UploadFailureAndResultData() failureValues vector null. Cosidered as empty.");
                }
                if (failureSlipValues == null)
                {
                    failureSlipValues = new string[0];
                    Logger.Log.Warn((object)"UploadFailureAndResultData() failureSlipValues vector null. Cosidered as empty.");
                }
                int num = int.Parse(PROCESS_LAYER);
                string[] stringArray1 = new string[4]
                {
          "ERROR_CODE",
          "MEASURE_FAIL_CODE",
          "MEASURE_NAME",
          "MEASURE_VALUE"
                };
                string[] stringArray2 = new string[2]
                {
          "ERROR_CODE",
          "FAILURE_TYPE_CODE"
                };
                string[] stringArray3 = new string[2]
                {
          "ERROR_CODE",
          "TEST_STEP_NAME"
                };
                Logger.Log.Info((object)("UploadFailureAndResultData() measureKeys " + ItacFunctions.alignResultString4Log(stringArray1, 4) + ItacFunctions.alignResultString4Log(measureValues, 4)));
                Logger.Log.Info((object)("UploadFailureAndResultData() failureKeys" + ItacFunctions.alignResultString4Log(stringArray2, 2) + ItacFunctions.alignResultString4Log(failureValues, 2)));
                Logger.Log.Info((object)("UploadFailureAndResultData() failureSlipKeys" + ItacFunctions.alignResultString4Log(stringArray3, 2) + ItacFunctions.alignResultString4Log(failureSlipValues, 2)));
                string[] stringArray4 = (string[])null;
                string[] stringArray5 = (string[])null;
                string[] strArray = (string[])null;
                Logger.Log.Info((object)string.Format("Calling: _imsapi.trUploadFailureAndResultData( \n _stationNumber: {0}, \n processLayer: {1}, \n SERIAL_NUMBER: {2}, \n SERIAL_NUMBER_POS: {3}, \n serialNumberState: {4}, \n 0, \n cycleTime: {5}, \n -1", (object)ItacFunctions._stationNumber, (object)num, (object)SERIAL_NUMBER, (object)SERIAL_NUMBER_POS, (object)serialNumberState, (object)cycleTime));
                int resultValue = ItacFunctions._imsapi.trUploadFailureAndResultData(ItacFunctions._sessionContext, ItacFunctions._stationNumber, num, SERIAL_NUMBER, SERIAL_NUMBER_POS, serialNumberState, 0, cycleTime, -1L, stringArray1, measureValues, out stringArray4, stringArray2, failureValues, out stringArray5, stringArray3, failureSlipValues, out strArray);
                if (resultValue != 0)
                {
                    ItacFunctions.printErrorText(resultValue);
                }
                else
                {
                    Logger.Log.Info((object)("UploadFailureAndResultData() measureResultValues " + ItacFunctions.alignResultString4Log(stringArray1, 4) + ItacFunctions.alignResultString4Log(stringArray4, 4)));
                    Logger.Log.Info((object)("UploadFailureAndResultData() failureResultValues " + ItacFunctions.alignResultString4Log(stringArray2, 2) + ItacFunctions.alignResultString4Log(stringArray5, 2)));
                    Logger.Log.Info((object)("UploadFailureAndResultData() failureSlipValues " + ItacFunctions.alignResultString4Log(stringArray3, 2) + ItacFunctions.alignResultString4Log(failureSlipValues, 2)));
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("UploadFailureAndResultData(): return with {0}", (object)flag));
            return flag;
        }

        public static bool AppendAttributeValues_MAC(string CURRENT_SERIAL_NUMBER, string SERIAL_NUMBER_POS, string[] MAC_ADDRESSES, int NumberOfMacAddressToUpdate, out string[] ResultString)
        {
            Logger.Log.Info((object)"AppendAttributeValues_MAC(): method called .");
            string str = string.Join(",", MAC_ADDRESSES, 0, NumberOfMacAddressToUpdate);
            Logger.Log.Info((object)string.Format("AppendAttributeValues_MAC( CURRENT_SERIAL_NUMBER = {0}, MAC_ADDRESSES = {1}", (object)CURRENT_SERIAL_NUMBER, (object)str));
            ItacFunctions.AttributeValue AttributeValue = new ItacFunctions.AttributeValue()
            {
                keys = new string[3]
              {
          "ATTRIBUTE_CODE",
          "ATTRIBUTE_VALUE",
          "ERROR_CODE"
              },
                values = new string[3] { "MAC", str, "0" }
            };
            bool flag = ItacFunctions.appendAttributeValues(CURRENT_SERIAL_NUMBER, SERIAL_NUMBER_POS, AttributeValue, out ResultString);
            Logger.Log.Info((object)string.Format("AppendAttributeValues_MAC(): return with {0}", (object)flag));
            return flag;
        }

        public static bool AppendAttributeValues_FAZIT(string CURRENT_SERIAL_NUMBER, string SERIAL_NUMBER_POS, string FAZIT_NUMBER_VALUE, out string[] resultSting)
        {
            Logger.Log.Info((object)"AppendAttributeValues_FAZIT(): method called .");
            ItacFunctions.AttributeValue AttributeValue = new ItacFunctions.AttributeValue()
            {
                keys = new string[3]
              {
          "ATTRIBUTE_CODE",
          "ATTRIBUTE_VALUE",
          "ERROR_CODE"
              },
                values = new string[6]
              {
          "FAZIT_NUMBER",
          FAZIT_NUMBER_VALUE,
          "0",
          "FAZIT_SENT",
          GenericFunctions.GetTimeStamp(),
          "0"
              }
            };
            bool flag = ItacFunctions.appendAttributeValues(CURRENT_SERIAL_NUMBER, SERIAL_NUMBER_POS, AttributeValue, out resultSting);
            Logger.Log.Info((object)string.Format("AppendAttributeValues_FAZIT(): return with {0}", (object)flag));
            return flag;
        }

        public static bool AppendAttributeValue(string CURRENT_SERIAL_NUMBER, string SERIAL_NUMBER_POS, string ATTRIBUTE_NAME, string ATTRIBUTE_VALUE, out string[] resultSting)
        {
            Logger.Log.Info((object)string.Format("AppendAttributeValue(): method called. ATTRIBUTE_NAME: {0}, ATTRIBUTE_VALUE: {1} .", (object)ATTRIBUTE_NAME, (object)ATTRIBUTE_VALUE));
            ItacFunctions.AttributeValue AttributeValue = new ItacFunctions.AttributeValue()
            {
                keys = new string[3]
              {
          "ATTRIBUTE_CODE",
          "ATTRIBUTE_VALUE",
          "ERROR_CODE"
              },
                values = new string[3]
              {
          ATTRIBUTE_NAME,
          ATTRIBUTE_VALUE,
          "0"
              }
            };
            bool flag = ItacFunctions.appendAttributeValues(CURRENT_SERIAL_NUMBER, SERIAL_NUMBER_POS, AttributeValue, out resultSting);
            Logger.Log.Info(string.Format("AppendAttributeValue(): return with {0}", flag));
            return flag;
        }

        public static bool GetAttributeValues(int OBJECT_TYPE, string OBJECT_NUMBER, string OBJECT_DETAIL, string[] ATTRIBUTE_CODE_ARRAY, int ALL_MERGED_LEVEL, string[] ATTRIBUTE_RESULT_KEYS, out string[] ATTRIBUTE_RESULT_VALUES)
        {
            Logger.Log.Info("getAttributeValues(): function called.");
            int result = 0;
            ATTRIBUTE_RESULT_VALUES = null;
            bool flag;
            try
            {
                result = _imsapi.attribGetAttributeValues(_sessionContext, _stationNumber, OBJECT_TYPE, OBJECT_NUMBER, OBJECT_DETAIL, ATTRIBUTE_CODE_ARRAY, ALL_MERGED_LEVEL, ATTRIBUTE_RESULT_KEYS, out ATTRIBUTE_RESULT_VALUES);
                if (result != 0)
                {
                    printErrorText(result);
                    flag = false;
                }
                else
                {
                    Logger.Log.Info(string.Format("getAttributeValues(): OUTPUT_VECTOR = {0}", string.Join(" ,", ATTRIBUTE_RESULT_VALUES)));
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info(string.Format("getAttributeValues(): return with {0}", flag));
            return flag;
        }

        public static bool GetBoxBarcode(out string PART_NUMBER, out string SERIAL_NUMBER, out string LEAR_INDEX, out string BOX_BARCODE)
        {
            Logger.Log.Info((object)"GetBoxBarcode(): method called.");
            string BOM_VERSION = "";
            string PROCESS_LAYER = "";
            string PROCESS_VERSION = "";
            string WORKORDER_NUMBER = "";
            PART_NUMBER = "";
            SERIAL_NUMBER = "";
            LEAR_INDEX = "";
            BOX_BARCODE = "";
            SerialNumberData[] serialNumberDataArray = new SerialNumberData[1];
            bool flag;
            try
            {
                Logger.Log.Info((object)"GetBoxBarcode(): Call GetStationSetting()");
                flag = ItacFunctions.GetStationSetting(out BOM_VERSION, out PART_NUMBER, out PROCESS_LAYER, out PROCESS_VERSION, out WORKORDER_NUMBER);
                if (flag)
                {
                    Logger.Log.Info((object)string.Format("Calling: _imsapi.trGetNextSerialNumber (\n _stationNumber: {0}, \n WORKORDER_NUMBER: {1}, \n PART_NUMBER: -1, \n 1. )", (object)ItacFunctions._stationNumber, (object)WORKORDER_NUMBER, (object)PART_NUMBER));
                    int nextSerialNumber = ItacFunctions._imsapi.trGetNextSerialNumber(ItacFunctions._sessionContext, ItacFunctions._stationNumber, WORKORDER_NUMBER, "-1", 1, out serialNumberDataArray);
                    if (nextSerialNumber != 0)
                    {
                        Logger.Log.Error((object)"GetSerialNumberInfo(): _imsapi.trGetNextSerialNumber failed");
                        ItacFunctions.printErrorText(nextSerialNumber);
                        flag = false;
                    }
                    else
                    {
                        SERIAL_NUMBER = (string)serialNumberDataArray[0].serialNumber;
                        Logger.Log.Info((object)string.Format("GetBoxBarcode(): serial number = {0} .", (object)SERIAL_NUMBER));
                    }
                }
                if (flag)
                {
                    Logger.Log.Info((object)"GetBoxBarcode(): Get Index from recipe. Calling GetRecipeData()");
                    flag = ItacFunctions.GetRecipeData("Index", PART_NUMBER, BOM_VERSION, PROCESS_VERSION, out LEAR_INDEX);
                }
                if (flag)
                {
                    BOX_BARCODE = PART_NUMBER + SERIAL_NUMBER + LEAR_INDEX;
                    Logger.Log.Info((object)string.Format("GetBoxBarcode(): PART_NUMBER: {0}, SERIAL_NUMBER: {1}, LEAR_INDEX: {2}, BOX_BARCODE: {3}.", (object)PART_NUMBER, (object)SERIAL_NUMBER, (object)LEAR_INDEX, (object)BOX_BARCODE));
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("GetBoxBarcode(): return with {0}", (object)flag));
            return flag;
        }

        private static bool itacLogin(string stationNumber, string stationPassword, string user, string password, string client, string registrationType, string systemIdentifier)
        {
            Logger.Log.Info((object)string.Format("itacLogin(): method called called."));
            IMSApiSessionValidationStruct validationStruct = new IMSApiSessionValidationStruct();
            IMSApiSessionContextStruct sessionContextStruct = (IMSApiSessionContextStruct)null;
            validationStruct.stationNumber = stationNumber;
            validationStruct.stationPassword = stationPassword;
            validationStruct.user = user;
            validationStruct.password = password;
            validationStruct.client = client;
            validationStruct.registrationType = registrationType;
            validationStruct.systemIdentifier = systemIdentifier;
            int resultValue = ItacFunctions._imsapi.regLogin(validationStruct, out sessionContextStruct);
            bool flag;
            if (resultValue != 0)
            {
                Logger.Log.Error((object)"itacLogin(): _imsapi.regLogin failed.");
                ItacFunctions.printErrorText(resultValue);
                flag = false;
            }
            else
            {
                ItacFunctions._sessionContext = sessionContextStruct;
                flag = true;
            }
            Logger.Log.Info((object)string.Format("itacLogin(): return with {0}", (object)flag));
            return flag;
        }

        private static bool getRecipeData(string MEASUREMENT_NAME, string[] RESULT_KEYS, string PART_NUMBER, string BOM_VERSION, string PROCESS_VERSION, out string[] OUTPUT_VECTOR)
        {
            Logger.Log.Info((object)"getRecipeData(): function called.");
            OUTPUT_VECTOR = (string[])null;
            bool flag;
            try
            {
                string[] strArray1 = (string[])null;
                KeyValue[] keyValueArray = new KeyValue[4]
                {
          new KeyValue()
          {
            key =  "PART_NUMBER",
            value =  PART_NUMBER
          },
          new KeyValue()
          {
            key =  "STATION_NUMBER",
            value =  ItacFunctions._stationNumber
          },
          new KeyValue()
          {
            key =  "BOM_VERSION",
            value =  BOM_VERSION
          },
          new KeyValue()
          {
            key =  "PROCESS_VERSION",
            value =  PROCESS_VERSION
          }
                };
                string[] strArray2 = RESULT_KEYS;
                Logger.Log.Info((object)string.Format("Calling: _imsapi.mdaGetRecipeData( \n _stationNumber: {0}, \n -1, \n -1, \n -1, \n MEASUREMENT_NAME: {1}, \n 0, \n -1, \n -1 , \n 3, \n recipeFilters, \n recipeResultKeys: {2})", (object)ItacFunctions._stationNumber, (object)MEASUREMENT_NAME, (object)string.Join(" ,", strArray2)));
                Logger.Log.Info((object)string.Format("_imsapi.mdaGetRecipeData 'recipeFilters': \n PART_NUMBER:{0}, \n STATION_NUMBER:{1}, \n BOM_VERSION:{2}, \n PROCESS_VERSION:{3}", (object)PART_NUMBER, (object)ItacFunctions._stationNumber, (object)BOM_VERSION, (object)PROCESS_VERSION));
                int recipeData = ItacFunctions._imsapi.mdaGetRecipeData(ItacFunctions._sessionContext, ItacFunctions._stationNumber, -1, "-1", "-1", MEASUREMENT_NAME, 0.0, "-1", "-1", 3, keyValueArray, strArray2, out strArray1);
                if (recipeData != 0)
                {
                    ItacFunctions.printErrorText(recipeData);
                    flag = false;
                }
                else
                {
                    OUTPUT_VECTOR = strArray1;
                    Logger.Log.Info((object)string.Format("getRecipeData(): OUTPUT_VECTOR = {0}", (object)string.Join(" ,", OUTPUT_VECTOR)));
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("getRecipeData(): return with {0}", (object)flag));
            return flag;
        }

        private static bool appendAttributeValues(string CURRENT_SERIAL_NUMBER, string SERIAL_NUMBER_POS, ItacFunctions.AttributeValue AttributeValue, out string[] ResultString)
        {
            Logger.Log.Info((object)"appendAttributeValues(): method called .");
            ResultString = (string[])null;
            bool flag;
            try
            {
                Logger.Log.Info((object)string.Format("Calling: _imsapi.attribAppendAttributeValues(\n _stationNumber: {0},\n 0,\n CURRENT_SERIAL_NUMBER: {1},\n ObjectDetail : {2},\n -1, \n 0, \n AttributeValue.keys: {3},\n AttributeValue.values: {3}", (object)ItacFunctions._stationNumber, (object)CURRENT_SERIAL_NUMBER, (object)SERIAL_NUMBER_POS, (object)string.Join(" ,", AttributeValue.keys), (object)string.Join(" ,", AttributeValue.values)));
                int resultValue = ItacFunctions._imsapi.attribAppendAttributeValues(ItacFunctions._sessionContext, ItacFunctions._stationNumber, 0, CURRENT_SERIAL_NUMBER, SERIAL_NUMBER_POS, -1L, 0, AttributeValue.keys, AttributeValue.values, out ResultString);
                if (resultValue != 0)
                {
                    Logger.Log.Error((object)"appendAttributeValues(): _imsapi.attribAppendAttributeValues failed");
                    ItacFunctions.printErrorText(resultValue);
                    flag = false;
                }
                else
                {
                    flag = true;
                    Logger.Log.Info((object)("appendAttributeValues() ResultString: \n ATTRIBUTE_CODE , \t ATTRIBUTE_VALUE \t , ERROR_CODE " + ItacFunctions.alignResultString4Log(ResultString, 3)));
                }
            }
            catch (Exception ex)
            {
                flag = false;
                string str = ex.Source + " " + ex.Message;
                Logger.Log.Fatal((object)str);
            }
            Logger.Log.Info((object)string.Format("appendAttributeValues(): return with {0}", (object)flag));
            return flag;
        }

        private static void printErrorText(int resultValue)
        {
            string str = "";
            if (ItacFunctions._imsapi.imsapiGetErrorText(ItacFunctions._sessionContext, resultValue, out str) != 0)
                str = "Unable to get errortext.";
            throw new Exception(string.Format("FATAL ERROR. resultValue: {0}\n errorText: {1}", (object)resultValue, (object)str));
        }

        private static string alignResultString4Log(string[] stringArray, int columns)
        {
            string str = "";
            if (stringArray != null)
            {
                for (int index = 0; index < stringArray.Length; ++index)
                    str = (index % columns != 0 ? str + " \t" : str + "\n") + stringArray[index];
            }
            return str;
        }

        private struct AttributeValue
        {
            public string[] keys;
            public string[] values;

            public AttributeValue(string[] keys, string[] values)
            {
                this.keys = keys;
                this.values = values;
            }
        }
    }
}
